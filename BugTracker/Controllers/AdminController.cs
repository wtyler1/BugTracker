using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class AdminController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        UserRoleHelper helper = new UserRoleHelper();
        // GET: Admin
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            List<AdminUserViewModel> userList = new List<AdminUserViewModel>();
            foreach (var user in db.Users.ToList())
            {
                var userCollection = new AdminUserViewModel();
                userCollection.user = user;
                userCollection.role = helper.ListUserRole(user.Id).ToList();
                userList.Add(userCollection);
            }
            return View(userList);
        }
        // Get: Admin/SelectRole/5
        [Authorize(Roles = "Admin")]
        public ActionResult SelectRole(string id)
        {
            var user = db.Users.Find(id);
            var roleUser = new UserRoleViewModel();
            roleUser.user = user;
            roleUser.Id = user.Id;
            roleUser.FirstName = user.FirstName;
            roleUser.LastName = user.LastName;
            roleUser.SelectedRoles = helper.ListUserRole(user.Id).ToArray();
            roleUser.UserRoles = new MultiSelectList(db.Roles, "Name", "Name", roleUser.SelectedRoles);
            return View(roleUser);
        }
        //POST: Admin/SelectRoles/5
      
        [HttpPost]
        public ActionResult SelectRole(UserRoleViewModel model)
        {
            var user = db.Users.Find(model.Id);
            foreach (var rolermv in db.Roles.Select(r => r.Name).ToList())
            { 
                helper.RemoveUserFromRole(user.Id, rolermv); 
            }
            if (model.SelectedRoles != null)
            { 
                foreach (var roleadd in model.SelectedRoles)
                {

                    helper.AddUserToRole(user.Id, roleadd);
                }
            }
            ViewBag.confirm = "User's role has been sucessfully modified";

          
            return RedirectToAction("Index");
        }

    }
}