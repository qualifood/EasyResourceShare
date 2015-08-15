using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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

        //Calculated Properties
        [NotMapped]
        public bool Free { get; set; }
        [NotMapped]
        public string CurrentUser { get; set; }
    }
}