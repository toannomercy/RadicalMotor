using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RadicalMotor.Models;
using RadicalMotor.Repository;

namespace RadicalMotor.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICustomerRepository _customerRepository;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ICustomerRepository customerRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _customerRepository = customerRepository;
        }

        [BindProperty]
        public ProfileDTO Input { get; set; }

        private async Task LoadAsync(ApplicationUser user)
        {
            var customer = await _customerRepository.GetCustomerByUserIdAsync(user.Id);
            if (customer == null)
            {
                customer = new Customer
                {
                    UserId = user.Id,
                    FullName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                };
                await _customerRepository.AddAsync(customer);
            }

            Input = new ProfileDTO
            {
                Username = user.UserName,
                FullName = customer.FullName,
                PhoneNumber = customer.PhoneNumber,
                Email = user.Email,
                Address = customer.Address,
                ProfileImageUrl = customer.CustomerImages.FirstOrDefault()?.ImageUrl
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }
    }
}
