using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Default.Cosmos.Identity.Razor.Sample.Pages
{
    [Authorize(Roles = "Admin, Employee")]
    public class EmployeeModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
