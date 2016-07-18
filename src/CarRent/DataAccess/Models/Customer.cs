using CarRent.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarRent.DataAccess.Models
{
    public class Customer
    {
        public virtual string Id { get; set; }

        public string ApplicationUserId { get; set; }

        public ApplicationUser UserInfo { get; set; }

        public virtual ICollection<CarRentInfo> RentedCars { get; set; }

    }
}
