using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Enrolled
    {
        public string UId { get; set; }
        public int ClaId { get; set; }
        public int Grade { get; set; }

        public Class Cla { get; set; }
        public Student U { get; set; }
    }
}
