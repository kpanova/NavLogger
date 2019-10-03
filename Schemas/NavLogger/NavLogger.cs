using System;
using System.Linq;
using System.Text;
using Terrasoft.Core;
using Terrasoft.Core.DB;
using Quartz;
using System.Threading.Tasks;
using Quartz.Impl;

namespace Terrasoft.Configuration
{
    public class NavLogger
    {
        public static Guid Supervisor = new Guid("410006E1-CA4E-4502-A9EC-E54D922D2C00");
        public static UserConnection Connection { get; set; }
        public static Guid CurrentLogFileGuid { get; set; }
        public static string FileEntityName { get; set; }

        private static StringBuilder _oldText = new StringBuilder();
        private static StringBuilder _currentText = new StringBuilder();

        public static void Info(string text)
        {
            var time = DateTime.Now;
            _currentText.AppendLine($"{time}.{time.Millisecond} Info: {text}");
        }

        public static void Error(string text)
        {
            var time = DateTime.Now;
            _currentText.AppendLine($"{time}.{time.Millisecond} Error: " + text + "");
        }
        public static void RecordTimingStart(string text)
        {
            var time = DateTime.Now;
            _currentText.AppendLine($"{time}.{time.Millisecond} Operation: {text} started");
        }
        public static void RecordTimingEnd(string text)
        {
            var time = DateTime.Now;
            _currentText.AppendLine($"{time}.{time.Millisecond} Operation: {text} ended");
        }

        public static void UpdateLogFile()
        {
            ReadFileData();
            _oldText = new StringBuilder(_currentText.ToString());
            _currentText = new StringBuilder();
            byte[] newLog = Encoding.UTF8.GetBytes(_oldText.ToString());
            byte[] oldLog = GetOldTExt();

            if (oldLog != null)
            {
                newLog = oldLog.Concat(newLog).ToArray();
            }

            var updateLog = new Update(Connection, FileEntityName)
                    .Set("Data", Column.Parameter(newLog))
                    .Where("Id").IsEqual(Column.Parameter(CurrentLogFileGuid));
            updateLog.Execute();
        }

        private static byte[] GetOldTExt()
        {
            var sel = new Select(Connection)
               .Column("Data")
               .From(FileEntityName)
                    .Where("Id").IsEqual(Column.Parameter(CurrentLogFileGuid))
               as Select;

            using (var dbExecutor = Connection.EnsureDBConnection())
            {
                return sel.ExecuteScalar<byte[]>();
            }
        }

        private static void ReadFileData()
        {
            NavLoggerHelper loggerHelper = new NavLoggerHelper(Connection);
            FileEntityName = loggerHelper.GetSysSettingValue().name;
            CurrentLogFileGuid = new Guid(Core.Configuration.SysSettings.GetValue(Connection, "NavActualLoggerFile").ToString());
        }

        public static void ClearLog()
        {
            _oldText = new StringBuilder();
            _currentText = new StringBuilder();
        }
    }
    public class NavLoggerWriter : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() => NavLogger.UpdateLogFile());
        }
    }

    public class NavLoggerWriterScheduler
    {
        public static bool Started;
        public static async void Start()
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await Task.Run(() => scheduler.Start());

            IJobDetail job = JobBuilder.Create<NavLoggerWriter>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("UpdateLog", "UpdateLogGroup")
                .StartAt(DateTimeOffset.UtcNow.AddSeconds(5))
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(5)
                    .RepeatForever())
                .Build();

            await Task.Run(() => scheduler.ScheduleJob(job, trigger)); 
        }
    }

}