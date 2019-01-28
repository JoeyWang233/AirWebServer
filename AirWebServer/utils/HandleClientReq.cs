using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AirWebServer.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AirWebServer.utils {
    public class HandleClientReq {
        public static DataTable getList(string DevSN, int page, string table) {
            string sql = "select distinct DevSN from " + table + (DevSN == null ? "" : " where DevSN='" + DevSN + "'");
            return DbUtil.ExecuteQuery(sql);
        }

        public static ActionResult<HomeDataObject> homeAll(string type, int statusNo, int mcTransNo, int mcTransNum, int statusNum, string DevSN, string[] EventTime, int pageSize) {
            DataTable statusData, mcTransData;
            HomeDataObject homeDataObject = new HomeDataObject();
            string sql;

            if (type == "first" || type == "right") {
                //查找Device_Status表
                sql = $"select no= Identity(int,1,1),* Into #status From Device_Status " +
                    $"{(DevSN != null || EventTime[0] != "undefined" ? " where " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != "undefined" ? ((DevSN != null ? "and " : "") + "EventTime>='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != "undefined" ? (" and EventTime <='" + EventTime[1] + "'") : "")}" +
                    "order by EventTime DESC " +
                    $"Select * From #status where no>{statusNo} and no <= {pageSize + statusNo}" +
                    " Drop Table #status ";
                Console.Write(sql);
                statusData = DbUtil.ExecuteQuery(sql);

                //查找McTrans表
                sql = $"select no= Identity(int,1,1),* Into #mctrans From McTrans " +
                    $"{(DevSN != null || EventTime[0] != "undefined" ? " where " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != "undefined" ? ((DevSN != null ? "and " : "") + "EventTime>='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != "undefined" ? (" and EventTime <='" + EventTime[1] + "'") : "")}" +
                    "order by EventTime DESC " +
                    $"Select * From #mctrans where no>{mcTransNo} and no <= {pageSize + mcTransNo}" +
                    " Drop Table #mctrans ";
                mcTransData = DbUtil.ExecuteQuery(sql);

                //对查询结果进行处理


            } else if (type == "last") {
                //查找Device_Status表符合条件的count
                sql = $"select count=count(*) From Device_Status " +
                    $"{(DevSN != null || EventTime[0] != "undefined" ? " where " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != "undefined" ? ((DevSN != null ? "and " : "") + "EventTime>='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != "undefined" ? (" and EventTime <='" + EventTime[1] + "'") : "")}";
                statusNo = int.Parse(DbUtil.ExecuteQuery(sql).Rows[0]["count"].ToString());

                //查找McTrans表符合条件的count
                sql = $"select count=count(*) From McTrans " +
                    $"{(DevSN != null || EventTime[0] != "undefined" ? " where " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != "undefined" ? ((DevSN != null ? "and " : "") + "EventTime>='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != "undefined" ? (" and EventTime <='" + EventTime[1] + "'") : "")}";
                mcTransNo = int.Parse(DbUtil.ExecuteQuery(sql).Rows[0]["count"].ToString());

                int total = statusNo + mcTransNo;
                if (total % pageSize != 0)
                    pageSize = total % pageSize;

                //查找Device_Status表
                sql = $"select top {pageSize} * From Device_Status " +
                    $"{(DevSN != null || EventTime[0] != "undefined" ? " where " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != "undefined" ? ((DevSN != null ? "and " : "") + "EventTime>='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != "undefined" ? (" and EventTime <='" + EventTime[1] + "'") : "")}" +
                    " order by EventTime ";
                statusData = DbUtil.ExecuteQuery(sql);

                //查找McTrans表
                sql = $"select top {pageSize} * From McTrans " +
                    $"{(DevSN != null || EventTime[0] != "undefined" ? " where " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != "undefined" ? ((DevSN != null ? "and " : "") + "EventTime>='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != "undefined" ? (" and EventTime <='" + EventTime[1] + "'") : "")}" +
                    " order by EventTime ";
                mcTransData = DbUtil.ExecuteQuery(sql);

                //对查询结果进行处理

            } else {
                //type=="left"
                //查找Device_Status表
                sql = $"select top {pageSize} * From Device_Status " +
                    $"where EventTime in (select top {statusNo - statusNum} EventTime from Device_Status " +
                    $"{(DevSN != null || EventTime[0] != "undefined" ? " where " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != "undefined" ? ((DevSN != null ? "and " : "") + "EventTime>='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != "undefined" ? (" and EventTime <='" + EventTime[1] + "'") : "")}" +
                    "order by EventTime DESC) " +
                    $"{(DevSN != null || EventTime[0] != "undefined" ? " and " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != "undefined" ? ((DevSN != null ? "and " : "") + "EventTime>='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != "undefined" ? (" and EventTime <='" + EventTime[1] + "'") : "")}" +
                    " order by EventTime ";
                statusData = DbUtil.ExecuteQuery(sql);

                //查找McTrans表
                sql = $"select top {pageSize} * From McTrans " +
                    $"where EventTime in (select top {mcTransNo - mcTransNum} EventTime from McTrans " +
                    $"{(DevSN != null || EventTime[0] != "undefined" ? " where " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != "undefined" ? ((DevSN != null ? "and " : "") + "EventTime>='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != "undefined" ? (" and EventTime <='" + EventTime[1] + "'") : "")}" +
                    "order by EventTime DESC) " +
                    $"{(DevSN != null || EventTime[0] != "undefined" ? " and " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != "undefined" ? ((DevSN != null ? "and " : "") + "EventTime>='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != "undefined" ? (" and EventTime <='" + EventTime[1] + "'") : "")}" +
                    " order by EventTime ";
                mcTransData = DbUtil.ExecuteQuery(sql);
            }

            return handleResult(statusData, mcTransData, type, pageSize, mcTransNo, statusNo, mcTransNum, statusNum);
        }

        private static HomeDataObject handleResult(DataTable statusData, DataTable mcTransData, string type, int pageSize, int mcTransNo, int statusNo, int mcTransNum, int statusNum) {
            Dictionary<string, object> map;
            List<Dictionary<string, object>> homeDataList = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> mcTransDataList = new List<Dictionary<string, object>>();
            List<int> isMcTrans = new List<int>();
            DataTable AlarmData = new DataTable();
            List<Dictionary<string, object>> AlarmDataList = new List<Dictionary<string, object>>();
            DataTable LoginData = new DataTable();
            List<Dictionary<string, object>> LoginDataList = new List<Dictionary<string, object>>();
            HomeDataObject homeDataObject = new HomeDataObject();
            string sql;

            DateTime dti;
            DateTime dtj;

            if (statusData.Rows.Count != 0) {
                //测试：将每一行数据做成键值对，存入到homeDataList1中
                foreach (DataRow row in statusData.Rows) {
                    map = new Dictionary<string, object>();
                    foreach (DataColumn column in statusData.Columns) {
                        map.Add(column.ToString(), row[column]);
                    }
                    homeDataList.Add(map);
                }

                foreach (DataRow row in mcTransData.Rows) {
                    map = new Dictionary<string, object>();
                    foreach (DataColumn column in mcTransData.Columns) {
                        map.Add(column.ToString(), row[column]);
                    }
                    mcTransDataList.Add(map);
                }

                int index = 0;
                for (int i = 0; i < mcTransDataList.Count; i++) {
                    for (int j = index; j < homeDataList.Count; j++) {
                        dti = DateTime.Parse((string)mcTransDataList[i]["EventTime"]);
                        dtj = DateTime.Parse((string)homeDataList[j]["EventTime"]);
                        if ((type == "first" || type == "right") ? (DateTime.Compare(dti, dtj) > 0) : (DateTime.Compare(dti, dtj) <= 0)) {
                            homeDataList.Insert(j, mcTransDataList[i]);
                            index = j + 1;
                            isMcTrans.Add(j);
                            break;
                        }
                        if (j >= pageSize - 1) {
                            index = j + 1;
                            break;
                        }
                    }
                    if (index >= pageSize)
                        break;
                }
                if (homeDataList.Count < pageSize) {
                    int len = homeDataList.Count();
                    int extraLen = pageSize - len;
                    // out of range,maybe mcTransDataList.length is short of isMcTrans.Count+extraLen
                    for (int i = isMcTrans.Count; i < extraLen + isMcTrans.Count; i++) {
                        homeDataList.Add(mcTransDataList[i]);
                    }
                    for (int i = len; i < homeDataList.Count; i++)
                        isMcTrans.Add(i);
                }

                //homeData = homeData.slice(0,pageSize);
                homeDataList = homeDataList.GetRange(0, pageSize);
            } else {
                foreach (DataRow row in mcTransData.Rows) {
                    map = new Dictionary<string, object>();
                    foreach (DataColumn column in mcTransData.Columns) {
                        map.Add(column.ToString(), row[column]);
                    }
                    homeDataList.Add(map);
                }

                for (int i = 0; i < mcTransDataList.Count; i++)
                    isMcTrans.Add(i);
            }

            int dataLength = homeDataList.Count();

            if (type == "first") {
                mcTransNo = isMcTrans.Count();
                statusNo = dataLength - isMcTrans.Count();
            } else if (type == "right") {
                mcTransNo += isMcTrans.Count();
                statusNo += dataLength - isMcTrans.Count();
            } else if (type == "left") {
                mcTransNo = mcTransNo - mcTransNum;
                statusNo = statusNo - statusNum;
                homeDataList.Reverse();
                for (int i = 0; i < isMcTrans.Count(); i++) {
                    isMcTrans[i] = dataLength - isMcTrans[i] - 1;
                }
            } else {
                homeDataList.Reverse();
                for (int i = 0; i < isMcTrans.Count(); i++) {
                    isMcTrans[i] = dataLength - isMcTrans[i] - 1;
                }
            }

            //判断homeDataList中的statusData数据是否是Alarm/Login数据，如果是，需要添加额外的字段
            if (dataLength != isMcTrans.Count()) {
                for (int i = 0; i < homeDataList.Count(); i++) {
                    if (!isMcTrans.Contains<int>(i)) {

                        sql = $"select * from TABLE_Alarm where EVENT_TIME = '{homeDataList[i]["EventTime"]}' and DevSN = '{homeDataList[i]["DevSN"]}'";
                        AlarmData = DbUtil.ExecuteQuery(sql);
                        if (AlarmData.Rows.Count != 0) {
                            //此条row是Alarm类型
                            foreach (DataRow row in AlarmData.Rows) {
                                map = new Dictionary<string, object>();
                                foreach (DataColumn column in AlarmData.Columns) {
                                    map.Add(column.ToString(), row[column]);
                                }
                                AlarmDataList.Add(map);
                            }
                            homeDataList[i].Add("alarm", AlarmDataList);
                        }

                        sql = $"select * from Device_Login where LoginTime = '{homeDataList[i]["EventTime"]}.000' and DevSN = '{homeDataList[i]["DevSN"]}'";
                        LoginData = DbUtil.ExecuteQuery(sql);
                        if (LoginData.Rows.Count != 0) {
                            //此条row是Login类型
                            foreach (DataRow row in LoginData.Rows) {
                                map = new Dictionary<string, object>();
                                foreach (DataColumn column in LoginData.Columns) {
                                    map.Add(column.ToString(), row[column]);
                                }
                                LoginDataList.Add(map);
                            }
                            homeDataList[i].Add("login", LoginDataList);
                        }
                    }
                }
            }
            //封装homeDataObject
            homeDataObject.homeData = homeDataList.ToArray();
            homeDataObject.isMcTrans = isMcTrans.ToArray();
            homeDataObject.statusNo = statusNo;
            homeDataObject.mcTransNo = mcTransNo;
            return homeDataObject;
        }

        public static ActionResult<HomeDataObject> homeStatus(string type, int statusNo, string DevSN, string[] Index, string[] EventTime, int pageSize) {
            //仅一级菜单时调用，仅包含Device_Status数据
            string sql;
            DataTable statusData = new DataTable();
            Dictionary<string, Object> map;
            List<Dictionary<string, Object>> homeDataList = new List<Dictionary<string, object>>();
            HomeDataObject homeDataObject = new HomeDataObject();
            if (type != "last") {
                switch (type) {
                    case "first":
                        statusNo = 0;
                        break;
                    case "left":
                        statusNo -= 1;
                        break;
                    default:
                        statusNo = +1;
                        break;
                }

                sql = $"select no= Identity(int,1,1),* Into #status From Device_Status " +
                    $"{((DevSN != null || Index[0] != "undefined" || Index[1] != "undefined" || EventTime[0] != "undefined" || EventTime[1] != "undefined") ? " where " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != "undefined" ? ((DevSN != null ? "and " : "") + "EventTime>='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != "undefined" ? (" and EventTime <='" + EventTime[1] + "'") : "")}" +
                    $"{(Index[0] != "undefined" ? ((DevSN != null || EventTime[0] != "undefined" ? "and " : "") + "C_INDEX >= '" + Index[0] + "'") : "")}" +
                    $"{(Index[1] != "undefined" ? ((DevSN != null || EventTime[0] != "undefined" || Index[0] != "undefined" ? "and " : "") + "C_INDEX <= '" + Index[1] + "'") : "")}" +
                    "order by EventTime DESC " +
                    $"Select * From #status where no>{statusNo * pageSize} and no <= {(statusNo + 1) * pageSize}" +
                    " Drop Table #status ";
                statusData = DbUtil.ExecuteQuery(sql);

                foreach (DataRow row in statusData.Rows) {
                    map = new Dictionary<string, object>();
                    foreach (DataColumn column in statusData.Columns) {
                        map.Add(column.ToString(), row[column]);
                    }
                    homeDataList.Add(map);
                }

                homeDataObject.homeData = homeDataList.ToArray();
                homeDataObject.isMcTrans = (new List<int>()).ToArray();
                homeDataObject.statusNo = statusNo;
                homeDataObject.mcTransNo = 0;

                return homeDataObject;
            } else {
                sql = $"select count=count(*) From Device_Status " +
                    $"{(DevSN != null || EventTime[0] != "undefined" || Index[0] != "undefined" || Index[1] != "undefined" ? " where " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != "undefined" ? ((DevSN != null ? "and " : "") + "EventTime>='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != "undefined" ? (" and EventTime <='" + EventTime[1] + "'") : "")}" +
                    $"{(Index[0] != "undefined" ? ((DevSN != null || EventTime[0] != "undefined" ? "and " : "") + "C_INDEX >= '" + Index[0] + "'") : "")}" +
                    $"{(Index[1] != "undefined" ? ((DevSN != null || EventTime[0] != "undefined" || Index[0] != "undefined" ? "and " : "") + "C_INDEX <= '" + Index[1] + "'") : "")}";
                int count = int.Parse(DbUtil.ExecuteQuery(sql).Rows[0]["count"].ToString());
                if (count % pageSize != 0)
                    statusNo = (int)Math.Floor((double)count / pageSize);
                else
                    statusNo = count / pageSize - 1;

                sql = $"select no= Identity(int,1,1),* Into #status From Device_Status " +
                    $"{((DevSN != null || Index[0] != "undefined" || Index[1] != "undefined" || EventTime[0] != "undefined" || EventTime[1] != "undefined") ? " where " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != "undefined" ? ((DevSN != null ? "and " : "") + "EventTime>='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != "undefined" ? (" and EventTime <='" + EventTime[1] + "'") : "")}" +
                    $"{(Index[0] != "undefined" ? ((DevSN != null || EventTime[0] != "undefined" ? "and " : "") + "C_INDEX >= '" + Index[0] + "'") : "")}" +
                    $"{(Index[1] != "undefined" ? ((DevSN != null || EventTime[0] != "undefined" || Index[0] != "undefined" ? "and " : "") + "C_INDEX <= '" + Index[1] + "'") : "")}" +
                    "order by EventTime DESC " +
                    $"select top {pageSize} * from #status where no>{statusNo * pageSize}" +
                    " Drop Table #status ";
                statusData = DbUtil.ExecuteQuery(sql);

                foreach (DataRow row in statusData.Rows) {
                    map = new Dictionary<string, object>();
                    foreach (DataColumn column in statusData.Columns) {
                        map.Add(column.ToString(), row[column]);
                    }
                    homeDataList.Add(map);
                }

                homeDataObject.homeData = homeDataList.ToArray();
                homeDataObject.isMcTrans = (new List<int>()).ToArray();
                homeDataObject.statusNo = statusNo;
                homeDataObject.mcTransNo = 0;

                return homeDataObject;
            }
        }

        public static ActionResult<HomeDataObject> homeAlarmOrLogin(string type, int statusNo, string DevSN, string[] Index, string[] EventTime, string dataType, int pageSize) {
            string sql;
            string table = (dataType == "Alarm" ? "TABLE_Alarm" : "Device_Login");
            string time = (dataType == "Alarm" ? "EVENT_TIME" : "LoginTime");
            List<Dictionary<string, Object>> homeDataList = new List<Dictionary<string, object>>();
            // 最终要返回的数据
            HomeDataObject homeDataObject = new HomeDataObject();

            if (type != "last") {
                switch (type) {
                    case "first":
                        statusNo = 0;
                        break;
                    case "left":
                        statusNo -= 1;
                        break;
                    default:
                        statusNo += 1;
                        break;
                }
                homeDataList = select(statusNo, DevSN, Index, EventTime, pageSize, table, time, dataType);
            } else {
                sql = $"select count=count(*) From {table} " +
                    $"{(DevSN != null || EventTime[0] != "undefined" || Index[0] != "undefined" || Index[1] != "undefined" ? " where " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != "undefined" ? ((DevSN != null ? "and " : "") + "EventTime>='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != "undefined" ? (" and EventTime <='" + EventTime[1] + "'") : "")}" +
                    $"{(Index[0] != "undefined" ? ((DevSN != null || EventTime[0] != "undefined" ? "and " : "") + "C_INDEX >= '" + Index[0] + "'") : "")}" +
                    $"{(Index[1] != "undefined" ? ((DevSN != null || EventTime[0] != "undefined" || Index[0] != "undefined" ? "and " : "") + "C_INDEX <= '" + Index[1] + "'") : "")}";
                int count = int.Parse(DbUtil.ExecuteQuery(sql).Rows[0]["count"].ToString());
                if (count % pageSize != 0)
                    statusNo = (int)Math.Floor((double)count / pageSize);
                else
                    statusNo = count / pageSize - 1;
                homeDataList = select(statusNo, DevSN, Index, EventTime, pageSize, table, time, dataType);
            }


            homeDataObject.homeData = homeDataList.ToArray();
            homeDataObject.isMcTrans = (new List<int>()).ToArray();
            homeDataObject.statusNo = statusNo;
            homeDataObject.mcTransNo = 0;

            return homeDataObject;
        }

        private static List<Dictionary<string, Object>> select(int statusNo, string DevSN, string[] Index, string[] EventTime, int pageSize, string table, string time, string dataType) {
            // 此方法服务于AlarmOrLogin方法
            string sql;
            Dictionary<string, Object> map;
            // 从alarm/login表中检索处的table
            DataTable alarmOrLoginData = new DataTable();
            DataTable specificStatusData = new DataTable();
            List<Dictionary<string, Object>> alarmOrLoginDataList = new List<Dictionary<string, object>>();
            List<Dictionary<string, Object>> homeDataList = new List<Dictionary<string, object>>();

            sql = $"select no= Identity(int,1,1),* Into #table From {table} " +
                    $"{((DevSN != null || Index[0] != "undefined" || Index[1] != "undefined" || EventTime[0] != "undefined" || EventTime[1] != "undefined") ? " where " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != "undefined" ? ((DevSN != null ? "and " : "") + time + ">='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != "undefined" ? (" and " + time + "<='" + EventTime[1] + "'") : "")}" +
                    $"{(Index[0] != "undefined" ? ((DevSN != null || EventTime[0] != "undefined" ? "and " : "") + "C_INDEX >= '" + Index[0] + "'") : "")}" +
                    $"{(Index[1] != "undefined" ? ((DevSN != null || EventTime[0] != "undefined" || Index[0] != "undefined" ? "and " : "") + "C_INDEX <= '" + Index[1] + "'") : "")}" +
                    $"order by {time} DESC " +
                    $"Select * From #table where no>{statusNo * pageSize} and no <= {(statusNo + 1) * pageSize}" +
                    " Drop Table #table ";
            // 从alarm/login 表中检索数据
            alarmOrLoginData = DbUtil.ExecuteQuery(sql);

            // 将从alarm/login表中获取的数据由 dataTable -> list
            foreach (DataRow row in alarmOrLoginData.Rows) {
                map = new Dictionary<string, object>();
                foreach (DataColumn column in alarmOrLoginData.Columns) {
                    map.Add(column.ToString(), row[column]);
                }
                alarmOrLoginDataList.Add(map);
            }

            // foreach每一个从alarm/login表中检索到的row
            foreach (Dictionary<string, Object> row in alarmOrLoginDataList) {
                sql = $"select * from Device_Status where DevSN = '{row["DevSN"]}' and EventTime = '{(dataType == "Alarm" ? row["EVENT_TIME"] : ((DateTime)row["LoginTime"]).ToString("yyyy-MM-dd HH:mm:ss"))}'";
                // 从 Device_Status 中找到的符合当前 alarm/login 的 status 数据
                specificStatusData = DbUtil.ExecuteQuery(sql);


                map = new Dictionary<string, object>();
                foreach (DataColumn column in specificStatusData.Columns)
                    map.Add(column.ToString(), (specificStatusData.Rows)[0][column]);
                map.Add((dataType == "Alarm" ? "alarm" : "login"), row);
                homeDataList.Add(map);
            }

            return homeDataList;
        }

        public static ActionResult<HomeDataObject> homeMcTrans(string type, int mcTransNo, string DevSN, string[] Index, string[] EventTime, string[] dataType, int pageSize) {
            string sql;
            DataTable mcTransData = new DataTable();
            List<Dictionary<string, Object>> homeData = new List<Dictionary<string, object>>();
            Dictionary<string, Object> map;
            HomeDataObject homeDataObject = new HomeDataObject();

            if (type != "last") {
                switch (type) {
                    case "first":
                        mcTransNo = 0;
                        break;
                    case "left":
                        mcTransNo -= 1;
                        break;
                    default:
                        mcTransNo += 1;
                        break;
                }
                sql = $"select no= Identity(int,1,1),* Into #mctrans From McTrans " +
                    $"{(DevSN != null || EventTime[0] != "undefined" || Index[0] != "undefined" || Index[1] != "undefined" || dataType[1] != "undefined" ? " where " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != "undefined" ? ((DevSN != null ? "and " : "") + "EventTime>='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != "undefined" ? (" and EventTime <='" + EventTime[1] + "'") : "")}" +
                    $"{(Index[0] != "undefined" ? ((DevSN != null || EventTime[0] != "undefined" ? "and " : "") + "C_INDEX >= '" + Index[0] + "'") : "")}" +
                    $"{(Index[1] != "undefined" ? ((DevSN != null || EventTime[0] != "undefined" || Index[0] != "undefined" ? "and " : "") + "C_INDEX <= '" + Index[1] + "'") : "")}" +
                    $"{(dataType[1] != "undefined" ? ((DevSN != null || EventTime[0] != "undefined" || Index[0] != "undefined" || Index[1] != "undefined" ? "and " : "") + (dataType[1] == "other" ? "EventCode!=128 and EventCode!=129 " : ("EventCode='" + dataType[1] + "'"))) : "")}" +
                    "order by EventTime DESC " +
                    $"Select * From #mctrans where no>{mcTransNo * pageSize} and no <= {pageSize * (mcTransNo + 1)}" +
                    " Drop Table #mctrans ";
                mcTransData = DbUtil.ExecuteQuery(sql);

                foreach (DataRow row in mcTransData.Rows) {
                    map = new Dictionary<string, object>();
                    foreach (DataColumn column in mcTransData.Columns) {
                        map.Add(column.ToString(), row[column]);
                    }
                    homeData.Add(map);
                }
            } else {
                sql = $"select count=count(*) From McTrans " +
                    $"{(DevSN != null || EventTime[0] != "undefined" || Index[0] != "undefined" || Index[1] != "undefined" || dataType[1] != "undefined" ? " where " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != "undefined" ? ((DevSN != null ? "and " : "") + "EventTime>='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != "undefined" ? (" and EventTime <='" + EventTime[1] + "'") : "")}" +
                    $"{(Index[0] != "undefined" ? ((DevSN != null || EventTime[0] != "undefined" ? "and " : "") + "C_INDEX >= '" + Index[0] + "'") : "")}" +
                    $"{(Index[1] != "undefined" ? ((DevSN != null || EventTime[0] != "undefined" || Index[0] != "undefined" ? "and " : "") + "C_INDEX <= '" + Index[1] + "'") : "")}" +
                    $"{(dataType[1] != "undefined" ? ((DevSN != null || EventTime[0] != "undefined" || Index[0] != "undefined" || Index[1] != "undefined" ? "and " : "") + (dataType[1] == "other" ? "EventCode!=128 and EventCode!=129 " : ("EventCode='" + dataType[1] + "'"))) : "")}";

                int count = int.Parse(DbUtil.ExecuteQuery(sql).Rows[0]["count"].ToString());
                if (count % pageSize != 0)
                    mcTransNo = (int)Math.Floor((double)count / pageSize);
                else
                    mcTransNo = count / pageSize - 1;

                sql = $"select no= Identity(int,1,1),* Into #mcTrans From McTrans " +
                    $"{((DevSN != null || Index[0] != "undefined" || Index[1] != "undefined" || EventTime[0] != "undefined" || EventTime[1] != "undefined") ? " where " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != "undefined" ? ((DevSN != null ? "and " : "") + "EventTime>='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != "undefined" ? (" and EventTime <='" + EventTime[1] + "'") : "")}" +
                    $"{(Index[0] != "undefined" ? ((DevSN != null || EventTime[0] != "undefined" ? "and " : "") + "C_INDEX >= '" + Index[0] + "'") : "")}" +
                    $"{(Index[1] != "undefined" ? ((DevSN != null || EventTime[0] != "undefined" || Index[0] != "undefined" ? "and " : "") + "C_INDEX <= '" + Index[1] + "'") : "")}" +
                    $"{(dataType[1] != "undefined" ? ((DevSN != null || EventTime[0] != "undefined" || Index[0] != "undefined" || Index[1] != "undefined" ? "and " : "") + (dataType[1] == "other" ? "EventCode!=128 and EventCode!=129 " : ("EventCode='" + dataType[1] + "'"))) : "")}" +
                "order by EventTime DESC " +
                    $"select top {pageSize} * from #mcTrans where no>{mcTransNo * pageSize}" +
                    " Drop Table #mcTrans ";
                mcTransData = DbUtil.ExecuteQuery(sql);

                foreach (DataRow row in mcTransData.Rows) {
                    map = new Dictionary<string, object>();
                    foreach (DataColumn column in mcTransData.Columns) {
                        map.Add(column.ToString(), row[column]);
                    }
                    homeData.Add(map);
                }
            }

            homeDataObject.homeData = homeData.ToArray();
            homeDataObject.isMcTrans = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            homeDataObject.statusNo = 0;
            homeDataObject.mcTransNo = mcTransNo;

            return homeDataObject;
        }

        public static ActionResult<object[]> getDetail(string DevSN) {
            Dictionary<string, object> map;
            List<object> detailDataList = new List<object>();
            DataTable detailData = new DataTable();
            DataTable DevIDData = new DataTable();

            string sql = $"select top 1 * from Device_Status where DevSN='{DevSN}' order by EventTime desc";
            detailData = DbUtil.ExecuteQuery(sql);

            map = new Dictionary<string, object>();
            foreach (DataColumn column in detailData.Columns) {
                map.Add(column.ToString(), detailData.Rows[0][column]);
            }

            detailDataList.Add(map);

            sql = $"select DevID from Device_Lst where DevSN = '{DevSN}'";
            DevIDData = DbUtil.ExecuteQuery(sql);
            map = new Dictionary<string, object>();
            map.Add(DevIDData.Columns[0].ToString(), DevIDData.Rows[0][DevIDData.Columns[0]]);
            detailDataList.Add(map);

            return detailDataList.ToArray();
        }

        public static ActionResult<object[]> getData(string DevSN, string No, int table, int pageSize) {
            int localPageSize = 1;
            string tab = "";
            string sql;
            DataTable data = new DataTable();
            DataTable countTable = new DataTable();
            int count;
            List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();
            Dictionary<string, object> map;
            List<object> getDataObject = new List<object>();

            switch (table) {
                case 1:
                    tab = "Device_Status";
                    break;
                case 2:
                    tab = "McTrans";
                    localPageSize = pageSize;
                    break;
                case 3:
                    tab = "TABLE_Alarm";
                    localPageSize = pageSize;
                    break;
                default:
                    break;
            }

            if (No != "last")
                sql = $"select top {(tab == "Device_Status" ? 1 : localPageSize)} * from {tab} where C_INDEX not in (select top {(tab == "Device_Status" ? (Convert.ToInt32(No) - 1) : localPageSize * (Convert.ToInt32(No) - 1))} C_INDEX from {tab} where DevSN='{DevSN}' order by {(tab == "TABLE_Alarm" ? "EVENT_TIME" : "EventTime")} desc) and DevSN={DevSN} order by {(tab == "TABLE_Alarm" ? "EVENT_TIME" : "EventTime")} desc";
            else
                sql = $"select top {(tab == "Device_Status" ? 1 : localPageSize)} * from {tab} where DevSN='{DevSN}' order by {(tab=="TABLE_Alarm"?"EVENT_TIME":"EventTime")} ";

            data = DbUtil.ExecuteQuery(sql);

            foreach(DataRow row in data.Rows) {
                map = new Dictionary<string, object>();
                foreach(DataColumn column in data.Columns) {
                    map.Add(column.ToString(), row[column]);
                }
                dataList.Add(map);
            }

            sql = $"select count=count(*) from {tab} where DevSN='{DevSN}'";

            countTable = DbUtil.ExecuteQuery(sql);
            count = (int)countTable.Rows[0]["count"];

            getDataObject.Add(dataList.ToArray());
            getDataObject.Add(count);

            return getDataObject.ToArray();
        }

        public static ActionResult<Dictionary<string, object>> getProps(string DevSN, string table) {
            string sql;
            DataTable dataTable;
            Dictionary<string, object> map;
            sql = $"select top 1 * from {table} where DevSN='{DevSN}' {(table=="Device_Status"?"order by EventTime desc":"")}";
            dataTable = DbUtil.ExecuteQuery(sql);

            map = new Dictionary<string, object>();
            foreach(DataColumn column in dataTable.Columns) {
                map.Add(column.ToString(), dataTable.Rows[0][column]);
            }

            return map;
        }

        public static ActionResult<int> alterProps(string alteredProps, string DevSN) {
            string sql;
            sql = $"update Device_Lst set {alteredProps} where DevSN='{DevSN}'";
            return DbUtil.ExecuteUpdate(sql);
        }
    }
}
