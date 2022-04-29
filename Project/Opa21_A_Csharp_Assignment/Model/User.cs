using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Opa21_A_Csharp_Assignment.Model
{
    public class User : IdentityUser
    {
        public int StudentId { get; set; }
        public int TeacherId { get; set; }
        public bool Admin { get; set; }
    }
}
