using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Class
    {
        public Class()
        {
            AsgnCategory = new HashSet<AsgnCategory>();
            Enrolled = new HashSet<Enrolled>();
        }

        public int ClassId { get; set; }
        public string Season { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public string Location { get; set; }
        public int CourseId { get; set; }
        public string UId { get; set; }

        public int Year { get; set; }

        public Course Course { get; set; }
        public Professor U { get; set; }
        public ICollection<AsgnCategory> AsgnCategory { get; set; }
        public ICollection<Enrolled> Enrolled { get; set; }
    }
}
