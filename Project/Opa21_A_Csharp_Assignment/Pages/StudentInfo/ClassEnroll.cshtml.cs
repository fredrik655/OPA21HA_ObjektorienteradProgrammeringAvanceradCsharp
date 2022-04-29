using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Opa21_A_Csharp_Assignment.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Opa21_A_Csharp_Assignment.Pages.Utility;

namespace Opa21_A_Csharp_Assignment.Pages.StudentInfo
{
    [Authorize(Roles = StaticDetail.StudentUser)]
    public class ClassEnrollModel : PageModel
    {

        private DatabaseContext databaseContext;
        private readonly UserManager<IdentityUser> _userManager;

        public ClassEnrollModel(DatabaseContext _databaseContext, UserManager<IdentityUser> userManager)
        {
            this.databaseContext = _databaseContext;
            _userManager = userManager;
        }

        public List<Course> courseList { get; set; }
        public User studentUser { get; set; }
        public List<Course> notEnrolledCourses { get; set; }
        public List<Enrollment> enrollments { get; set; }
        public int studentId { get; set; }
        public Student student { get; set; }
        public Course course { get; set; }

        public async Task OnGet()
        {
            notEnrolledCourses = new List<Course>();
            var id = _userManager.GetUserId(HttpContext.User);
            studentUser = (User)(await databaseContext.Users.FindAsync(id));
            studentId = studentUser.StudentId;
            courseList = await databaseContext.Course.ToListAsync();
            enrollments = await databaseContext.Enrollment.Where(x => x.Student.StudentId == studentId).ToListAsync();


            bool inList = false;
            for (int i = 0; i < courseList.Count; i++)
            {
                inList = false;

                foreach (var enrolledClass in enrollments)
                {
                    if (courseList[i].CourseId == enrolledClass.Course.CourseId)
                    {
                        inList = true;
                    }
                }

                if (!inList)
                {
                    notEnrolledCourses.Add(courseList[i]);
                }
            }
        }

        public async Task<IActionResult> OnPostEnroll(int id)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                studentUser = (User)(await databaseContext.Users.FindAsync(userId));
                studentId = studentUser.StudentId;
                course = await databaseContext.Course.FindAsync(id);
                student = await databaseContext.Student.FindAsync(studentId);
                Enrollment en = new Enrollment { Course = course, Student = student };
                await databaseContext.Enrollment.AddAsync(en);
                await databaseContext.SaveChangesAsync();
               return RedirectToPage("/StudentInfo/StudentPage");
            }
            return Page();
        }


    }
}
