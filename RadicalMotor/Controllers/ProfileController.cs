using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using RadicalMotor.Models;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace RadicalMotor.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ProfileController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile([FromForm] ProfileDTO model)
        {
            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
            }

            if (model.ProfileImage == null)
            {
                ModelState.Remove(nameof(model.ProfileImage));
                ModelState.Remove(nameof(model.ProfileImageUrl));
            }


            if (!ModelState.IsValid)
            {
                var modelStateEntries = ModelState.Select(x => new { x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToList() });
                foreach (var entry in modelStateEntries)
                {
                    Console.WriteLine($"Key: {entry.Key}, Errors: {string.Join(", ", entry.Errors)}");
                }

                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, errors });
            }


            var user = await _userManager.GetUserAsync(User);
            var customer = _context.Customers.FirstOrDefault(c => c.UserId == user.Id);
            var customerImage = _context.CustomerImages.FirstOrDefault(ci => ci.CustomerId == customer.CustomerId);

            if (user != null)
            {
                user.Email = model.Email;
                user.UserName = model.Username;
                await _userManager.UpdateAsync(user);

                if (!string.IsNullOrEmpty(model.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await _userManager.ResetPasswordAsync(user, token, model.Password);
                    if (!result.Succeeded)
                    {
                        return Json(new { success = false, errors = result.Errors.Select(e => e.Description).ToList() });
                    }
                }
            }

            if (customer != null)
            {
                customer.FullName = model.FullName;
                customer.PhoneNumber = model.PhoneNumber;
                customer.Address = model.Address;
                customer.DateOfBirth = model.DateOfBirth != DateTime.MinValue ? model.DateOfBirth : customer.DateOfBirth;
                _context.Customers.Update(customer);
            }

            if (model.ProfileImage != null)
            {
                var imageUrl = await SaveProfileImageAsync(model.ProfileImage);
                if (customerImage != null)
                {
                    customerImage.ImageUrl = imageUrl;
                    _context.CustomerImages.Update(customerImage);
                }
                else
                {
                    customerImage = new CustomerImage
                    {
                        CustomerId = customer.CustomerId,
                        ImageUrl = imageUrl
                    };
                    _context.CustomerImages.Add(customerImage);
                }
                model.ProfileImageUrl = imageUrl;
            }


            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Profile updated successfully!" });
        }

        private async Task<string> SaveProfileImageAsync(IFormFile profileImage)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img");
            Directory.CreateDirectory(uploadsFolder);
            var uniqueFileName = Path.GetFileName(profileImage.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profileImage.CopyToAsync(stream);
            }
            return $"/img/{uniqueFileName}";
        }
    }
}
