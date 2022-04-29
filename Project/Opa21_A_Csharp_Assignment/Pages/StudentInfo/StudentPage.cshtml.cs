using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Opa21_A_Csharp_Assignment.Model;
using Opa21_A_Csharp_Assignment.Pages.Utility;

namespace Opa21_A_Csharp_Assignment.Pages.StudentInfo
{
    [Authorize(Roles = StaticDetail.StudentUser)]
    public class StudentPageModel : PageModel
    {

        private DatabaseContext databaseContext;
        private readonly UserManager<IdentityUser> _userManager;
        public StudentPageModel(DatabaseContext _databaseContext, UserManager<IdentityUser> userManager)
        {
            this.databaseContext = _databaseContext;
            _userManager = userManager;
        }

        public List<Course> courseList { get; set; }
        public List<Enrollment> enrollments { get; set; }
        public List<Student> studentList { get; set; }
        public List<Course> studentCourseList { get; set; }
        public User studentUser { get; set; }
        public int studentId { get; set; }
        public Student student { get; set; }
        public Course course { get; set; }
        public async Task OnGet()
        {
            var id = _userManager.GetUserId(HttpContext.User);
            studentUser = (User)(await databaseContext.Users.FindAsync(id));
            studentId = studentUser.StudentId;
            studentCourseList = new List<Course>();
            studentList = await databaseContext.Student.ToListAsync();
            courseList = await databaseContext.Course.ToListAsync();
            enrollments = await databaseContext.Enrollment.Where(x => x.Student.StudentId == studentId).ToListAsync();
            foreach (var course in enrollments)
            {
                studentCourseList.Add(course.Course);
            }
        }

        public async Task<IActionResult> OnPostQuit(int id)
        {
            if (ModelState.IsValid)
            {
                courseList = await databaseContext.Course.ToListAsync();
                studentList = await databaseContext.Student.ToListAsync();
                var userId = _userManager.GetUserId(HttpContext.User);
                studentUser = (User)(await databaseContext.Users.FindAsync(userId));
                studentId = studentUser.StudentId;
                List<Enrollment> allEnrollments = await databaseContext.Enrollment.ToListAsync();
                Enrollment enrollmentItem = null;
                foreach (var enrollment in allEnrollments)
                {
                    if(enrollment.Student.StudentId == studentId && enrollment.Course.CourseId == id)
                    {
                        enrollmentItem = enrollment;
                    }
                }
                if(enrollmentItem != null)
                {
                    databaseContext.Enrollment.Remove(enrollmentItem);
                    await databaseContext.SaveChangesAsync();
                }
                return RedirectToPage("/StudentInfo/StudentPage");
            }
            return Page();
        }
    }
}
