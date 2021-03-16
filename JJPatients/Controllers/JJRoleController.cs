using JJPatients.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JJPatients.Controllers
{
    [Authorize(Roles = "administrators")]
    public class JJRoleController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public JJRoleController(
                UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        // list all current roles, links to manage roles
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            var roles = roleManager.Roles.OrderBy(a => a.Name);
            return View(roles);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRole model)
        {
            try
            {        
                if (ModelState.IsValid)
                {
                    string roleName = model.RoleName.Trim();
                    IdentityRole role = await roleManager.FindByNameAsync(model.RoleName);
                    if (role != null)
                    {
                        TempData["message"] = model.RoleName + " is already on file";
                        return RedirectToAction("Index");
                    }
                    IdentityRole newRole = new IdentityRole { Name = model.RoleName };

                
                    var result = await roleManager.CreateAsync(newRole);

                    if (result.Succeeded)
                    {
                        TempData["message"] = "role added: " + model.RoleName;
                        return RedirectToAction("Index");

                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }else
                {
                    if (model.RoleName == null || model.RoleName.Trim() == "")
                    {
                        TempData["message"] = "Role name is empty";
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception e)
            {
                TempData["message"] = e.Message;
                return RedirectToAction("Index");
            }
                return RedirectToAction("Index");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ListRole()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if(role == null)
            {
                // Error Message
            }

            var model = new EditRole
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach(var user in userManager.Users)
            {
                if(await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRole model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);

            if(role == null)
            {
                // Error Message
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRole");

                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                // Error Message
                TempData["message"] = $"Role with id = {roleId} cannot be found";
                return RedirectToAction("Index");
            }

            var notInModel = new List<UserRole>();
            var inModel = new List<UserRole>();

            foreach(var user in userManager.Users)
            {
                var userrole = new UserRole
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    UserEmail = user.Email
                };

                if(await userManager.IsInRoleAsync(user, role.Name))
                {
                    userrole.IsSelected = true;
                    inModel.Add(userrole);
                }
                else
                {
                    userrole.IsSelected = false;
                    notInModel.Add(userrole);
                }
                
            }
            ViewBag.roleName = role.Name;
            ViewBag.roleId = roleId;
            ViewData["UserIdList"] = new SelectList(notInModel.OrderBy(a=>a.UserName), "UserId", "UserName");
            return View(inModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRole> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if(role == null)
            {
                // Error Message
                TempData["message"] = $"Role with id = {roleId} cannot be found";
                return RedirectToAction("EditUsersInRole", new { roleId = roleId });
            }

            for(int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;

                if(model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if(!model[i].IsSelected && (await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                if (result.Succeeded)
                {
                    if(i < (model.Count - 1))
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("EditRole", new { id = roleId });

                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return RedirectToAction("EditRole", new { id = roleId });
        }

        [HttpPost]
        public async Task<IActionResult> AddUserInRole(string roleId, string userId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                // Error Message
                TempData["message"] = $"Role with id = {roleId} cannot be found";
                return RedirectToAction("EditUsersInRole", new { roleId = roleId });
            }

            var user = await userManager.FindByIdAsync(userId);
            IdentityResult result = null;

            if (!await userManager.IsInRoleAsync(user, role.Name))
            {
                result = await userManager.AddToRoleAsync(user, role.Name);
            }
            else
            {
                // Error Message
                TempData["message"] = $"{user.UserName} is already on the role";
                return RedirectToAction("EditUsersInRole", new { roleId = roleId });
            }
            if (result.Succeeded)
            {
                TempData["message"] = $" {user.UserName} is added";
                return RedirectToAction("EditUsersInRole", new { roleId = roleId });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return RedirectToAction("EditUsersInRole", new { roleId = roleId });
        }


        [HttpPost]
        public async Task<IActionResult> DeleteUserInRole(string roleId, string userId)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            string currentUserId = "";
            if (claimsIdentity != null)
            {
                // the principal identity is a claims identity.
                // now we need to find the NameIdentifier claim
                var userIdClaim = claimsIdentity.Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

                if (userIdClaim != null)
                {
                    currentUserId = userIdClaim.Value;
                }
            }
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                // Error Message
                TempData["message"] = $"Role with id = {roleId} cannot be found";
                
                return RedirectToAction("EditUsersInRole", new { roleId = roleId });
            }

            var user = await userManager.FindByIdAsync(userId);
            IdentityResult result = null;
            if(currentUserId == userId && role.Name == "administrators")
            {
                // Error Message
                //ViewBag.ErrorMessage = "The current users cannot remove themselves from the administrators role";
                TempData["message"] = "The current users cannot remove themselves from the administrators role";
                return RedirectToAction("EditUsersInRole", new { roleId = roleId });
            }
            if (await userManager.IsInRoleAsync(user, role.Name))
            {
                result = await userManager.RemoveFromRoleAsync(user, role.Name);
            }
            else
            {
                // Error Message
                TempData["message"] = $"{user.UserName} doesn't exist on the role";
                return RedirectToAction("EditUsersInRole", new { roleId = roleId });
            }
            if (result.Succeeded)
            {
                TempData["message"] = $" {user.UserName} is deleted";
                return RedirectToAction("EditUsersInRole", new { roleId = roleId });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return RedirectToAction("EditUsersInRole", new { roleId = roleId });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                // Error Message
                TempData["message"] = $"Role with id = {roleId} cannot be found";
                return RedirectToAction("EditUsersInRole", new { roleId = roleId });
            }

            var userList = new List<UserRole>();

            foreach (var user in userManager.Users)
            {
                var userrole = new UserRole
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    UserEmail = user.Email
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userList.Add(userrole);
                }

            }
            //var model = new DeleteRole
            //{
            //    RoleId = role.Id
            //}
            
            if(userList == null || userList.Count == 0)
            {
                IdentityResult result = null;
                result = await roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    TempData["message"] = $" {role.Name} is deleted ";
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.roleId = roleId;
                ViewBag.rolename = role.Name;
                return View(userList);
            }            
        }


        [HttpPost]
        public async Task<IActionResult> DeleteRoleConfirmed(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                // Error Message
                TempData["message"] = $"Role with id = {roleId} cannot be found";

                return RedirectToAction("EditUsersInRole", new { roleId = roleId });
            }

            //var userList = new List<UserRole>();
            List<IdentityUser> userList = userManager.Users.ToList();
            for (int i = 0; i < userList.Count; i++)
            {
                IdentityResult result = null;
                var userrole = new UserRole
                {
                    UserId = userList[i].Id,
                    UserName = userList[i].UserName,
                    UserEmail = userList[i].Email
                };

                
                if (await userManager.IsInRoleAsync(userList[i], role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(userList[i], role.Name);
                }
                else
                {
                    break;
                }
                if (result.Succeeded)
                {
                    TempData["message"] = $" {userList[i].UserName} is deleted from " + role.Name + " role";
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }

            IdentityResult resultDeleteRole = null;
            resultDeleteRole = await roleManager.DeleteAsync(role);

            if (resultDeleteRole.Succeeded)
            {
                TempData["message"] = $" {role.Name} is deleted ";
            }

            foreach (var error in resultDeleteRole.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }


            return RedirectToAction("Index");
        }


    }

}

