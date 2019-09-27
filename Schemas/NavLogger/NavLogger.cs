namespace Terrasoft.Configuration
{
    using System;
    using System.Linq;
    using Terrasoft.Common;
    using System.Text;
    using Terrasoft.Core;
    using System.Web;
    using Terrasoft.Core.DB;



    public class NavLogger
    {
        private static UserConnection Connection;
        public static Guid Supervisor = new Guid("410006E1-CA4E-4502-A9EC-E54D922D2C00");


        public NavLogger(UserConnection userConnection)
        {
            Connection = userConnection;
        }

        public NavLogger()
        {
            Connection = (UserConnection)HttpContext.Current.Session["UserConnection"];
        }

        public void Info(string text)
        {
            text = DateTime.Now.ToString() + " Info: " + text + "\r\n";
            UpdateLogFile(text);
        }

        public void Error(string text)
        {
            text = DateTime.Now.ToString() + " Error: " + text + "\r\n";
            UpdateLogFile(text);
 
        }

        private void UpdateLogFile(string text)
        {
            byte[] newLog = Encoding.UTF8.GetBytes(text);
            byte[] oldLog = GetOldTExt();

            if (oldLog != null)
            {
                newLog = oldLog.Concat(newLog).ToArray();
            }

            var updateLog = new Update(Connection, "ContactFile")
                    .Set("Data", Column.Parameter(newLog))
                    .Where("ContactId").IsEqual(Column.Parameter(Supervisor));
            updateLog.Execute();
        }

        private byte[] GetOldTExt()
        {
            var sel = new Select(Connection)
               .Column("Data")
               .From("ContactFile")
               .Where("ContactId").IsEqual(Column.Parameter(Supervisor))
               as Select;

            using (var dbExecutor = Connection.EnsureDBConnection())
            {
                return sel.ExecuteScalar<byte[]>();
            }
        }
    }
    
}