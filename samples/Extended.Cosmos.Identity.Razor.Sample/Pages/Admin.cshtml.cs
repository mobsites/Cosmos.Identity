using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Extended.Cosmos.Identity.Razor.Sample.Pages
{
    [Authorize(Roles = "Admin")]
    public class AdminModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
