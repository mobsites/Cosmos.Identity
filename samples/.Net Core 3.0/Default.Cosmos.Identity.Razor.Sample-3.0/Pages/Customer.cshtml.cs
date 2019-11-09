using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Default.Cosmos.Identity.Razor.Sample_3._0.Pages
{
    [Authorize(Roles = "Admin, Customer")]
    public class CustomerModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
