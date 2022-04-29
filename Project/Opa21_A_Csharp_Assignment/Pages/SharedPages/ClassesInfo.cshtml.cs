using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Opa21_A_Csharp_Assignment.Model;

namespace Opa21_A_Csharp_Assignment.Pages.SharedPages
{
    public class ClassesInfoModel : PageModel
    {

        private DatabaseContext databaseContext;
        public ClassesInfoModel(DatabaseContext _databaseContext)
        {
            this.databaseContext = _databaseContext;
        }

        public List<Course> courseList { get; set; }
        public async Task OnGet()
        {
            courseList = await databaseContext.Course.ToListAsync();
        }
    }
}
