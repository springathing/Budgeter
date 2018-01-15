using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using mblyakher_budgeter.Models;

namespace mblyakher_budgeter.Controllers
{
    [Authorize]
    [RequireHttps]
    public class BudgetItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BudgetItems
        public ActionResult Index()
        {
            var budgetItems = db.BudgetItems.Include(b => b.Budget).Include(b => b.BudgetItemType);
            return View(budgetItems.ToList());
        }

        // GET: BudgetItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItem budgetItem = db.BudgetItems.Find(id);
            if (budgetItem == null)
            {
                return HttpNotFound();
            }
            return View(budgetItem);
        }

        // GET: BudgetItems/Create
        public ActionResult Create(int householdId, int budgetId)
        {
            Budget budget = db.Budgets.Find(budgetId);
            List<BudgetItem> budgetItems = db.BudgetItems.Where(i => i.BudgetId == budgetId).ToList();
            List<BudgetItemType> allTypes = db.BudgetItemTypes.ToList();
            List<BudgetItemType> usedTypes = new List<BudgetItemType>();
            List<BudgetItemType> listTypes = db.BudgetItemTypes.ToList();

            foreach (BudgetItemType budgetitemtype in allTypes)
            {
                foreach (BudgetItem budgetitem in budgetItems)
                {
                    if (budgetitemtype.Id == budgetitem.BudgetItemTypeId)
                    {
                        listTypes.Remove(budgetitemtype);
                    }
                }
            }
            ViewBag.HouseholdId = householdId;
            ViewBag.BudgetItemTypeId = new SelectList(listTypes.Where(c => c.Name != "Income"), "Id", "Name");
            //ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name");
            //ViewBag.BudgetItemTypeId = new SelectList(db.BudgetItemTypes, "Id", "Name");
            return View();
        }

        // POST: BudgetItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Amount,BudgetItemTypeId,BudgetId")] BudgetItem budgetItem, int householdId, int budgetId)
        {
            if (ModelState.IsValid)
            {
                budgetItem.BudgetId = budgetId;
                var budget = db.Budgets.Find(budgetItem.BudgetId);
                var budgetitemtype = db.BudgetItemTypes.Find(budgetItem.BudgetItemTypeId);
                db.BudgetItems.Add(budgetItem);
                db.SaveChanges();
                return RedirectToAction("Details", "Households", new { id = householdId });
            }

            ViewBag.BudgetId = budgetId;
            ViewBag.HouseholdId = householdId;
            ViewBag.BudgetItemTypeId = new SelectList(db.BudgetItemTypes, "Id", "Name", budgetItem.BudgetItemTypeId);
            return View(budgetItem);
        }

        // GET: BudgetItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItem budgetItem = db.BudgetItems.Find(id);
            if (budgetItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name", budgetItem.BudgetId);
            ViewBag.BudgetItemTypeId = new SelectList(db.BudgetItemTypes, "Id", "Name", budgetItem.BudgetItemTypeId);
            return View(budgetItem);
        }

        // POST: BudgetItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Amount,BudgetItemTypeId,BudgetId")] BudgetItem budgetItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(budgetItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name", budgetItem.BudgetId);
            ViewBag.BudgetItemTypeId = new SelectList(db.BudgetItemTypes, "Id", "Name", budgetItem.BudgetItemTypeId);
            return View(budgetItem);
        }

        // GET: BudgetItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItem budgetItem = db.BudgetItems.Find(id);
            if (budgetItem == null)
            {
                return HttpNotFound();
            }
            return View(budgetItem);
        }

        // POST: BudgetItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BudgetItem budgetItem = db.BudgetItems.Find(id);
            db.BudgetItems.Remove(budgetItem);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
