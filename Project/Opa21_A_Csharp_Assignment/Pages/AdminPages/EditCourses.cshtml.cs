using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Opa21_A_Csharp_Assignment.Model;
using Opa21_A_Csharp_Assignment.Pages.Utility;

namespace Opa21_A_Csharp_Assignment.Pages.AdminPages
{
    [Authorize(Roles = StaticDetail.AdminUser)]
    public class EditCoursesModel : PageModel
    {
        private DatabaseContext databaseContext;
        public EditCoursesModel(DatabaseContext _databaseContext)
        {
            this.databaseContext = _databaseContext;
        }

        public List<Course> courseList { get; set; }
        public List<Enrollment> enrollmentList { get; set; }
        public List<TeacherEnrollment> teacherEnrollmentList { get; set; }
        public async Task OnGet()
        {
            courseList = await databaseContext.Course.ToListAsync();
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            Course course = await databaseContext.Course.FindAsync(id);

            if(course == null)
            {
                return NotFound();
            }
            enrollmentList = await databaseContext.Enrollment.Where(x => x.Course.CourseId == id).ToListAsync();
            foreach (var enrollment in enrollmentList)
            {
                databaseContext.Enrollment.Remove(enrollment);
                await databaseContext.SaveChangesAsync();
            }
            teacherEnrollmentList = await databaseContext.TeacherEnrollment.Where(x => x.Course.CourseId == id).ToListAsync();
            foreach (var teacherEnrollment in teacherEnrollmentList)
            {
                databaseContext.TeacherEnrollment.Remove(teacherEnrollment);
                await databaseContext.SaveChangesAsync();
            }
            databaseContext.Course.Remove(course);
            await databaseContext.SaveChangesAsync();

            return RedirectToPage("/AdminPages/EditCourses");
        }
    }
}
