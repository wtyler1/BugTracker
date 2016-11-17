using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Threading.Tasks;

namespace BugTracker
{
    public class TicketAttachmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ImageUploadValidator validator = new ImageUploadValidator();

        // GET: TicketAttachments
        [Authorize(Roles = "Admin,PM,Submitter,Developer")]
        public ActionResult Index()
        {
            var ticketAttachments = db.TicketAttachments.Include(t => t.User);
            return View(ticketAttachments.ToList());
        }

        // GET: TicketAttachments/Details/5
        [Authorize(Roles = "Admin,PM,Submitter,Developer")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Create
        //public ActionResult Create(int? id)
        //{
        //    ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");
        //    return View();
        //}

        // POST: TicketAttachments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,PM,Submitter,Developer")]
        public async Task<ActionResult> Create([Bind(Include = "Id,TicketId,Description,FileURL")] TicketAttachment ticketAttachment, HttpPostedFileBase Image)
        {
            var tOwner = db.Tickets.FirstOrDefault(t => t.Id == ticketAttachment.TicketId).OwnerUser.FullName;

            if (ModelState.IsValid)
            {
                //Add image
                if (validator.IsWebFriendlyImage(Image))
                {
                    var filename = Path.GetFileName(Image.FileName);
                    Image.SaveAs(Path.Combine(Server.MapPath("~/img/uploads/"), filename));
                   ticketAttachment.FileURL = "~/img/uploads/" + filename;
                }

                ticketAttachment.UserId = User.Identity.GetUserId();
                ticketAttachment.Created = DateTime.Now;
                db.TicketAttachments.Add(ticketAttachment);
                db.SaveChanges();
                //TODO: Add TicketNotifications for Attachments
                var svc2 = new EmailService();
                var msg2 = new IdentityMessage();
                var newdev = db.Tickets.FirstOrDefault(t => t.Id == ticketAttachment.TicketId).AssignedToUser.Email;


                var newdevname = db.Tickets.FirstOrDefault(t => t.Id == ticketAttachment.TicketId).AssignedToUser.FullName;
                msg2.Destination = newdev;
                msg2.Subject = "BugTracker";
                msg2.Body =" An attachment has been added to " + tOwner + "'s ticket. ";

                await svc2.SendAsync(msg2);

                return RedirectToAction("Details","Tickets", new {id = ticketAttachment.TicketId });
                //return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketAttachment.UserId);
            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Edit/5
        [Authorize(Roles = "Admin,PM,Submitter,Developer")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketAttachment.UserId);
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TicketId,FilePath,Description,Created,UserId,FileURL")] TicketAttachment ticketAttachment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketAttachment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Tickets", new { id = ticketAttachment.TicketId });
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketAttachment.UserId);
            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Delete/5
        [Authorize(Roles = "Admin,PM,Submitter,Developer")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            db.TicketAttachments.Remove(ticketAttachment);
            db.SaveChanges();
            return RedirectToAction("Index", "Tickets");
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
