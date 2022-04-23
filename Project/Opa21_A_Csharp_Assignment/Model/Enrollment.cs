using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Opa21_A_Csharp_Assignment.Model
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }
        public Student Student { get; set; }
        public Course Course { get; set; }
    }
}
