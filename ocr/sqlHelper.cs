using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ocr
{
    class sqlHelper
    {
        private string connectString;

        public string CreateConnectString(string serverName, string userName, string psw) {
            string serverInfo = string.Format("Data Source=" + serverName + ",1433;Network Library=DBMSSOCN;Initial Catalog=");
            string pwd = ";User ID=" + userName + ";PWD=" + psw;
            string connString = string.Format("{0}{1}{2}", serverInfo, "temp04", pwd);
            return connString;
        }




    }
}