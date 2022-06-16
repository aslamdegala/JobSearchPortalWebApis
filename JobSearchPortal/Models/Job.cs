using System;
using System.Collections.Generic;

#nullable disable

namespace JobSearchPortal.Models
{
    public partial class Job
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public string JobDescription { get; set; }
        public string JobLocation { get; set; }
        public string JobType { get; set; }
        public int Experience { get; set; }
        public int Salary { get; set; }
        public string Skill { get; set; }
        public bool IsDeleted { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}
