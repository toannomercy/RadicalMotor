using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RadicalMotor.Models;
using RadicalMotor.Repository;

namespace RadicalMotor.Areas.Identity.Admin.Controllers
{
    [Area("Admin")]
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
        public async Task<IActionResult> Index()
        {
            ViewBag.VehicleImages = await _vehicleImageRepository.GetAll();
            var vehicles = await _vehicleRepository.GetAllWithPriceListAsync();
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

    }
}
