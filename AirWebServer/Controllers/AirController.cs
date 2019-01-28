using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AirWebServer.utils;
using System.Data;
using AirWebServer.Model;
using Newtonsoft.Json.Linq;

namespace AirWebServer.Controllers {

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PropsController : ControllerBase {
        [HttpGet]
        // GET api/Props/getList
        public ActionResult<DataTable> getList(string DevSN, int page) {
            // Devices.js页面调用
            return HandleClientReq.getList(DevSN, page, "Device_Lst"); 
        }

        [HttpGet]
        // GET api/Props/getProps
        public ActionResult<Dictionary<string, object>> getProps(string DevSN) {
            // parms tag调用
            return HandleClientReq.getProps(DevSN, "Device_Lst");
        }

        [HttpGet]
        // GET api/Props/alterProps
        public ActionResult<int> alterProps(string alteredProps, string DevSN) {
            return HandleClientReq.alterProps(alteredProps,DevSN);
        }
    }

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class homeController : ControllerBase {
        [HttpGet]
        // api/home/All
        public ActionResult<HomeDataObject> All(string type, int statusNo, int mcTransNo, int mcTransNum, int statusNum, string DevSN, string[] EventTime) {
            int pageSize = 8;
            return HandleClientReq.homeAll(type, statusNo, mcTransNo, mcTransNum, statusNum, DevSN, EventTime, pageSize);
        }

        [HttpGet]
        // api/home/Status
        public ActionResult<HomeDataObject> Status(string type, int statusNo, string DevSN, string[] Index, string[] EventTime, string dataType) {
            int pageSize = 8;
            return HandleClientReq.homeStatus(type, statusNo, DevSN, Index, EventTime, pageSize);
        }

        [HttpGet]
        // api/home/AlarmOrLogin
        public ActionResult<HomeDataObject> AlarmOrLogin(string type, int statusNo, string DevSN, string[] Index, string[] EventTime, string dataType) {
            int pageSize = 8;
            return HandleClientReq.homeAlarmOrLogin(type, statusNo, DevSN, Index, EventTime, dataType, pageSize);
        }

        [HttpGet]
        // api/home/McTrans
        public ActionResult<HomeDataObject> McTrans(string type, int mcTransNo, string DevSN, string[] Index, string[] EventTime, string[] dataType) {
            int pageSize = 8;
            return HandleClientReq.homeMcTrans(type, mcTransNo, DevSN, Index, EventTime, dataType, pageSize);
        }
    }

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class tableDataController : ControllerBase {
        [HttpGet]
        // GET api/tableData/getDetail
        public ActionResult<object[]> getDetail(string DevSN) {
            return HandleClientReq.getDetail(DevSN);
        }

        [HttpGet]
        // GET api/tableData/getData
        public ActionResult<object[]> getData(string DevSN, string No, int table, int pageSize) {
            return HandleClientReq.getData(DevSN, No, table, pageSize);
        }
    }
}
