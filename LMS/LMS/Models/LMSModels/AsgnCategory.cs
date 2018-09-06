using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class AsgnCategory
    {
        public AsgnCategory()
        {
            Assignment = new HashSet<Assignment>();
        }

        public int CatId { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }
        public int ClaId { get; set; }

        public Class Cla { get; set; }
        public ICollection<Assignment> Assignment { get; set; }
    }
}
