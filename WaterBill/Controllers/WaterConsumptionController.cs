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

        public IActionResult Edit(int? id)
        {
            var lastBeforeRecord = _db
                .WaterConsumptions.Where(w => w.Date < _db.WaterConsumptions.Max(wc => wc.Date)) // Get records before the latest date
                .OrderByDescending(w => w.Date) // Sort descending to get the most recent before the latest
                .FirstOrDefault(); // Get the top 1 result
            ViewBag.LastDate = lastBeforeRecord?.Date.ToString("yyyy-MM-dd"); // Format for HTML date input
            ViewBag.LastMeterReading = lastBeforeRecord?.MeterReading ?? 0;

            if (id == null || id == 0)
            {
                return NotFound();
            }
            WaterConsumption? waterConsumptionFromDb = _db.WaterConsumptions.Find(id);

            if (waterConsumptionFromDb == null)
            {
                return NotFound();
            }
            return View(waterConsumptionFromDb);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(WaterConsumption obj)
        {
            var lastBeforeRecord = _db
                .WaterConsumptions.Where(w => w.Date < _db.WaterConsumptions.Max(wc => wc.Date)) // Get records before the latest date
                .OrderByDescending(w => w.Date) // Sort descending to get the most recent before latest
                .FirstOrDefault(); // Get the top 1 result

            if (lastBeforeRecord != null) // Ensure lastBeforeRecord is not null
            {
                obj.Charge =
                    ((obj.MeterReading - lastBeforeRecord.MeterReading) * 2.142)
                    + ((obj.MeterReading - lastBeforeRecord.MeterReading) * 0.785) * 3.726;
            }
            else
            {
                // If there's no previous record, you might want to set a default charge or handle it differently
                obj.Charge = 0; // Set default charge if no previous record exists
            }

            if (ModelState.IsValid)
            {
                _db.WaterConsumptions.Update(obj);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(obj); // Return view with obj to retain user input in case of errors
        }
    }
}
