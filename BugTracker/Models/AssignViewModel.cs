using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class AssignViewModel
    {
        
        public ApplicationUser user { get; set; }
        public List<string> role { get; set; }
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public MultiSelectList UsersAssigned { get; set; }
        public string[] SelectedUsers { get; set; }

    }
}