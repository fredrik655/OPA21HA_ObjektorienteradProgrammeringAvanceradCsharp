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
    public class AddPageModel : PageModel
    {

        private DatabaseContext databaseContext;

        public AddPageModel(DatabaseContext _databaseContext)
        {
            this.databaseContext = _databaseContext;
        }

        [BindProperty]
        public Course Course { get; set; }

        public async Task OnGet(string? type)
        {
           
        }

        public async Task<IActionResult> OnPostAddCourse()
        {
            if (ModelState.IsValid)
            {
                await databaseContext.AddAsync(Course);
                await databaseContext.SaveChangesAsync();
                return RedirectToPage("/AdminPages/EditCourses");
            }
            return Page();
        }
    }
}
