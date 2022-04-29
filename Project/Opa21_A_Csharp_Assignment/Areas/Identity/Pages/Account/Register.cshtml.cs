using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Opa21_A_Csharp_Assignment.Model;
using Opa21_A_Csharp_Assignment.Pages.Utility;

namespace Opa21_A_Csharp_Assignment.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        //private readonly IEmailSender _emailSender;

        private RoleManager<IdentityRole> _roleManager;
        private DatabaseContext databaseContext;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            //IEmailSender emailSender
            RoleManager<IdentityRole> roleManger,
            DatabaseContext _databaseContext
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            // _emailSender = emailSender;
            this._roleManager = roleManger;
            this.databaseContext = _databaseContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
            public string Name { get; set; }

            [Required]
            public int Age { get; set; }

            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
            public string Title { get; set; }

            [Required]
            public bool Student { get; set; }

            [Required]
            public bool Admin { get; set; }
            [Required]
            public bool Teacher { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                //var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                var user = new User { };
                if (Input.Admin == true)
                {
                    user = new User
                    {
                        UserName = Input.Email,
                        Email = Input.Email,
                        StudentId = -1,
                        TeacherId = -1,
                        Admin = true
                    };
                }
                else if(Input.Student == true)
                {
                    Student student = new Student { Name = Input.Name, Age = Input.Age.ToString() };
                    await databaseContext.Student.AddAsync(student);
                    await databaseContext.SaveChangesAsync();
                    int LastIndex = databaseContext.Student.OrderByDescending(x => x.StudentId).FirstOrDefault().StudentId;
                    user = new User
                    {
                        UserName = Input.Email,
                        Email = Input.Email,
                        StudentId = LastIndex,
                        TeacherId = -1,
                        Admin = false
                    };
                }
                else if (Input.Teacher)
                {
                    Teacher teacher = new Teacher { Name = Input.Name, Age = Input.Age.ToString(), Title = Input.Title };
                    await databaseContext.Teacher.AddAsync(teacher);
                    await databaseContext.SaveChangesAsync();
                    int LastIndex = databaseContext.Teacher.OrderByDescending(x => x.TeacherId).FirstOrDefault().TeacherId;
                    user = new User
                    {
                        UserName = Input.Email,
                        Email = Input.Email,
                        StudentId = -1,
                        TeacherId = LastIndex,
                        Admin = false
                    };
                }
                else
                {
                    user = new User
                    {
                        UserName = Input.Email,
                        Email = Input.Email,
                        StudentId = -1,
                        TeacherId = -1,
                        Admin = false
                    };
                }
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync(StaticDetail.AdminUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetail.AdminUser));
                    }
                    if (!await _roleManager.RoleExistsAsync(StaticDetail.StudentUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetail.StudentUser));
                    }
                    if (!await _roleManager.RoleExistsAsync(StaticDetail.TeacherUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetail.TeacherUser));
                    }

                    if(user.Admin == true)
                    {
                        await _userManager.AddToRoleAsync(user, StaticDetail.AdminUser);
                    }
                    else if(user.TeacherId != -1)
                    {
                        await _userManager.AddToRoleAsync(user, StaticDetail.TeacherUser);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, StaticDetail.StudentUser);
                    }



                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                      //  $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
