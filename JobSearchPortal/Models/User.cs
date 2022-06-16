using System;
using System.Collections.Generic;

#nullable disable

namespace JobSearchPortal.Models
{
    public partial class User
    {
        public User()
        {
            Jobs = new HashSet<Job>();
        }

        public int UserId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public virtual ICollection<Job> Jobs { get; set; }
    }
}
