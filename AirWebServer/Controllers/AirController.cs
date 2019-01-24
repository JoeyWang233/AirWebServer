using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AirWebServer.utils;
using System.Data;
using AirWebServer.Model;

namespace AirWebServer.Controllers{

    [Route("api/[controller]")]
    [ApiController]
    public class PropsController : ControllerBase{
        [HttpGet]
        // GET api/Props
        public ActionResult<DataTable> Get(string DevSN, int type, int page) {
            switch (type) {
                case 0: return HandleClientReq.getList(DevSN, page, "Device_Lst");    
                default: return NotFound();
            }
        }
    }

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class homeController : ControllerBase {
        [HttpGet]
        // api/home/homeAll
        public ActionResult<HomeDataObject> homeAll(string type, int statusNo, int mcTransNo, int mcTransNum, int statusNum, string DevSN, string[] EventTime) {
            int pageSize = 8;
            return HandleClientReq.homeAll(type, statusNo,mcTransNo,mcTransNum, statusNum,DevSN, EventTime,pageSize);
        }
    }
}
