using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Department
    {
        public Department()
        {
            Course = new HashSet<Course>();
            Professor = new HashSet<Professor>();
            Student = new HashSet<Student>();
        }

        public string Name { get; set; }
        public string Subject { get; set; }

        public ICollection<Course> Course { get; set; }
        public ICollection<Professor> Professor { get; set; }
        public ICollection<Student> Student { get; set; }
    }
}
