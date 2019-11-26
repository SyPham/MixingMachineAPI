using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class Rawdata
    {
        public int id { get; set; }

        public string machineID { get; set; }
        public int RPM { get; set; }
        public DateTime createddatetime { get; set; }
        public int duration { get; set; }

        public int sequence { get; set; }
    }
}
