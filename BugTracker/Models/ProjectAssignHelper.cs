﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class ProjectAssignHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsUserOnProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var user = project.User.Any(u => u.Id == userId);
            return (user);
            
        }
        public void AddUserToProject(string userId, int projectId)
        {
            ApplicationUser user = db.Users.Find(userId);
            Project project = db.Projects.Find(projectId);
            project.User.Add(user);
            db.SaveChanges();
        }

        public void RemoveUserFromProject(string userId,int projectId)
        {
            ApplicationUser user = db.Users.Find(userId);
            Project project = db.Projects.Find(projectId);
            project.User.Remove(user);
            db.SaveChanges();
        }
        public List<Project> ListUserProjects (string userId)
        {
            ApplicationUser user = db.Users.Find(userId);
            return user.Projects.ToList();
        }
        public List<ApplicationUser> ListUsersOnProject(int projectId)
        {
            Project project = db.Projects.Find(projectId);
            return project.User.ToList();
        }

        public List<ApplicationUser> ListUsersNotOnProject(int projectId)
        {
            //Project project = db.Projects.Find(projectId);
            //var userIDs = project.User.Select(u=> u.Id);
            //return db.Users.Where(u => !userIDs.Contains(u.Id)).ToList();
            return db.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToList();
           
        }
    }
}