using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RadicalMotor.Models;
using RadicalMotor.Repository;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RadicalMotor.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICustomerRepository _customerRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ICustomerRepository customerRepository,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _customerRepository = customerRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileDTO model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Unable to load user.");
                return RedirectToAction("Index", "Manage", new { area = "Identity" });
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Model validation failed.");
                return RedirectToAction("Index", "Manage", new { area = "Identity" });
            }

            var customer = await _customerRepository.GetCustomerByUserIdAsync(user.Id);
            if (customer == null)
            {
                ModelState.AddModelError(string.Empty, "Unable to load customer.");
                return RedirectToAction("Index", "Manage", new { area = "Identity" });
            }

            // Update user fields
            if (model.Email != user.Email)
            {
                user.Email = model.Email;
                var setEmailResult = await _userManager.UpdateAsync(user);
                if (!setEmailResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Unexpected error when trying to set email.");
                    return RedirectToAction("Index", "Manage", new { area = "Identity" });
                }
            }

            if (model.PhoneNumber != user.PhoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Unexpected error when trying to set phone number.");
                    return RedirectToAction("Index", "Manage", new { area = "Identity" });
                }
            }

            // Update customer fields
            customer.FullName = model.FullName;
            customer.Address = model.Address;
            customer.DateOfBirth = model.DateOfBirth;
            await _customerRepository.UpdateAsync(customer);

            // Save profile image
            if (model.ProfileImage != null && model.ProfileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var image = SixLabors.ImageSharp.Image.Load(model.ProfileImage.OpenReadStream()))
                {
                    image.Mutate(x => x.Resize(150, 150));
                    await image.SaveAsync(filePath); // Automatic encoder selected based on file extension.
                }

                var customerImage = new CustomerImage
                {
                    ImageUrl = "/img/" + uniqueFileName,
                    CustomerId = customer.CustomerId
                };
                await _customerRepository.AddCustomerImageAsync(customerImage);
            }

            if (!string.IsNullOrEmpty(model.Password) && !string.IsNullOrEmpty(model.ConfirmPassword) && model.Password == model.ConfirmPassword)
            {
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.Password, model.Password);
                if (!changePasswordResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Unexpected error when trying to set password.");
                    return RedirectToAction("Index", "Manage", new { area = "Identity" });
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["SuccessMessage"] = "Your profile has been updated";
            return RedirectToAction("Index", "Manage", new { area = "Identity" });
        }
    }
}
