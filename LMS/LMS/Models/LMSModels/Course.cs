using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Course
    {
        public Course()
        {
            Class = new HashSet<Class>();
        }

        public int CourseId { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public string Subject { get; set; }

        public Department SubjectNavigation { get; set; }
        public ICollection<Class> Class { get; set; }
    }
}
