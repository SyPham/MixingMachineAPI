using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class MachineDetail
    {
        public string MachineID { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public double RPM { get; set; }

        public bool Status { get; set; }
    }
}
