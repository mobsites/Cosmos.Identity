using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Extended.Cosmos.Identity.Razor.Sample.Pages
{
    [Authorize(Roles = "Admin, Employee")]
    public class EmployeeModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
