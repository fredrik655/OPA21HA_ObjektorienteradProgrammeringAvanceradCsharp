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
    public class TeacherClassesModel : PageModel
    {
        private DatabaseContext databaseContext;
        private readonly UserManager<IdentityUser> _userManager;

        public TeacherClassesModel(DatabaseContext _databaseContext, UserManager<IdentityUser> userManager)
        {
            this.databaseContext = _databaseContext;
            _userManager = userManager;
        }

        public List<Course> courseList { get; set; }
        public User teacherUser { get; set; }
        public List<Course> notTeachingCourses { get; set; }
        public List<TeacherEnrollment> teacherEnrollments { get; set; }
        public int teacherId { get; set; }
        public Teacher teacher { get; set; }
        public Course course { get; set; }

        public async Task OnGet()
        {
            notTeachingCourses = new List<Course>();
            var id = _userManager.GetUserId(HttpContext.User);
            teacherUser = (User)(await databaseContext.Users.FindAsync(id));
            teacherId = teacherUser.TeacherId;
            courseList = await databaseContext.Course.ToListAsync();
            teacherEnrollments = await databaseContext.TeacherEnrollment.Where(x => x.Teacher.TeacherId == teacherId).ToListAsync();


            bool inList = false;
            for (int i = 0; i < courseList.Count; i++)
            {
                inList = false;

                foreach (var enrolledClass in teacherEnrollments)
                {
                    if (courseList[i].CourseId == enrolledClass.Course.CourseId)
                    {
                        inList = true;
                    }
                }

                if (!inList)
                {
                    notTeachingCourses.Add(courseList[i]);
                }
            }
        }

        public async Task<IActionResult> OnPostTeach(int id)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                teacherUser = (User)(await databaseContext.Users.FindAsync(userId));
                teacherId = teacherUser.TeacherId;
                course = await databaseContext.Course.FindAsync(id);
                teacher = await databaseContext.Teacher.FindAsync(teacherId);
                TeacherEnrollment en = new TeacherEnrollment { Course = course, Teacher = teacher };
                await databaseContext.TeacherEnrollment.AddAsync(en);
                await databaseContext.SaveChangesAsync();
                return RedirectToPage("/TeacherPages/TeacherClasses");
            }
            return Page();
        }
    }
}
