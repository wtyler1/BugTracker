using BugTracker.Models;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;

using System.Web;


namespace BugTracker
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        ProjectAssignHelper projecthelper = new ProjectAssignHelper();
        UserRoleHelper helper = new UserRoleHelper();


        // GET: Tickets
        [Authorize(Roles = "Admin,PM,Submitter,Developer")]
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return View(db.Tickets.ToList());
            }

            if (User.IsInRole("PM"))
            { 
            var user = db.Users.Find(User.Identity.GetUserId());
            var project = user.Projects;
            var tickets = project.SelectMany(t => t.Tickets).ToList();             //Ticket relationship in IEmunable so have to convert to list and to bring list objects used SelectMany

            return View(tickets);
            }
            if (User.IsInRole("Developer"))
            {
                var userId = User.Identity.GetUserId();
                var tickets = db.Tickets.Where(u => u.AssignedToUserId == userId);

                return View(tickets);
            }
            if (User.IsInRole("Submitter"))
            {
                var userId = User.Identity.GetUserId();
                var tickets = db.Tickets.Where(u =>u.OwnerUserId == userId );
                return View(tickets);
            }

            return View();
 }

        // GET: Tickets/Details/5
        [Authorize(Roles = "Admin,PM,Developer,Submitter")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);


            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }
 #region Tickets Create
        // GET: Tickets/Create
        [Authorize(Roles ="Submitter")]
        public ActionResult Create()
        {
            var user = User.Identity.GetUserId();
            var Userprojectlist = projecthelper.ListUserProjects(user);

            var Owneruser = db.Users.FirstOrDefault(u => u.Id == user);
            //or
            var Owner = db.Users.Find(user);

            //if (User.IsInRole("Admin") | User.IsInRole("PM"))
            //{
            //}
                ViewBag.ProjectId = new SelectList(Userprojectlist, "Id", "Name");
            //TODO: submitter, Developer can not assign ticket

            //ViewBag.AssignedToUser = db.Users.Where(u => u.FirstName == "UnAssigned");
            
            //TODO: not pulling Full name onto the View might need to use ViewModel
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");

            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                //Bring Full name of User
                ticket.AssignedToUserId = db.Users.FirstOrDefault(u => u.FirstName == "UnAssigned").Id;
                ticket.OwnerUserId = User.Identity.GetUserId();
                
                ticket.Created = DateTime.Now;
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }
        #endregion
    
    #region Tickets Edit
        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        { 
            var roles = db.Roles.FirstOrDefault(r => r.Name == "Developer");
            var userDV = db.Users.Where(u => u.Roles.Any(r => r.RoleId == roles.Id));
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            if (User.IsInRole("PM")| User.IsInRole("Admin"))
            { 
            ViewBag.AssignedToUserId = new SelectList(userDV, "Id", "FirstName",ticket.AssignedToUserId);
            }
            //ViewBag.OwnerUserId = db.Users.Find(User.Identity.GetUserId()).FullName;
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }
        

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Created,Title,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedToUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                // Edit changed the Created and Updated fields
                //db.Entry(ticket).Property("ProjectId").IsModified = false;
                ticket.Update = DateTime.Now;
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }
        #endregion

        #region Ticket Assign
        //[Authorize(Roles = "Admin,PM")]
        //public ActionResult AssignTicket(int id)
        //{

        //    var ticketId = db.Tickets.Find(id);
        //    var roles = db.Roles.FirstOrDefault(r => r.Name == "Developer");
        //    var userDV = db.Users.Where(u => u.Roles.Any(r => r.RoleId == roles.Id));


        //    foreach (var user in db.Users.ToList())
        //    {

        //        var ticketCollection = new ViewModelTicket();
        //        ticketCollection.FirstName = user.FirstName;
        //        ticketCollection.LastName = user.LastName;
        //        //    // TODO: dont understand this 
        //        ticketCollection.SelectedUsers = helper.ListUserRole(user.Id).ToArray();
        //        //ticketCollection.SelectedUsers = helper.ListUserRole(user.Id).ToArray();
        //        ticketCollection.TicketUsersAssigned = new MultiSelectList(userDV, "Id", "FullName", ticketCollection.SelectedUsers);


        //        return View(ticketCollection);
        //    }

        //    return View();
        //}

        ////POST: Projects/AssignUsers
        //[HttpPost]
        // public ActionResult AssignTicket (ViewModelTicket model)
        //{

        //    return RedirectToAction("Index");
        //}

        #endregion

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
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
