using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AirWebServer.utils;
using System.Data;

namespace AirWebServer.Controllers{

    [Route("api/[controller]")]
    [ApiController]
    public class PropsController : ControllerBase{
        [HttpGet]
        public ActionResult<DataTable> Get(string DevSN, int type, int page) {
            switch (type) {
                case 0: return HandleClientReq.getList(DevSN, page, "Device_Lst");    
                default: return NotFound();
            }
        }
    }
}
