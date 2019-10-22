using System;
using System.Collections.Generic;
using System.Text;

namespace General.Entities
{
    public class OrderDetailSearchArgs
    {
        public string q { get; set; }
        public bool? hascheckin { get; set; }
        public bool? hasend { get; set; }
    }
}
