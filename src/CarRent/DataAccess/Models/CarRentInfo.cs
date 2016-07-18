using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRent.DataAccess.Models
{
    public class CarRentInfo
    {
        public long Id { get; set; }
        public long CarId { get; set; }
        public string CustomerId { get; set; }
        public DateTime RentDate { get; set; }

        public virtual Car Car { get; set; }
    }
}
