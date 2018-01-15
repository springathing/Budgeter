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
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index()
        {
            var transactions = db.Transactions.Include(t => t.Account).Include(t => t.TransactionType);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Deposit
        public ActionResult Deposit(int householdId, int accountId)
        {
            //ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "Name");
            ViewBag.HouseholdId = householdId;
            ViewBag.AccountId = accountId;
            //ViewBag.CategoryId = 1;
            //ViewBag.TypeId = 1;
            ViewBag.MadeBy = User.Identity.GetUserId();
            return View();
        }

        // POST: Transactions/Deposit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deposit([Bind(Include = "Id,Description,CreatedDate,Amount,TransactionTypeId,AccountId,MadeById")] Transaction transaction, int householdId, int accountId)
        {
            if (ModelState.IsValid)
            {
                transaction.AccountId = accountId;
                var Account = db.Accounts.Find(transaction.AccountId);
                if (Account != null)
                {
                    transaction.Account = Account;
                    transaction.MadeBy = ViewBag.EnteredbyId;
                    transaction.TransactionTypeId = 1;
                    transaction.CreatedDate = DateTimeOffset.Now;
                }
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Details", "Households", new { id = householdId });
            }

            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name", transaction.AccountId);
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Name", transaction.TransactionTypeId);
            ViewBag.HouseholdId = householdId;
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create(int? householdId)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            Household household = db.Households.Find(householdId);
            var ownAccounts = context.Accounts.Where(b => b.Household.Id == household.Id);

            ViewBag.HouseholdId = householdId;
            ViewBag.AccountId = new SelectList(ownAccounts, "Id", "Name");
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes.Where(c => c.Name != "Income"), "Id", "Name");
            ViewBag.MadeById = User.Identity.GetUserId();
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Description,CreatedDate,Amount,TrasactionTypeId,AccountId")] Transaction transaction, int householdId)
        {
            if (ModelState.IsValid)
            {
                var Account = db.Accounts.Find(transaction.AccountId);
                if (Account != null)
                {
                    transaction.Account = Account;
                    transaction.MadeBy = ViewBag.MadeById;
                    transaction.TransactionTypeId = 2;
                    transaction.CreatedDate = DateTimeOffset.Now;
                    db.Transactions.Add(transaction);
                    db.SaveChanges();
                    return RedirectToAction("Details", "Households", new { id = householdId });
                }
            }

            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name", transaction.AccountId);
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Name", transaction.TransactionTypeId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? householdId, int? transactionId)
        {
            if (transactionId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(transactionId);
            if (transaction == null)
            {
                return HttpNotFound();
            }

            transaction.Account = db.Accounts.Find(transaction.AccountId);
            Household household = db.Households.Find(householdId);
            ViewBag.HouseholdId = householdId;
            ViewBag.AccountId = new SelectList(household.Accounts, "Id", "Name", transaction.AccountId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Description,CreatedDate,Amount,TrasactionTypeId,AccountId")] Transaction transaction, int householdId)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HouseholdId = householdId;
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name", transaction.AccountId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? householdId, int? transactionId)
        {
            if (transactionId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(transactionId);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.HouseholdId = householdId;
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int householdId, int transactionId)
        {
            Transaction transaction = db.Transactions.Find(transactionId);
            db.Transactions.Remove(transaction);
            db.SaveChanges();

            ViewBag.HouseholdId = householdId;
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
