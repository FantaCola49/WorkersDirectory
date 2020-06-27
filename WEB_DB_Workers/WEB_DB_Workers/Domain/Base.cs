using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEB_DB_Workers.Domain
{
    public static class Base
    {
        private static string ConnectionString
        {
            get
            { return System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]; }
        }

        public static string strConnect
        {
            get
            { return System.Configuration.ConfigurationManager.ConnectionStrings[ConnectionString].ConnectionString; }
        }
    }
}