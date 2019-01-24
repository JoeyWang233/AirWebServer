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
                    $"{(DevSN != null || EventTime[0] != null ? " where " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != null ? ((DevSN != null ? "and " : "") + "EventTime>='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != null ? (" and EventTime <='" + EventTime[1] + "'") : "")}" +
                    "order by EventTime DESC " +
                    $"Select * From #status where no>{statusNo} and no <= {pageSize + statusNo}" +
                    " Drop Table #status ";
                Console.Write(sql);
                statusData = DbUtil.ExecuteQuery(sql);

                //查找McTrans表
                sql = $"select no= Identity(int,1,1),* Into #mctrans From McTrans " +
                    $"{(DevSN != null || EventTime[0] != null ? " where " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != null ? ((DevSN != null ? "and " : "") + "EventTime>='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != null ? (" and EventTime <='" + EventTime[1] + "'") : "")}" +
                    "order by EventTime DESC " +
                    $"Select * From #mctrans where no>{mcTransNo} and no <= {pageSize + mcTransNo}" +
                    " Drop Table #mctrans ";
                mcTransData = DbUtil.ExecuteQuery(sql);

                //对查询结果进行处理


            } else if (type == "last") {
                //查找Device_Status表符合条件的count
                sql = $"select count=count(*) From Device_Status " +
                    $"{(DevSN != null || EventTime[0] != null ? " where " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != null ? ((DevSN != null ? "and " : "") + "EventTime>='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != null ? (" and EventTime <='" + EventTime[1] + "'") : "")}";
                statusNo = int.Parse(DbUtil.ExecuteQuery(sql).Rows[0]["count"].ToString());

                //查找McTrans表符合条件的count
                sql = $"select count=count(*) From McTrans " +
                    $"{(DevSN != null || EventTime[0] != null ? " where " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != null ? ((DevSN != null ? "and " : "") + "EventTime>='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != null ? (" and EventTime <='" + EventTime[1] + "'") : "")}";
                mcTransNo = int.Parse(DbUtil.ExecuteQuery(sql).Rows[0]["count"].ToString());

                int total = statusNo + mcTransNo;
                if (total % pageSize != 0)
                    pageSize = total % pageSize;

                //查找Device_Status表
                sql = $"select top {pageSize} * From Device_Status " +
                    $"{(DevSN != null || EventTime[0] != null ? " where " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != null ? ((DevSN != null ? "and " : "") + "EventTime>='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != null ? (" and EventTime <='" + EventTime[1] + "'") : "")}" +
                    " order by EventTime ";
                statusData = DbUtil.ExecuteQuery(sql);

                //查找McTrans表
                sql = $"select top {pageSize} * From McTrans " +
                    $"{(DevSN != null || EventTime[0] != null ? " where " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != null ? ((DevSN != null ? "and " : "") + "EventTime>='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != null ? (" and EventTime <='" + EventTime[1] + "'") : "")}" +
                    " order by EventTime ";
                mcTransData = DbUtil.ExecuteQuery(sql);

                //对查询结果进行处理

            } else {
                //type=="left"
                //查找Device_Status表
                sql = $"select top {pageSize} * From Device_Status " +
                    $"where EventTime in (select top {statusNo - statusNum} EventTime from Device_Status " +
                    $"{(DevSN != null || EventTime[0] != null ? " where " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != null ? ((DevSN != null ? "and " : "") + "EventTime>='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != null ? (" and EventTime <='" + EventTime[1] + "'") : "")}" +
                    "order by EventTime DESC) " +
                    $"{(DevSN != null || EventTime[0] != null ? " and " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != null ? ((DevSN != null ? "and " : "") + "EventTime>='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != null ? (" and EventTime <='" + EventTime[1] + "'") : "")}" +
                    " order by EventTime ";
                statusData = DbUtil.ExecuteQuery(sql);

                //查找McTrans表
                sql = $"select top {pageSize} * From McTrans " +
                    $"where EventTime in (select top {mcTransNo - mcTransNum} EventTime from McTrans " +
                    $"{(DevSN != null || EventTime[0] != null ? " where " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != null ? ((DevSN != null ? "and " : "") + "EventTime>='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != null ? (" and EventTime <='" + EventTime[1] + "'") : "")}" +
                    "order by EventTime DESC) " +
                    $"{(DevSN != null || EventTime[0] != null ? " and " : "")}" +
                    $"{(DevSN != null ? ("DevSN ='" + DevSN + "' ") : "")}" +
                    $"{(EventTime[0] != null ? ((DevSN != null ? "and " : "") + "EventTime>='" + EventTime[0] + "'") : "")}" +
                    $"{(EventTime[1] != null ? (" and EventTime <='" + EventTime[1] + "'") : "")}" +
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

            if (statusData != null) { 
                //测试：将每一行数据做成键值对，存入到homeDataList1中
                foreach(DataRow row in statusData.Rows) {
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
                for(int i = 0; i < homeDataList.Count(); i++) {
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
    }
}
