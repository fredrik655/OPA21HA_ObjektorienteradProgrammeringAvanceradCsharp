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

namespace Opa21_A_Csharp_Assignment.Pages.TeacherPages
{
    [Authorize(Roles = StaticDetail.TeacherUser)]
    public class TeacherPageModel : PageModel
    {
        private DatabaseContext databaseContext;
        private readonly UserManager<IdentityUser> _userManager;
        public TeacherPageModel(DatabaseContext _databaseContext, UserManager<IdentityUser> userManager)
        {
            this.databaseContext = _databaseContext;
            _userManager = userManager;
        }

        public List<Course> courseList { get; set; }
        public List<TeacherEnrollment> teacherEnrollments { get; set; }
        public List<Student> studentList { get; set; }
        public List<Teacher> teacherList { get; set; }
        public List<Course> teacherCourseList { get; set; }
        public User teacherUser { get; set; }
        public int teacherId { get; set; }
        public Teacher teacher { get; set; }
        public Course course { get; set; }
        public async Task OnGet()
        {
            var id = _userManager.GetUserId(HttpContext.User);
            teacherUser = (User)(await databaseContext.Users.FindAsync(id));
            teacherId = teacherUser.TeacherId;
            teacherCourseList = new List<Course>();
            //studentList = await databaseContext.Teacher.ToListAsync();
            courseList = await databaseContext.Course.ToListAsync();
            teacherEnrollments = await databaseContext.TeacherEnrollment.Where(x => x.Teacher.TeacherId == teacherId).ToListAsync();
            foreach (var course in teacherEnrollments)
            {
                teacherCourseList.Add(course.Course);
            }
        }

        public async Task<IActionResult> OnPostQuit(int id)
        {
            if (ModelState.IsValid)
            {
                courseList = await databaseContext.Course.ToListAsync();
                studentList = await databaseContext.Student.ToListAsync();
                teacherList = await databaseContext.Teacher.ToListAsync();
                var userId = _userManager.GetUserId(HttpContext.User);
                teacherUser = (User)(await databaseContext.Users.FindAsync(userId));
                teacherId = teacherUser.TeacherId;
                List<TeacherEnrollment> allEnrollments = await databaseContext.TeacherEnrollment.ToListAsync();
                TeacherEnrollment enrollmentItem = null;
                foreach (var enrollment in allEnrollments)
                {
                    if (enrollment.Teacher.TeacherId == teacherId && enrollment.Course.CourseId == id)
                    {
                        enrollmentItem = enrollment;
                    }
                }
                if (enrollmentItem != null)
                {
                    databaseContext.TeacherEnrollment.Remove(enrollmentItem);
                    await databaseContext.SaveChangesAsync();
                }
                return RedirectToPage("/TeacherPages/TeacherPage");
            }
            return Page();
        }
    }
}
