using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RadicalMotor.Models;
using RadicalMotor.Repository;

namespace RadicalMotor.Areas.Identity.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class VehicleController : Controller
    {
        private readonly IVehicleImageRepository _vehicleImageRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IVehicleTypeRepository _vehicleTypeRepository;
        private readonly IPriceListRepository _priceListRepository;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public VehicleController(IWebHostEnvironment webHostEnvironment, IPriceListRepository priceListRepository, ApplicationDbContext context, IVehicleImageRepository vehicleImageRepository, IVehicleRepository vehicleRepository, IVehicleTypeRepository vehicleTypeRepository)
        {
            _vehicleImageRepository = vehicleImageRepository;
            _vehicleRepository = vehicleRepository;
            _vehicleTypeRepository = vehicleTypeRepository;
            _context = context;
            _priceListRepository = priceListRepository;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index(string searchString)
        {
            ViewBag.VehicleImages = await _vehicleImageRepository.GetAll();
            var vehicles = await _vehicleRepository.GetAllWithPriceListAsync();
            if (!string.IsNullOrEmpty(searchString))
            {
                vehicles = vehicles.Where(p => p.VehicleName.ToLower().Contains(searchString.ToLower()));
            }
            return View(vehicles);
        }
        // GET: Admin/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.VersionOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Normal", Text = "Normal" },
                new SelectListItem { Value = "Special", Text = "Special" }
            };
            var vehicleTypes = _vehicleTypeRepository.GetAllVehicleTypes();
            ViewBag.VehicleTypes = new SelectList(vehicleTypes, "VehicleTypeId", "TypeName");
            var priceListIds = (await _priceListRepository.GetAllPriceListsAsync())
                .Select(pl => pl.PriceListId)
                .ToList();

            ViewBag.PriceListIds = new SelectList(priceListIds);
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            var vehicle = await _context.Vehicles
                .Include(v => v.PriceList)
                .Include(v => v.VehicleType)
                .FirstOrDefaultAsync(m => m.ChassisNumber == id);
            var vehicleImages = new Dictionary<string, IEnumerable<VehicleImage>>();
            var prices = new Dictionary<string, decimal>();
            var images = await _vehicleImageRepository.GetByChassisNumber(vehicle.ChassisNumber);
            prices.Add(vehicle.ChassisNumber, vehicle.PriceList?.Price ?? 0);
            vehicleImages.Add(vehicle.ChassisNumber, images);
            if (id == null)
            {
                return NotFound();
            }


            if (vehicle == null)
            {
                return NotFound();
            }
            ViewBag.Prices = prices;

            ViewBag.VehicleImages = vehicleImages;

            return View(vehicle);
        }
        // POST: Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                var vehicleImages = new List<VehicleImage>();

                if (vehicle.Images != null && vehicle.Images.Count > 0)
                {
                    foreach (var image in vehicle.Images)
                    {
                        if (image.Length > 0)
                        {
                            var imageUrl = await SaveImage(image);

                            var vehicleImage = new VehicleImage
                            {
                                ImageUrl = imageUrl,
                                Vehicle = vehicle
                            };

                            vehicleImages.Add(vehicleImage);
                        }
                    }
                }

                vehicle.VehicleImages = vehicleImages;
                _vehicleRepository.AddVehicle(vehicle);

                return RedirectToAction(nameof(Index));
            }

            return View(vehicle);
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine(_webHostEnvironment.WebRootPath, "img", image.FileName);

            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/img/" + image.FileName;
        }


        // GET: Admin/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            ViewBag.VersionOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Normal", Text = "Normal" },
                new SelectListItem { Value = "Special", Text = "Special" }
            };

            var vehicleTypes = _vehicleTypeRepository.GetAllVehicleTypes();
            ViewBag.VehicleTypes = new SelectList(vehicleTypes, "VehicleTypeId", "TypeName");
            var priceListIds = (await _priceListRepository.GetAllPriceListsAsync())
                .Select(pl => pl.PriceListId)
                .ToList();

            ViewBag.PriceListIds = new SelectList(priceListIds);
            return View(vehicle);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                var existingVehicle = await _vehicleRepository.GetByIdAsync(vehicle.ChassisNumber);

                if (existingVehicle == null)
                {
                    return NotFound();
                }
                if (vehicle.Images != null && vehicle.Images.Any(img => img.Length > 0))
                {
                    foreach (var img in existingVehicle.VehicleImages)
                    {
                        _vehicleImageRepository.Delete(img.ImageId);
                    }
                    foreach (var image in vehicle.Images)
                    {
                        if (image.Length > 0)
                        {
                            var imageUrl = await SaveImage(image);
                            var vehicleImage = new VehicleImage
                            {
                                ImageUrl = imageUrl,
                                ChassisNumber = existingVehicle.ChassisNumber
                            };
                            _vehicleImageRepository.Add(vehicleImage);
                        }
                    }
                }

                _vehicleRepository.UpdateVehicle(vehicle);

                return RedirectToAction(nameof(Index));
            }

            ViewBag.VersionOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Normal", Text = "Normal" },
                new SelectListItem { Value = "Special", Text = "Special" }
            };

            var vehicleTypes = _vehicleTypeRepository.GetAllVehicleTypes();
            ViewBag.VehicleTypes = new SelectList(vehicleTypes, "VehicleTypeId", "TypeName");
            var priceListIds = (await _priceListRepository.GetAllPriceListsAsync())
                .Select(pl => pl.PriceListId)
                .ToList();

            ViewBag.PriceListIds = new SelectList(priceListIds);
            return View(vehicle);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var vehicle = await _context.Vehicles
                .Include(v => v.PriceList)
                .Include(v => v.VehicleType)
                .FirstOrDefaultAsync(m => m.ChassisNumber == id);
            var vehicleImages = new Dictionary<string, IEnumerable<VehicleImage>>();
            var prices = new Dictionary<string, decimal>();
            var images = await _vehicleImageRepository.GetByChassisNumber(vehicle.ChassisNumber);
            prices.Add(vehicle.ChassisNumber, vehicle.PriceList?.Price ?? 0);
            vehicleImages.Add(vehicle.ChassisNumber, images);
            if (id == null)
            {
                return NotFound();
            }


            if (vehicle == null)
            {
                return NotFound();
            }
            ViewBag.Prices = prices;

            ViewBag.VehicleImages = vehicleImages;
            return View(vehicle);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);

            if (vehicle == null)
            {
                return NotFound();
            }

            _vehicleRepository.DeleteVehicle(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
