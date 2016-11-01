using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Update { get; set; }
        public int ProjectId { get; set; }
        public int TicketTypeId { get; set; }
        public int TicketPriorityId { get; set; }
        public int TicketStatusId { get; set; }
        public string OwnerUserId { get; set; }
        public string AssignedToUserId { get; set; }
        
       
        public Ticket()
        {
            this.TicketAttachments = new HashSet<TicketAttachment>();
            this.TicketComments = new HashSet<TicketComment>();
            this.TicketHistories = new HashSet<TicketHistory>();
            this.TicketNotifications = new HashSet<TicketNotification>();
           
        }

        //Navigational properties for our children
            public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }
            public virtual ICollection<TicketComment> TicketComments { get; set; }
            public virtual ICollection<TicketHistory> TicketHistories { get; set; }
            public virtual ICollection<TicketNotification> TicketNotifications { get; set; }
            
        // Two connections to the User table to track Owner and Assigned Users
            public virtual ApplicationUser OwnerUser { get; set; }
            public virtual ApplicationUser AssignedToUser { get; set; }

        //Navigational properties for our parent
            public virtual Project Project { get; set; }
            public virtual TicketStatus TicketStatus { get; set; }
            public virtual TicketPriority TicketPriorities { get; set; }
            public virtual TicketType TicketType { get; set; }

    }
}