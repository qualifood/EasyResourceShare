using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IamUsingIt.Models
{
    public class Reservation
    {
        //Primary Key
        public int ReservationId { get; set; }

        //Resource Relationship
        public int ResourceId { get; set; }
        public Resource Resource { get; set; } 

        //UserRelationShip
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }

        //Properties
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime Begin { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime End { get; set; }

        //Unmapped Properties
        [NotMapped]
        public string Username => User.UserName;

        [NotMapped]
        public string ResourceName => Resource.Name;

        [NotMapped]
        [DataType(DataType.Time)]
        public DateTime BeginTime { get; set; }

        [NotMapped]
        [DataType(DataType.Time)]
        public DateTime EndTime { get; set; }
    }
}