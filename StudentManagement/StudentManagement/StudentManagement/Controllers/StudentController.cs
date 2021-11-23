using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagement.DBContexts;
using StudentManagement.Entities;
using StudentManagement.Models;
using StudentManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Controllers
{
    public class StudentController : Controller
    {
        private readonly IUserService userService;
        private readonly UserManager<User> userManager;
        private readonly StudentManagementContext context;
        private static User user = new User();

        public StudentController(IUserService userService,
                                 UserManager<User> userManager,
                                 StudentManagementContext context)
        {
            this.userService = userService;
            this.userManager = userManager;
            this.context = context;
        }
        public IActionResult Index()
        {

            var appUsers = userService.Gets();
            var users = new List<Student>();
            foreach (var u in appUsers)
            {

                var user = new Student()
                {
                    UserId = u.Id,
                    Events = u.Events,
                    Skills = null,
                    UserSchoolYears = u.UserSchoolYears,
                    Username = u.UserName,
                    StudentCode = u.StudentCode,
                    Roles = string.Join(',', Task.Run(async () => await userManager.GetRolesAsync(u)).Result.ToArray())
                };
                users.Add(user);
            }
            ViewData["ListEventId"] = new SelectList(context.ListEvents, "ListEventId", "ListEventName");
            ViewData["SchoolYearId"] = new SelectList(context.SchoolYears, "SchoolYearId", "SchoolYearName");
            return View(users);
        }
    }
}
