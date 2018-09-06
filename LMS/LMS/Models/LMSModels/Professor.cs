using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Professor
    {
        public Professor()
        {
            Class = new HashSet<Class>();
        }

        public string UId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Dob { get; set; }
        public string Password { get; set; }
        public string Subject { get; set; }

        public Department SubjectNavigation { get; set; }
        public ICollection<Class> Class { get; set; }
    }
}
