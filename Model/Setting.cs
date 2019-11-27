using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Model
{
    [Table("setting")]
    public class Setting
    {
        [Key]
        public string id { get; set; }
        public int standardRPM { get; set; }
        public int minRPM { get; set; }
        public int startBuzzerAfter { get; set; }
        public int stopwatch { get; set; }
        public int timer { get; set; }
    }
    
}
