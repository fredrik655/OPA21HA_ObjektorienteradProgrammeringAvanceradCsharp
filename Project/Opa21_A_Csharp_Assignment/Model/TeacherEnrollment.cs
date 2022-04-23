using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Opa21_A_Csharp_Assignment.Model
{
    public class TeacherEnrollment
    {
        [Key]
        public int TeacherEnrollmentId { get; set; }
        public Teacher Teacher { get; set; }
        public Course Course { get; set; }
    }
}
