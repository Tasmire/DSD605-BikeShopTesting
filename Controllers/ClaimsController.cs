using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DSD603_BikeShopDB.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace DSD603_BikeShopDB.Controllers
{
    [Authorize(Policy = "ViewClaimsPolicy")]
    public class ClaimsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ClaimsController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: Claims/Index
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .OrderBy(u => u.UserName)
                .ToListAsync();

            var userClaimsList = new List<UserClaimsViewModel>();
            foreach (var user in users)
            {
                var claims = await _userManager.GetClaimsAsync(user);
                userClaimsList.Add(new UserClaimsViewModel
                {
                    UserName = user.UserName ?? "Unknown User",
                    Claims = claims.ToList()
                });
            }

            var model = new ClaimsIndexViewModel
            {
                UserClaims = userClaimsList,
                Users = users.Select(u => u.UserName ?? "Unknown User").ToList(),
                StatusMessage = TempData["StatusMessage"] as string
            };

            return View(model);
        }

        // GET: Claims/Create
        public async Task<IActionResult> Create()
        {
            var users = await _userManager.Users
                .OrderBy(u => u.UserName)
                .ToListAsync();

            var userItems = users.Select(u => new SelectListItem
            {
                Value = u.UserName ?? "Unknown User",
                Text = u.UserName ?? "Unknown User"
            });

            var model = new ClaimsCreateViewModel
            {
                Users = new SelectList(userItems, "Value", "Text"),
                StatusMessage = TempData["StatusMessage"] as string
            };

            model = await BuildCreateViewModel();

            return View(model);
        }

        // POST: Claims/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClaimsCreateViewModel model)
        {
            model.Users = await GetUsersSelectListAsync(model.SelectedUserName);

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByNameAsync(model.SelectedUserName);
            if (user == null)
            {
                model.StatusMessage = "Selected user not found.";
                return View(model);
            }

            var existingClaims = await _userManager.GetClaimsAsync(user);
            var isDuplicate = existingClaims.Any(c =>
                string.Equals(c.Type, model.ClaimType.Trim(), StringComparison.OrdinalIgnoreCase) &&
                string.Equals(c.Value, model.ClaimValue?.Trim() ?? string.Empty, StringComparison.OrdinalIgnoreCase));

            if (isDuplicate)
            {
                model.StatusMessage = $"User '{user.UserName}' already has the claim '{model.ClaimType}: {model.ClaimValue}'.";
                return View(model);
            }

            var claim = new Claim(model.ClaimType.Trim(), model.ClaimValue?.Trim() ?? string.Empty);
            var result = await _userManager.AddClaimAsync(user, claim);

            if (result.Succeeded)
            {
                TempData["StatusMessage"] = $"Success: Created and assigned claim '{model.ClaimType}: {model.ClaimValue}' to user '{user.UserName}'.";
                return RedirectToAction(nameof(Create));
            }
            else
            {
                model.StatusMessage = string.Join("; ", result.Errors.Select(e => e.Description));
                return View(model);
            }
        }

        // GET: Claims/Assign
        public async Task<IActionResult> Assign()
        {
            var users = await _userManager.Users
                .OrderBy(u => u.UserName)
                .ToListAsync();

            var userItems = users.Select(u => new SelectListItem
            {
                Value = u.UserName ?? "Unknown User",
                Text = u.UserName ?? "Unknown User"
            });

            var model = new ClaimsAssignViewModel
            {
                Users = new SelectList(userItems, "Value", "Text"),
                StatusMessage = TempData["StatusMessage"] as string
            };

            return View(model);
        }

        // POST: Claims/Assign
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(ClaimsAssignViewModel model)
        {
            model.Users = await GetUsersSelectListAsync(model.SelectedUserName);

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByNameAsync(model.SelectedUserName);
            if (user == null)
            {
                model.StatusMessage = "Selected user not found.";
                return View(model);
            }

            var claim = new Claim(model.ClaimType.Trim(), model.ClaimValue?.Trim() ?? string.Empty);
            var result = await _userManager.AddClaimAsync(user, claim);

            if (result.Succeeded)
            {
                model.StatusMessage = $"Success: Assigned claim '{model.ClaimType}: {model.ClaimValue}' to user '{user.UserName}'.";
                model.ClaimType = string.Empty;
                model.ClaimValue = string.Empty;
                model.SelectedUserName = string.Empty;
            }
            else
            {
                model.StatusMessage = string.Join("; ", result.Errors.Select(e => e.Description));
            }

            return View(model);
        }

        // Utility method
        private async Task<SelectList> GetUsersSelectListAsync(string? selectedUserName = null)
        {
            var users = await _userManager.Users
                .OrderBy(u => u.UserName)
                .ToListAsync();

            var userItems = users.Select(u => new SelectListItem
            {
                Value = u.UserName ?? "Unknown User",
                Text = u.UserName ?? "Unknown User"
            });

            return new SelectList(userItems, "Value", "Text", selectedUserName);
        }

        private async Task<ClaimsCreateViewModel> BuildCreateViewModel(string? selectedUserName = null)
        {
            var users = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();

            // Gather all claims
            var allUserClaims = new List<UserClaimDetail>();
            var claimTypeGroups = new List<ClaimTypeGroup>();
            var claimTypeDict = new Dictionary<string, HashSet<string>>();
            var usersWithClaimsSet = new HashSet<string>();

            foreach (var user in users)
            {
                var claims = await _userManager.GetClaimsAsync(user);
                if (claims.Any())
                    usersWithClaimsSet.Add(user.UserName ?? "");

                foreach (var claim in claims)
                {
                    allUserClaims.Add(new UserClaimDetail
                    {
                        UserName = user.UserName ?? "",
                        Email = user.Email ?? "",
                        UserId = user.Id,
                        ClaimType = claim.Type,
                        ClaimValue = claim.Value
                    });

                    if (!claimTypeDict.ContainsKey(claim.Type))
                        claimTypeDict[claim.Type] = new HashSet<string>();
                    claimTypeDict[claim.Type].Add(claim.Value);
                }
            }

            foreach (var kvp in claimTypeDict)
            {
                claimTypeGroups.Add(new ClaimTypeGroup
                {
                    ClaimType = kvp.Key,
                    Values = kvp.Value.ToList(),
                    UsageCount = allUserClaims.Count(uc => uc.ClaimType == kvp.Key)
                });
            }

            return new ClaimsCreateViewModel
            {
                Users = new SelectList(users.Select(u => new SelectListItem
                {
                    Value = u.UserName ?? "Unknown User",
                    Text = u.UserName ?? "Unknown User"
                }), "Value", "Text", selectedUserName),
                ClaimTypeGroups = claimTypeGroups,
                AllUserClaims = allUserClaims,
                TotalClaimTypes = claimTypeGroups.Count,
                TotalClaims = allUserClaims.Count,
                UsersWithClaims = usersWithClaimsSet.Count,
                StatusMessage = TempData["StatusMessage"] as string
            };
        }
    }
}