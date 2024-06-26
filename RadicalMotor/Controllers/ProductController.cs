﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RadicalMotor.Models;
using RadicalMotor.Repository;

namespace RadicalMotor.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IVehicleImageRepository _vehicleImageRepository;
        private readonly IVehicleTypeRepository _vehicleTypeRepository;

        public ProductController(ApplicationDbContext context, IVehicleImageRepository vehicleImageRepository, IVehicleRepository vehicleRepository, IVehicleTypeRepository vehicleTypeRepository)
        {
            _context = context;
            _vehicleImageRepository = vehicleImageRepository;
            _vehicleRepository = vehicleRepository;
            _vehicleTypeRepository = vehicleTypeRepository;
        }

        // GET: Product
        public async Task<IActionResult> Index(string vehicleType)
        {
            var vehicles = await _vehicleRepository.GetAllAsync();
            var vehicleTypes = _vehicleTypeRepository.GetAllVehicleTypes();
            var vehicleImages = new Dictionary<string, IEnumerable<VehicleImage>>();

            foreach (var vehicle in vehicles)
            {
                var images = await _vehicleImageRepository.GetByChassisNumber(vehicle.ChassisNumber);
                vehicleImages.Add(vehicle.ChassisNumber, images);
            }
            var vehiclePrice = await _context.Vehicles
                        .Include(v => v.PriceList)
                        .ToListAsync();

            var prices = new Dictionary<string, decimal>();
            foreach (var vehicle in vehiclePrice)
            {
                prices.Add(vehicle.ChassisNumber, vehicle.PriceList?.Price ?? 0);
            }
            if (!string.IsNullOrEmpty(vehicleType))
            {
                vehicles = vehicles.Where(p => p.VehicleType.TypeName == vehicleType);
            }

            ViewBag.Prices = prices;

            ViewBag.VehicleImages = vehicleImages;

            var vehicleTypeNames = vehicleTypes.Select(v => v.TypeName).Distinct();
            ViewBag.VehicleTypes = vehicleTypeNames;
            return View(vehicles);
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.PriceList)
                .Include(v => v.VehicleType)
                .FirstOrDefaultAsync(m => m.ChassisNumber == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            ViewData["PriceListID"] = new SelectList(_context.Set<PriceList>(), "PriceListId", "PriceListId");
            ViewData["VehicleTypeId"] = new SelectList(_context.VehicleTypes, "VehicleTypeId", "VehicleTypeId");
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ChassisNumber,VehicleName,EntryDate,Version,PriceListID,VehicleTypeId")] Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PriceListID"] = new SelectList(_context.Set<PriceList>(), "PriceListId", "PriceListId", vehicle.PriceListID);
            ViewData["VehicleTypeId"] = new SelectList(_context.VehicleTypes, "VehicleTypeId", "VehicleTypeId", vehicle.VehicleTypeId);
            return View(vehicle);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            ViewData["PriceListID"] = new SelectList(_context.Set<PriceList>(), "PriceListId", "PriceListId", vehicle.PriceListID);
            ViewData["VehicleTypeId"] = new SelectList(_context.VehicleTypes, "VehicleTypeId", "VehicleTypeId", vehicle.VehicleTypeId);
            return View(vehicle);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ChassisNumber,VehicleName,EntryDate,Version,PriceListID,VehicleTypeId")] Vehicle vehicle)
        {
            if (id != vehicle.ChassisNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.ChassisNumber))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PriceListID"] = new SelectList(_context.Set<PriceList>(), "PriceListId", "PriceListId", vehicle.PriceListID);
            ViewData["VehicleTypeId"] = new SelectList(_context.VehicleTypes, "VehicleTypeId", "VehicleTypeId", vehicle.VehicleTypeId);
            return View(vehicle);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.PriceList)
                .Include(v => v.VehicleType)
                .FirstOrDefaultAsync(m => m.ChassisNumber == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(string id)
        {
            return _context.Vehicles.Any(e => e.ChassisNumber == id);
        }
    }
}
