using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Extended.Cosmos.Identity.Razor.Sample_3._0.Pages
{
    [Authorize(Roles = "Admin, Employee")]
    public class EmployeeModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
