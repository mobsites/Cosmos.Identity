using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using IdentityUser = Mobsites.AspNetCore.Identity.Cosmos.IdentityUser;

namespace Default.Cosmos.Identity.Razor.Sample.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public IList<IdentityUser> Admins { get; set; }
        public IList<IdentityUser> Employees { get; set; }
        public IList<IdentityUser> Customers { get; set; }
        public IList<IdentityUser> NonRoleUsers { get; set; }

        public bool IsUsers { get; set; }

        [TempData]
        public string Message { get; set; }

        public IndexModel(ILogger<IndexModel> logger, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
            ViewData["Title"] = "Home page";

            Admins = await _userManager.GetUsersInRoleAsync("Admin");
            Employees = await _userManager.GetUsersInRoleAsync("Employee");
            Customers = await _userManager.GetUsersInRoleAsync("Customer");
            NonRoleUsers = _userManager.Users.ToList();

            NonRoleUsers = NonRoleUsers
                .Where(user => !Admins.Contains(user, new UserComparer()) && !Employees.Contains(user, new UserComparer()) && !Customers.Contains(user, new UserComparer()))
                .ToList();

            IsUsers = Admins.Count() + Employees.Count + Customers.Count() + NonRoleUsers.Count() > 0;
        }

        public async Task<IActionResult> OnPostAsync(string username)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                if (User.Identity.Name == username)
                {
                    Message = $"You cannot delete yourself.";
                }
                else
                {
                    var user = await _userManager.FindByNameAsync(username);

                    if (user is null)
                    {
                        Message = $"Could not find user \"{username}\" to delete!";
                    }
                    else
                    {
                        var result = await _userManager.DeleteAsync(user);
                        if (!result.Succeeded)
                        {
                            Message = $"Could not remove user \"{username}\"!";
                        }
                    }
                }
            }
            else
            {
                Message = "Only Admin users can remove another user!";
            }

            return RedirectToPage();
        }
    }

    class UserComparer : IEqualityComparer<IdentityUser>
    {
        // Products are equal if their names and product numbers are equal.
        public bool Equals(IdentityUser x, IdentityUser y)
        {

            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (x is null || y is null)
                return false;

            //Check whether the products' properties are equal.
            return x.Id == y.Id;
        }

        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(IdentityUser user)
        {
            //Check whether the object is null
            if (user is null) return 0;

            //Get hash code for the Name field if it is not null.
            return user.Id == null ? 0 : user.Id.GetHashCode();
        }

    }
}
