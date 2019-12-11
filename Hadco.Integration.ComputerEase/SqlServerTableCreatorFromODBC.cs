using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using NUnit.Framework.Constraints;

namespace Hadco.Integration.ComputerEase
{
    // https://gist.github.com/ryankirkman/3094688
    // Sourced from: http://darrylagostinelli.com/2011/06/27/create-a-sql-table-from-a-datatable-in-c-net/
    // with modifications for copying from an ODBC data source
    public static class SqlServerTableCreatorFromOdbc
    {
        private const int SQLCommandTimeout = 5600;
        private const int OdbcCommandTimeout = 1800;
        public static void CreateAndPopulateTable(string odbcSelectText, string destinationTableName, string odbcConnectionString, string sqlServerConnectionString, string migrationSql, string logPath, bool loggingEnabled, bool doFullSync)
        {
            using (OdbcConnection conn = new OdbcConnection(odbcConnectionString))
            {
                using (SqlConnection sc = new SqlConnection(sqlServerConnectionString))
                {
                    sc.Open();
                    conn.Open();
                    CreateAndPopulateTable(destinationTableName, sc, conn, odbcSelectText, migrationSql, logPath, loggingEnabled, doFullSync);
                }
            }
        }

        public static void CreateAndPopulateTable(string tableName, SqlConnection sqlcon, OdbcConnection conn, string odbcSelectText, string migrationSql, string logPath, bool loggingEnabled, bool doFullSync)
        {
            string indexesSql;
            string dropIndexesSql;
            DropTableIfExists(tableName, sqlcon);

            if (loggingEnabled)
                File.AppendAllText(logPath,
                    string.Format("{0}\t{1}" + Environment.NewLine, DateTime.Now.ToString("s"), "Creating table for " + tableName));
            if (doFullSync && tableName == "Categories")
            {
                using (OdbcCommand oc = new OdbcCommand(string.Format(odbcSelectText, ""), conn))
                {
                    oc.CommandTimeout = OdbcCommandTimeout;
                    using (var reader = oc.ExecuteReader())
                    {
                        CreateTable(tableName, sqlcon, reader, out indexesSql, out dropIndexesSql);
                    }
                }

                using (
                    OdbcCommand oc = new OdbcCommand(string.Format(odbcSelectText, @"AND jcjob.dateopen Is Null"), conn)
                    )
                {
                    oc.CommandTimeout = OdbcCommandTimeout;
                    using (var reader = oc.ExecuteReader())
                    {
                        if (loggingEnabled)
                            File.AppendAllText(logPath,
                                string.Format("{0}\t{1}" + Environment.NewLine, DateTime.Now.ToString("s"),
                                    "Importing data into " + tableName));

                        PopulateTable(tableName, sqlcon, reader);
                    }
                }
                for (int i = 1; i < 24; i++)
                {
                    var condition = @"AND(jcjob.dateopen>" + $@"{{d'{DateTime.Now.Date.AddMonths(1 - i):yyyy-MM-dd}'}}" +
                                    @" AND jcjob.dateopen<" + $@"{{d'{DateTime.Now.Date.AddMonths(-i):yyyy-MM-dd}'}}" +
                                    ")";
                    using (OdbcCommand oc = new OdbcCommand(string.Format(odbcSelectText, condition), conn))
                    {
                        oc.CommandTimeout = OdbcCommandTimeout;
                        using (var reader = oc.ExecuteReader())
                        {
                            if (loggingEnabled)
                                File.AppendAllText(logPath,
                                    string.Format("{0}\t{1}" + Environment.NewLine, DateTime.Now.ToString("s"),
                                        "Importing data into " + tableName));

                            PopulateTable(tableName, sqlcon, reader);
                        }
                    }
                }
            }
            else
            {
                using (OdbcCommand oc = new OdbcCommand(odbcSelectText, conn))
                {
                    oc.CommandTimeout = OdbcCommandTimeout;
                    using (var reader = oc.ExecuteReader())
                    {
                        CreateTable(tableName, sqlcon, reader, out indexesSql, out dropIndexesSql);

                        if (loggingEnabled)
                            File.AppendAllText(logPath,
                                string.Format("{0}\t{1}" + Environment.NewLine, DateTime.Now.ToString("s"),
                                    "Importing data into " + tableName));

                        PopulateTable(tableName, sqlcon, reader);
                    }

                    if (loggingEnabled)
                        File.AppendAllText(logPath,
                            string.Format("{0}\t{1}" + Environment.NewLine, DateTime.Now.ToString("s"),
                                "Creating indexes for " + tableName));
                }

                if (indexesSql.Length > 0)
                {
                    CreateIndexes(indexesSql, sqlcon);
                }
            }
            if (loggingEnabled)
                File.AppendAllText(logPath,
                    string.Format("{0}\t{1}" + Environment.NewLine, DateTime.Now.ToString("s"),
                        "Running merge for " + tableName));

            // migrate the data
            {
                using (SqlCommand cmd = new SqlCommand(migrationSql, sqlcon))
                {
                    cmd.CommandTimeout = 5600;
                    if (tableName.Equals("Jobs", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var param = new SqlParameter("@getdate", DateTime.Now);
                        cmd.Parameters.Add(param);
                    }
                    cmd.ExecuteNonQuery();
                }
            }
            //if(loggingEnabled)
            //File.AppendAllText(logPath, string.Format("{0}\t{1}" + Environment.NewLine, DateTime.Now.ToString("s"), "Dropping raw table " + tableName));

            //DropTableIfExists(tableName, sqlcon);
        }

        public static void PopulateTable(string tableName, SqlConnection sqlcon, OdbcDataReader reader)
        {
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlcon, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.KeepNulls | SqlBulkCopyOptions.KeepIdentity, null))
            {
                bulkCopy.DestinationTableName = "[Raw]." + tableName;
                bulkCopy.BulkCopyTimeout = 1800;

                // This is roughly optimal it seems: http://sqlblog.com/blogs/linchi_shea/archive/2011/07/01/performance-impact-what-is-the-optimal-payload-for-sqlbulkcopy-writetoserver.aspx
                bulkCopy.BatchSize = Convert.ToInt32(ConfigurationManager.AppSettings["BulkCopyBatchSize"]);
                bulkCopy.WriteToServer(reader);
            }
        }

