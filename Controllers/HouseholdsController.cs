using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using mblyakher_budgeter.Models;
using Microsoft.AspNet.Identity;

namespace mblyakher_budgeter.Controllers
{
    [Authorize]
    [RequireHttps]
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Households
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(db.Households.ToList());
        }

        // GET: Households/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            HouseholdViewModel model = new HouseholdViewModel();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            model.household = db.Households.Find(id);
            if (model.household == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: Households/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,OwnerId")] Household household)
        {
            if (ModelState.IsValid)
            {
                string userId = User.Identity.GetUserId();
                household.OwnerId = userId;
                db.Households.Add(household);

                HouseholdUsersHelper helper = new HouseholdUsersHelper(db);
                helper.AddUserToHousehold(household.Id, userId);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(household);
        }

        public ActionResult Assign(int id)
        {
            var household = db.Households.Find(id);
            HouseholdUsersHelper helper = new HouseholdUsersHelper(db);
            var model = new AssignUsersViewModel();

            model.Household = household;
            model.SelectedUsers = helper.ListAssignedUsers(id).ToArray();
            model.Users = new MultiSelectList(model.SelectedUsers.Where(u => (u.FullName != "N/A" && u.FullName != "(Remove Assigned User)")).OrderBy(u => u.FirstName), "Id", "FullName", model.SelectedUsers);
            //model.Users = new MultiSelectList(db.Users.Where(u => (u.DisplayName != "N/A" && u.DisplayName != "(Remove Assigned User)")).OrderBy(u => u.FirstName), "Id", "DisplayName", model.SelectedUsers);

            return View(model);
        }

        [HttpPost]
        public ActionResult Assign(AssignUsersViewModel model)
        {
            var household = db.Households.Find(model.Household.Id);
            HouseholdUsersHelper helper = new HouseholdUsersHelper(db);

            foreach (var user in db.Users.Select(r => r.Id).ToList())
            {
                if (model.SelectedUsers != null)
                {
                    foreach (var item in model.SelectedUsers)
                    {
                        helper.RemoveUserFromHousehold(household.Id, item.Id);
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        // GET: Households/UnAssign/5
        public ActionResult UnAssign(int householdId, string userId)
        {
            //var household = db.Households.Find(householdId);
            //var user = db.Users.Find(userId);
            //HouseholdUsersHelper helper = new HouseholdUsersHelper(db);
            var model = new AssignUsersViewModel();

            ViewBag.HouseholdId = householdId;
            ViewBag.UserId = userId;

            return View(model);
        }

        // POST: Households/UnAssign/5
        [HttpPost]
        public ActionResult UnAssign(AssignUsersViewModel model, int householdId, string userId)
        {
            if (ModelState.IsValid)
            {
                var household = db.Households.Find(householdId);
                var user = db.Users.Find(userId);
                HouseholdUsersHelper helper = new HouseholdUsersHelper(db);

                helper.RemoveUserFromHousehold(household.Id, user.Id);
                return RedirectToAction("Assign", "Households", new { id = household.Id });
            }
            return View(model);
        }

        // GET: Households/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Household household)
        {
            if (ModelState.IsValid)
            {
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(household);
        }

        // GET: Households/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Household household = db.Households.Find(id);
            db.Households.Remove(household);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Households/Leave/5
        public ActionResult Leave(int? id)
        {
            Household household = db.Households.Find(id);
            ViewBag.HouseholdName = household.Name;
            ViewBag.HouseholdId = id;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Leave/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Leave(int id)
        {
            var user = User.Identity.GetUserId();
            HouseholdUsersHelper helper = new HouseholdUsersHelper(db);
            helper.RemoveUserFromHousehold(id, user);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
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
