using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AirWebServer.Model {
    public class HomeDataObject {
        public Dictionary<string, Object>[] homeData;
        public int[] isMcTrans;
        public int statusNo;
        public int mcTransNo;
    }
}
