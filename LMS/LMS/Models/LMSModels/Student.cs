using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Student
    {
        public Student()
        {
            Enrolled = new HashSet<Enrolled>();
            Submission = new HashSet<Submission>();
        }

        public string UId { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public DateTime Dob { get; set; }
        public string Pwd { get; set; }
        public string Subject { get; set; }

        public Department SubjectNavigation { get; set; }
        public ICollection<Enrolled> Enrolled { get; set; }
        public ICollection<Submission> Submission { get; set; }
    }
}
