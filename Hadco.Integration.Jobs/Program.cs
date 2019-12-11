using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Hadco.Integration.Jobs
{
    class Program
    {
        static void Main(string[] args)
        {
            Job job = new Job()
            {
                JobNumber = "KCISCOOL2",
                Name = "Cool Casey",
                Address1 = "1925 N 825 E",
                City = "Lehi",
                State = "UT",
                Zip = "84043",
                CustomerNumber = "IVORYP",
                Class = "bo",
                DateOpen = "{d'2015-11-07'}",
                Status = 1,
                Memo = "nothing of note",
                DateFiled = "{d'2015-11-07'}",
                PreliminaryFilingNumber = "34343"
            };
            string odbcConnectionString = ConfigurationManager.ConnectionStrings["Hadco.ComputerEase"].ConnectionString;
            InsertJob(job, odbcConnectionString);
        }

        public static void InsertJob(Job job, string connectionString)
        {
            var insertSql = $@"INSERT into jcjob (jobnum, name, address, addresscity, addressstate, addresszip, cusnum, class, status, memo, dateopen) 
values ('{job.JobNumber}', '{job.Name}', '{job.Address1}', '{job.City}', '{job.State}', '{job.Zip}', '{job.CustomerNumber}', '{job.Class}', {job.Status}, '{job.Memo}', {job.DateOpen})";
            using (OdbcConnection conn = new OdbcConnection(connectionString))
            {
                using (OdbcCommand comm = new OdbcCommand(insertSql, conn))
                {
                    conn.Open();
                    comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public class Job
        {
            public string JobNumber { get; set; }

            public string Name { get; set; }

            public string Address1 { get; set; }

            public string City { get; set; }

            public string State { get; set; }

            public string Zip { get; set; }

            public string CustomerNumber { get; set; }

            public string Class { get; set; }

            public string DateOpen { get; set; }

            public int Status { get; set; }

            public string Memo { get; set; }

            public string DateFiled { get; set; }

            public string PreliminaryFilingNumber { get; set; }
        }
    }
}
