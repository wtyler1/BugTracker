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

namespace BugTracker
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        ProjectAssignHelper phelper = new ProjectAssignHelper();
        UserRoleHelper helper = new UserRoleHelper();
        // GET: Projects
        [Authorize]
        public ActionResult Index()

        {
            if (User.IsInRole("Admin") | User.IsInRole("PM"))
            {

               return View(db.Projects.ToList());
             }
            else
            {
                var projList = phelper.ListUserProjects(User.Identity.GetUserId());
              return View(projList);
            }
        }
        //GET: Projects/AssignUsers
        [Authorize(Roles ="Admin,PM")]
        public ActionResult AssignUsers(int id)
        {
            var projectId = db.Projects.Find(id);
            foreach (var user in db.Users.ToList())
            {
               
                var projectCollection = new AssignViewModel();
                projectCollection.FirstName = user.FirstName;
                projectCollection.ProjectName = projectId.Name;
                projectCollection.LastName = user.LastName;
                projectCollection.SelectedUsers = helper.ListUserRole(user.Id).ToArray();
                projectCollection.UsersAssigned = new MultiSelectList(db.Users, "Id", "FullName", projectCollection.SelectedUsers);


            return View(projectCollection);
            }

            return View();
        }

        //POST: Projects/AssignUsers
        [HttpPost]
        // model not bring the full list of users to remove
        public ActionResult AssignUsers(AssignViewModel model)
        {
            //remove Users to Project
            foreach (var usersremove in db.Users.ToList())
            {
                //phelper.RemoveUserFromProject(usersremove, Convert.ToInt32(model.Id));
                phelper.RemoveUserFromProject(usersremove.Id, model.Id);
            }
            if (model.SelectedUsers!=null)
            { 
                    //TODO: if statement for if none are selected
                    foreach (var useradd in model.SelectedUsers)
                    {
                        //phelper.AddUserToProject(useradd, Convert.ToInt32(model.Id));
                        phelper.AddUserToProject(useradd, model.Id);

                    }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AssignUsersRemove (AssignViewModel model)
        {
            foreach (var usersremove in model.SelectedUsers)
            {
                //phelper.RemoveUserFromProject(usersremove, Convert.ToInt32(model.Id));
                phelper.RemoveUserFromProject(usersremove, model.Id);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
         public ActionResult AssignUsersAdd(AssignViewModel model)
        {
            foreach (var useradd in model.SelectedUsers)
            {
                //phelper.AddUserToProject(useradd, Convert.ToInt32(model.Id));
                phelper.AddUserToProject(useradd, model.Id);

            }
            return RedirectToAction("Index");
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles ="Admin,PM")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Created")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.Created = DateTime.Now;
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin,PM")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Created")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
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
