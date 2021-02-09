using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admins")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;


        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }


        public async Task<IActionResult> List()
        {
            var roles = _roleManager.Roles.ToList();
            IEnumerable<IdentityUser> users;
            var userNames = new List<string>();
            var result = new Dictionary<string, IEnumerable<string>>();

            foreach(var role in roles)
            {
                users = await _userManager.GetUsersInRoleAsync(role.Name);

                foreach (var user in users)
                {
                    userNames.Add(user.UserName);
                }

                result.Add(role.Name, new List<string>(userNames));
                userNames.Clear();
            }

            return View(result);
        }

        [HttpPost]
        public async Task<RedirectToActionResult> DeleteRole(string roleName)
        {
            if (roleName == "admins")
            {
                return RedirectToActionWithMessage("Нельзя удалить роль админа", nameof(List));
            }

            var role = await _roleManager.FindByNameAsync(roleName);

            if (role is null)
            {
                return RedirectToActionWithMessage("Ошибка! роль с таким именем не найдена", nameof(List));
            }

            foreach (var user in await _userManager.GetUsersInRoleAsync(roleName))
            {
                await _userManager.RemoveFromRoleAsync(user, roleName);
            }

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                return RedirectToActionWithMessage($"Роль {roleName} Удалена", nameof(List));
            }
            else
            {
                return RedirectToActionWithMessage(result.Errors.ToString(), nameof(List));
            }
        }

        [HttpPost]
        public async Task<RedirectToActionResult> DeleteUserFromRole(string userName, string roleName)
        {
            IdentityUser user = await _userManager.FindByNameAsync(userName);
             
            if (user is null)
            {
                return RedirectToActionWithMessage($"Пользователь {userName} не найден", nameof(List));
            }

            IdentityRole role = await _roleManager.FindByNameAsync(roleName);

            if (role is null)
            {
                return RedirectToActionWithMessage($"Роль {roleName} не найдена", nameof(List));
            }

            var result = await _userManager.RemoveFromRoleAsync(user, role.Name);

            if (result.Succeeded)
            {
                return RedirectToActionWithMessage($"Пользователь {userName} удален из роли {roleName}", nameof(List));
            }
            else
            {
                return RedirectToActionWithMessage(result.Errors.ToString(), nameof(List));
            }
        }

        private RedirectToActionResult RedirectToActionWithMessage(string message, string action)
        {
            TempData["Message"] = message;

            return RedirectToAction(action);
        }
    }
}
