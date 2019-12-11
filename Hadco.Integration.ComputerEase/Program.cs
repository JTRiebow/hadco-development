using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using Topshelf;
using System.Timers;
using System.Reflection;
using System.IO;

namespace Hadco.Integration.ComputerEase
{
    public class ComputereaseIntegration
    {
        readonly Timer _timer;
        private string _path;
        private DateTime _lastTimeFullSync;
        private static bool _currentlyRunning;

        public ComputereaseIntegration()
        {
            string odbcConnectionString = ConfigurationManager.ConnectionStrings["Hadco.ComputerEase"].ConnectionString;
            string destinationConnectionString = ConfigurationManager.ConnectionStrings["Destination"].ConnectionString;
            int synchronizeMinutesDelay = Convert.ToInt32(ConfigurationManager.AppSettings["SychronizeMinutesDelay"]);
            int fullSynchronizationHourStart = Convert.ToInt32(ConfigurationManager.AppSettings["FullSynchronizationHourStart"]);
            int fullSynchronizationHourEnd = Convert.ToInt32(ConfigurationManager.AppSettings["FullSynchronizationHourEnd"]);
            bool loggingEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["LoggingEnabled"]);
            _path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "log.txt");
            _lastTimeFullSync = DateTime.MinValue;

            _timer = new Timer(synchronizeMinutesDelay * 60 * 1000) { AutoReset = true };
                     
            //_timer = new Timer(synchronizeMinutesDelay) { AutoReset = true }; // ONLY FOR DEBUGGING!!!

            _timer.Elapsed += (sender, eventArgs) =>
            {
                if (!_currentlyRunning)
                {
                    _currentlyRunning = true;
                    try
                    {
                        DateTime starttime = DateTime.Now;
                        bool doFullSync = _lastTimeFullSync.Date != starttime.Date && starttime.Hour >= fullSynchronizationHourStart && starttime.Hour < fullSynchronizationHourEnd;

                        if (doFullSync )
                        {
                            _lastTimeFullSync = starttime;
                            if(loggingEnabled)
                            File.AppendAllText(_path,
                                $"{DateTime.Now.ToString("s")}\t{"Starting full sync." + Environment.NewLine}");
                        }
                        else
                        {
                            if (loggingEnabled)
                                File.AppendAllText(_path,
                                    $"{DateTime.Now.ToString("s")}\t{"Starting partial sync." + Environment.NewLine}");
                        }

                        foreach (var table in Table.GetTables())
                        {
                            if (loggingEnabled)
                                File.AppendAllText(_path, string.Format("{0}\t{1}" + Environment.NewLine, DateTime.Now.ToString("s"), "Starting migration process for " + table.TableName));

                            string odbcSelect = doFullSync ? table.OdbcSelectText : table.PartialOdbcSelectText;
                            string migrationSql = doFullSync ? table.MigrationSql : table.PartialMigrationSql;

                            SqlServerTableCreatorFromOdbc.CreateAndPopulateTable(odbcSelect, table.TableName, odbcConnectionString, destinationConnectionString, migrationSql, _path, loggingEnabled, doFullSync);
                        }
                        if (loggingEnabled)
                            File.AppendAllText(_path,
                                $"{DateTime.Now.ToString("s")}\t{"Sync Completed." + Environment.NewLine}");
                    }
                    catch(Exception e)
                    {
                        File.AppendAllText(_path, $"{DateTime.Now.ToString("s")}\t{e.Message + Environment.NewLine}");
                    }
                    finally
                    {
                        _currentlyRunning = false;
                    }
                }
            };
        }

        public void Start()
        {
            _timer.Start();
            File.AppendAllText(_path, $"{DateTime.Now.ToString("s")}{"\tService Started" + Environment.NewLine}");
        }
        public void Stop()
        {
            File.AppendAllText(_path, $"{DateTime.Now.ToString("s")}{"\tService Stop Issued" + Environment.NewLine}");
            _timer.Stop();
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {

                x.Service<ComputereaseIntegration>(service =>
                {
                    service.ConstructUsing(() => new ComputereaseIntegration());
                    service.WhenStarted(a => a.Start());
                    service.WhenStopped(a => a.Stop());
                });
                x.RunAsLocalSystem();
                x.SetDescription("Synchronizes ComputerEase with time.hadcoconstruction.com.");
                x.SetDisplayName("Hadco.Integration.ComputerEase");
                x.SetServiceName("Hadco.Integration.ComputerEase");
            });
        }
    }
}
