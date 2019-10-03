
using System;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using Terrasoft.Core;
using Terrasoft.Core.Entities;
using Terrasoft.Common;
using System.Linq;
using Terrasoft.Core.DB;

namespace Terrasoft.Configuration
{

    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class NavLoggerHelper
    {
        private static UserConnection Connection;
        private string _LoggerFile = "NavActualLoggerFile";


        public NavLoggerHelper(UserConnection userConnection)
        {
            if(userConnection != null)
                Connection = userConnection;
            else
                Connection = (UserConnection)HttpContext.Current.Session["UserConnection"];
        }

        public NavLoggerHelper()
        {
            Connection = (UserConnection)HttpContext.Current.Session["UserConnection"];
        }


        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "GetSysSettingValue", BodyStyle = WebMessageBodyStyle.Wrapped,
                RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public (Guid id, string name) GetSysSettingValue()
        {
            try
            {
                var sel = new Select(Connection)
                    .Column("SysSettings", "ReferenceSchemaUId")
                    .Column("SysSchema", "Name")
                    .From("SysSettings")
                    .Join(JoinType.Inner, "SysSchema")
                    .On("SysSettings", "ReferenceSchemaUId").IsEqual("SysSchema", "Uid")
                    .Where("SysSettings", "Code").IsEqual(Column.Parameter(_LoggerFile))
                 as Select;

                Guid referenceSchemaUId = Guid.Empty;
                string name = "";

                using (var dbExecutor = Connection.EnsureDBConnection())
                {
                    using (var dataReader = sel.ExecuteReader(dbExecutor))
                    {
                        while (dataReader.Read())
                        {
                            referenceSchemaUId = dataReader.GetGuid(0);
                            name = dataReader.GetString(1);
                        }
                    }
                }

                return (referenceSchemaUId, name);



            }
            catch (Exception ex)
            {
              	throw new Exception(ex.Message);
            }


		 }
}
}
