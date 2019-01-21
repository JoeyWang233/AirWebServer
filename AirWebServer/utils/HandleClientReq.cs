using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AirWebServer.utils {
    public class HandleClientReq {
        public static DataTable getList(string DevSN, int page, string table) {
            string sql = "select distinct DevSN from " + table + (DevSN == null ? "": " where DevSN='" + DevSN + "'");
            return DbUtil.ExecuteQuery(sql);
        }
    }
}
