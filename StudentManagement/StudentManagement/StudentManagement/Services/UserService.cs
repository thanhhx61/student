using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.DBContexts;
using StudentManagement.Entities;
using StudentManagement.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly StudentManagementContext context;

        public UserService(UserManager<User> userManager,
                            SignInManager<User> signInManager,
                            RoleManager<IdentityRole> roleManager,
                            StudentManagementContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.context = context;
        }

        public User Get(string userId)
        {
            return context.Users.FirstOrDefault(c => c.Id == userId);
        }

        public List<User> Gets()
        {
            return context.Users.Include(p => p.Events).Include(p => p.UserSchoolYears).ToList();
        }

        [HttpGet]
        public List<User> GetUsers()
        {
            var user = context.Users.ToList();
            return user;
        }
        public async Task<LoginResult> Login(Login LoginUser)
        {
            var user = await userManager.FindByNameAsync(LoginUser.Username);
            if (user == null)
            {
                return new LoginResult()
                {
                    UserId = string.Empty,
                    Username = string.Empty,
                    Message = "ユーザーネーム và パスワード!"
                };
            }
            var signInResult = await signInManager.PasswordSignInAsync(user, LoginUser.Password, LoginUser.RememberMe, false);
            if (signInResult.Succeeded)
            {
                var roles = await userManager.GetRolesAsync(user);
                return new LoginResult()
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    Message = "登陆成功！",
                    Roles = roles.ToArray()
                };
            }
            return new LoginResult()
            {
                UserId = string.Empty,
                Username = string.Empty,
                Message = "エラーが発生しました。 後でもう一度やり直してください!"
            };
        }

        public void Sighout()
        {
            signInManager.SignOutAsync().Wait();
        }


        public async Task<User> GetUserByUserName(string userName)
        {
            return await context.Users.FirstOrDefaultAsync(x => x.UserName == userName);
        }
    }
}
