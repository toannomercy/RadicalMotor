using Microsoft.AspNetCore.Mvc;
using RadicalMotor.Models;
using RadicalMotor.Repository;

namespace RadicalMotor.Areas.Identity.Admin.Controllers
{
    [Area("Admin")]
    public class VehicleController : Controller
    {
        private readonly IVehicleImageRepository _vehicleImageRepository;
        private readonly IVehicleRepository _vehicleRepository;
        public VehicleController(IVehicleImageRepository vehicleImageRepository, IVehicleRepository vehicleRepository)
        {
            _vehicleImageRepository = vehicleImageRepository;
            _vehicleRepository = vehicleRepository;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.VehicleImages = await _vehicleImageRepository.GetAll();
            var vehicles = await _vehicleRepository.GetAllWithPriceListAsync();
            return View(vehicles);
        }
        // GET: Admin/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vehicle vehicle, List<IFormFile> images)
        {
            if (ModelState.IsValid)
            {
                var vehicleImages = new List<VehicleImage>();
                foreach (var image in images)
                {
                    if (image.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await image.CopyToAsync(ms);
                            var imageBytes = ms.ToArray();
                            vehicleImages.Add(new VehicleImage { Vehicle = vehicle, ImageUrl = Convert.ToBase64String(imageBytes) });
                        }
                    }
                }
                _vehicleRepository.AddVehicle(vehicle, vehicleImages);
                return RedirectToAction(nameof(Index));
            }
            return View(vehicle);
        }

    }
}
