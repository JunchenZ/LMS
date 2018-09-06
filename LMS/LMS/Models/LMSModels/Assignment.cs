using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Assignment
    {
        public Assignment()
        {
            Submission = new HashSet<Submission>();
        }

        public int AId { get; set; }
        public string Name { get; set; }
        public string Contents { get; set; }
        public DateTime Due { get; set; }
        public bool SType { get; set; }
        public int Points { get; set; }
        public int CatId { get; set; }

        public AsgnCategory Cat { get; set; }
        public ICollection<Submission> Submission { get; set; }
    }
}
