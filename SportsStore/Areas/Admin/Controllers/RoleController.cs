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
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;


        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }


        public IActionResult List()
        {
            var roles = _roleManager.Roles.AsEnumerable();
            var rolesName = new List<string>();

            foreach(var role in roles)
            {
                rolesName.Add(role.Name);
            }

            return View(rolesName.AsEnumerable());
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Delete(string roleName)
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

        private RedirectToActionResult RedirectToActionWithMessage(string message, string action)
        {
            TempData["Message"] = message;

            return RedirectToAction(action);
        }
    }
}