        public static int DropIndexes(string dropIndexesSql, SqlConnection sqlcon)
        {
            using (SqlCommand cmd = new SqlCommand(dropIndexesSql, sqlcon))
            {
                cmd.CommandTimeout = SQLCommandTimeout;
                return cmd.ExecuteNonQuery();
            }
        }

        public static int CreateIndexes(string indexesSql, SqlConnection sqlcon)
        {
            using (SqlCommand cmd = new SqlCommand(indexesSql, sqlcon))
            {

                cmd.CommandTimeout = SQLCommandTimeout;
                return cmd.ExecuteNonQuery();
            }
        }

        public static int CreateTable(string tableName, SqlConnection sqlcon, OdbcDataReader reader, out string indexesSql, out string dropIndexesSql)
        {
            string sql = GenerateTableSQL(tableName, reader, out indexesSql, out dropIndexesSql);
            using (SqlCommand cmd = new SqlCommand(sql, sqlcon))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        public static string GenerateTableSQL(string tableName, OdbcDataReader reader, out string indexesSql, out string dropIndexesSql)
        {
            StringBuilder sql = new StringBuilder("CREATE TABLE [Raw].[" + tableName + "] (\n");
            StringBuilder indexes = new StringBuilder();
            StringBuilder dropIndexes = new StringBuilder();

            int fCount = reader.FieldCount;
            for (int i = 0; i < fCount; i++)
            {
                var columnName = reader.GetName(i);
                bool isIdField = columnName.EndsWith("num") && !columnName.EndsWith("phonenum");
                sql.Append("\t[" + columnName + "] " + SQLGetType(reader.GetFieldType(i), isIdField) + ",\n");

                if (isIdField)
                {
                    dropIndexes.AppendLine($"Drop index idx_{columnName} on [raw].[{tableName}];");
                    indexes.AppendLine(string.Format("create index idx_{0} on [raw].[{1}] ({0});", columnName, tableName));
                }
            }
            indexesSql = indexes.ToString();
            dropIndexesSql = dropIndexes.ToString();
            return sql.ToString().TrimEnd(new char[] { ',', '\n' }) + "\n);" + Environment.NewLine;
        }

        public static int TruncateTable(string tableName, SqlConnection sqlcon)
        {
            string sql = $"TRUNCATE TABLE [Raw].{tableName}";
            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            return cmd.ExecuteNonQuery();
        }

        public static int DropTableIfExists(string tableName, SqlConnection sqlcon)
        {
            string sql = $"IF OBJECT_ID('[Raw].{tableName}') IS NOT NULL DROP TABLE [Raw].{tableName}";
            using (SqlCommand cmd = new SqlCommand(sql, sqlcon))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        // Return T-SQL data type definition, based on schema definition for a column
        public static string SQLGetType(Type type, bool isIdField)
        {
            switch (type.ToString())
            {
                case "System.String":
                    if (isIdField)
                    {
                        return "VARCHAR(256)";
                    }
                    return "VARCHAR(max)";

                case "System.Decimal":
                case "System.Double":
                case "System.Single":
                    return "REAL";

                case "System.Int64":
                    return "BIGINT";

                case "System.Int16":
                case "System.Int32":
                    return "INT";

                case "System.DateTime":
                    return "DATETIME";

                case "System.Boolean":
                    return "BIT";

                case "System.Byte":
                    return "TINYINT";

                case "System.Guid":
                    return "UNIQUEIDENTIFIER";

                default:
                    throw new Exception(type.ToString() + " not implemented.");
            }
        }
    }
}