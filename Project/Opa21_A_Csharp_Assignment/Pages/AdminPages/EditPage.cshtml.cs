using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Opa21_A_Csharp_Assignment.Model;
using Opa21_A_Csharp_Assignment.Pages.Utility;

namespace Opa21_A_Csharp_Assignment.Pages.AdminPages
{
    [Authorize(Roles = StaticDetail.AdminUser)]
    public class EditPageModel : PageModel
    {
        private DatabaseContext databaseContext;

        public EditPageModel(DatabaseContext _databaseContext)
        {
            this.databaseContext = _databaseContext;
        }

 
        [BindProperty]
        public Course course { get; set; }

        public async Task OnGet(int id)
        {
            course = await databaseContext.Course.FindAsync(id);
        }
        
        public async Task<IActionResult> OnPostEditCourse()
        {
            if (ModelState.IsValid)
            {
                databaseContext.Course.Update(course);
                await databaseContext.SaveChangesAsync();
                return RedirectToPage("/AdminPages/EditCourses");
            }
            return Page();
        }
    }
}
