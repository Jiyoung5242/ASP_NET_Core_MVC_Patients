using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JJPatients.Models
{
    public class UserRole
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public bool IsSelected { get; set; }
        public string UserEmail { get; set; }
    }
}
