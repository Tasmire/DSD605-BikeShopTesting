using DSD603_BikeShopDB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DSD603_BikeShopDB.Controllers
{
    [Authorize(Policy = "ViewRolesPolicy")]
    public class RolesController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new RolesIndexViewModel
            {
                Roles = await _roleManager.Roles.AsNoTracking().ToListAsync(),
                UserRoles = new List<UserRolesViewModel>()
            };

            var users = await _userManager.Users.AsNoTracking().ToListAsync();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                vm.UserRoles.Add(new UserRolesViewModel
                {
                    UserName = user.UserName,
                    Roles = roles.ToList()
                });
            }

            return View(vm);
        }

        // GET: Roles/Create
        [HttpGet]
        public IActionResult Create()
        {
            var vm = new RolesCreateViewModel();
            return View(vm); // Simply return empty model to populate the form
        }

        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RolesCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var roleName = model.Name.Trim();

            // Prevent duplicate roles
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                ModelState.AddModelError(nameof(model.Name), $"The role “{roleName}” already exists.");
                return View(model);
            }

            // Create the new role
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
                return RedirectToAction(nameof(Index));

            // Surface any errors from RoleManager
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Assign()
        {
            var vm = new RolesAssignViewModel();
            await PopulateListsAsync(vm); // Populate the dropdowns
            return View(vm); // Return empty model to populate the form
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(RolesAssignViewModel model)
        {
            ModelState.Remove(nameof(model.Users));
            ModelState.Remove(nameof(model.Roles));

            // Populate the SelectLists BEFORE validation
            await PopulateListsAsync(model);

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByNameAsync(model.SelectedUserName);
            if (user == null)
            {
                ModelState.AddModelError(nameof(model.SelectedUserName), "Selected user does not exist.");
                return View(model);
            }

            if (!await _roleManager.RoleExistsAsync(model.SelectedRoleName))
            {
                ModelState.AddModelError(nameof(model.SelectedRoleName), "Selected role does not exist.");
                return View(model);
            }

            if (await _userManager.IsInRoleAsync(user, model.SelectedRoleName))
            {
                ModelState.AddModelError(string.Empty, $"User '{model.SelectedUserName}' is already assigned to role '{model.SelectedRoleName}'.");
                return View(model);
            }

            var result = await _userManager.AddToRoleAsync(user, model.SelectedRoleName);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateListsAsync(RolesAssignViewModel vm)
        {
            var users = await _userManager.Users.AsNoTracking().OrderBy(u => u.UserName).ToListAsync();
            var roles = await _roleManager.Roles.AsNoTracking().OrderBy(r => r.Name).ToListAsync();

            vm.Users = new SelectList(users, nameof(IdentityUser.UserName), nameof(IdentityUser.UserName), vm.SelectedUserName);
            vm.Roles = new SelectList(roles, nameof(IdentityRole.Name), nameof(IdentityRole.Name), vm.SelectedRoleName);
        }
    }
}
