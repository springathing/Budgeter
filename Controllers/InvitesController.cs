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
using System.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;

namespace mblyakher_budgeter.Controllers
{
    [RequireHttps]
    public class InvitesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Invites
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var invites = db.Invites.Include(i => i.Household);
            return View(invites.ToList());
        }

        // GET: Invites/Details/5
        [Authorize(Roles = "Adming")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invite invite = db.Invites.Find(id);
            if (invite == null)
            {
                return HttpNotFound();
            }
            return View(invite);
        }

        // GET: Invites/Create
        [Authorize]
        public ActionResult Create(int? id)
        {
            var sender = db.Users.Find(User.Identity.GetUserId());
            var household = db.Households.Find(id);

            ViewBag.EmailFromId = sender.Id;
            ViewBag.HouseholdId = household.Id;
            //ViewBag.SentFromId = new SelectList(db.Users, "Id", "FirstName");
            //ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name");
            return View();
        }

        // POST: Invites/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,EmailTo,Created,Code,EmailFromId,HouseholdId")] Invite invite)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            var finalString = new String(stringChars);

            invite.Code = finalString;
            invite.Created = DateTimeOffset.Now;
            if (ModelState.IsValid)
            {
                db.Invites.Add(invite);
                db.SaveChanges();

                try
                {
                    EmailModel model = new EmailModel();
                    string body = string.Empty;
                    var household = db.Households.Find(invite.HouseholdId);
                    var checkRegistry = db.Users.Any(p => p.Email == invite.EmailTo);
                    if (checkRegistry == true)
                    {
                        if (!household.Users.Any(p => p.Email == invite.EmailTo))
                        {
                            body = "You've been invited to join the Household " + household.Name + " by " + user.FullName + ". " +
                                "Please click <a href='https://mblyakher-budgeter.azurewebsites.net/Households/'>here</a> to accept the invitation.";
                        }
                        else
                        {
                            body = "You've been invited to join the Household " + household.Name + " by " + user.FullName + ". Unfortunately since you're already part of this " +
                                "Household there are no further actions available for this request. Thank you for your time!";
                        }
                    }
                    else
                    {
                        body = "You've been invited to join the Household " + household.Name + " by " + user.FullName + ". Since you appear to not " +
                            "be registered in our database, please click <a href='https://mblyakher-budgeter.azurewebsites.net/Account/Register'>here</a> to first register before being able to accept the invitation.";
                    }
                    //var body = "<p>Email From: <bold>{0}</bold> ({1})</p><p> Message:</p><p>{2}</p>";
                    string from = user.Email;

                    //MailMessage email = new MailMessage(from, ConfigurationManager.AppSettings["emailto"]);
                    MailMessage email = new MailMessage(from, "maxim.blyakher@gmail.com")
                    {
                        Subject = "MyBudgeter Household Invitation",
                        Body = string.Format(body, model.FromName, model.FromEmail, model.Body),
                        IsBodyHtml = true
                    };

                    var svc = new PersonalEmail(); //instantiation to be able to access SendAsync method(it actually sends the email)
                    await svc.SendAsync(email);

                    return RedirectToAction("Index", "Households");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }

                return RedirectToAction("Index");
            }

            return View(invite);
        }

        //// GET: Invites/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Invite invite = db.Invites.Find(id);
        //    if (invite == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", invite.HouseholdId);
        //    return View(invite);
        //}

        //// POST: Invites/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,EmailTo,Created,Code,SentFromId,HouseholdId")] Invite invite)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(invite).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", invite.HouseholdId);
        //    return View(invite);
        //}

        //// GET: Invites/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Invite invite = db.Invites.Find(id);
        //    if (invite == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(invite);
        //}

        //// POST: Invites/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Invite invite = db.Invites.Find(id);
        //    db.Invites.Remove(invite);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
