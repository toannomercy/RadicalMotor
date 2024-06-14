using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RadicalMotor.Models;
using System.Threading.Tasks;

namespace RadicalMotor.Areas.Identity.Pages.Account.Manage
{
	public class IndexModel : PageModel
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly ApplicationDbContext _context;

		public IndexModel(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			ApplicationDbContext context)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_context = context;
		}

		public InputModel Input { get; set; }

		public class InputModel
		{
			public string Username { get; set; }
			public string FullName { get; set; }
			public string PhoneNumber { get; set; }
			public string Email { get; set; }
			public string Address { get; set; }
			public string IdentityCard { get; set; }
			public string Gender { get; set; }
			public DateTime DateOfBirth { get; set; }
			public string ProfileImageUrl { get; set; }
		}

		public async Task<IActionResult> OnGetAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id);

			if (customer == null)
			{
				TempData["ErrorMessage"] = "Account not registered as a customer.";
				return RedirectToPage("/Account/Login", new { area = "Identity" });
			}

			Input = new InputModel
			{
				Username = user.UserName,
				FullName = customer.FullName,
				PhoneNumber = customer.PhoneNumber,
				Email = user.Email,
				Address = customer.Address,
				IdentityCard = customer.IdentityCard,
				Gender = customer.Gender,
				DateOfBirth = customer.DateOfBirth,
				ProfileImageUrl = customer.CustomerImages.FirstOrDefault()?.ImageUrl
			};

			return Page();
		}
	}
}
