﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinistryPlatform.Models
{
    public class Attribute
    {
        public int dp_RecordID { get; set; }
        public string dp_RecordName { get; set; }
        public int dp_Selected { get; set; }
        public int? dp_FileID { get; set; }
        public int dp_RecordStatus { get; set; }
        public string Attribute_Type { get; set; }
        public string Attribute_Category { get; set; }
        public string Attribute_Name { get; set; }
    }
}
