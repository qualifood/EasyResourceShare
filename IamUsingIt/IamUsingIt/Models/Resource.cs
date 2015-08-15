using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IamUsingIt.Models
{
    public class Resource
    {
        //Primary Key
        public int ResourceId { get; set; } 

        //Reservation Relationship
        public ICollection<Reservation> Reservations { get; set; } 

        //Properties
        public string Name { get; set; }
    }
}