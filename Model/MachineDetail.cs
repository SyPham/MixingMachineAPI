using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class MachineDetail
    {
        public string MachineID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public double RPM { get; set; }

        public bool Status { get; set; }
    }
}
