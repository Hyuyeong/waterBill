using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WaterBill.Data;
using WaterBill.Models;

namespace WaterBill.Controllers
{
    public class WaterConsumptionController : Controller
    {
        private readonly ApplicationDbContext _db;

        public WaterConsumptionController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: WaterComsuptionController
        public ActionResult Index()
        {
            List<WaterConsumption> objWaterConsumptionList = _db.WaterConsumptions.ToList();
            return View(objWaterConsumptionList);
        }

        public ActionResult Create()
        {
            var lastRecord = _db.WaterConsumptions.OrderByDescending(w => w.Date).FirstOrDefault();
            ViewBag.LastDate = lastRecord?.Date.ToString("yyyy-MM-dd"); // Format for HTML date input
            ViewBag.LastMeterReading = lastRecord?.MeterReading ?? 0;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(WaterConsumption obj)
        {
            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            // Get the last meter reading value
            int lastReading = await _db
                .WaterConsumptions.OrderByDescending(x => x.Date)
                .Select(x => x.MeterReading)
                .FirstOrDefaultAsync();

            // Calculate charge and assign it to obj
            obj.Charge =
                ((obj.MeterReading - lastReading) * 2.142)
                + ((obj.MeterReading - lastReading) * 0.785) * 3.726;

            // Add the new reading to the database
            _db.WaterConsumptions.Add(obj);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            WaterConsumption? waterConsumptionFromDb = _db.WaterConsumptions.Find(id);

            if (waterConsumptionFromDb == null)
            {
                return NotFound();
            }
            _db.WaterConsumptions.Remove(waterConsumptionFromDb);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
