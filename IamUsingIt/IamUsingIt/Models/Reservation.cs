using System;
using System.Collections.Generic;
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

        //Properties
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }

    }
}