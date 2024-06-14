using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using RadicalMotor.Models;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;

namespace RadicalMotor.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public ProfileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data submitted." });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id);
            if (customer == null)
            {
                return Json(new { success = false, message = "Account not registered as a customer." });
            }

            // Update customer properties
            customer.FullName = model.FullName;
            customer.PhoneNumber = model.PhoneNumber;
            customer.Address = model.Address;
            customer.DateOfBirth = model.DateOfBirth != DateTime.MinValue ? model.DateOfBirth : customer.DateOfBirth;
            customer.IdentityCard = model.IdentityCard;
            customer.Gender = model.Gender;
            _context.Customers.Update(customer);

            // Update user properties
            user.Email = model.Email;
            user.UserName = model.Username;
            var updateUserResult = await _userManager.UpdateAsync(user);
            if (!updateUserResult.Succeeded)
            {
                return Json(new { success = false, errors = updateUserResult.Errors.Select(e => e.Description).ToList() });
            }

            // Handle password change
            if (!string.IsNullOrEmpty(model.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetPasswordResult = await _userManager.ResetPasswordAsync(user, token, model.Password);
                if (!resetPasswordResult.Succeeded)
                {
                    return Json(new { success = false, errors = resetPasswordResult.Errors.Select(e => e.Description).ToList() });
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            // Handle profile image upload
            if (model.ProfileImage != null)
            {
                var imageUrl = await SaveProfileImageAsync(model.ProfileImage);
                var customerImage = _context.CustomerImages.FirstOrDefault(ci => ci.CustomerId == customer.CustomerId);

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
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles");
            Directory.CreateDirectory(uploadsFolder);
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(profileImage.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profileImage.CopyToAsync(stream);
            }
            return $"/images/profiles/{uniqueFileName}";
        }
    }
}
