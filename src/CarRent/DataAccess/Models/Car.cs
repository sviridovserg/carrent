using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRent.DataAccess.Models
{
    public enum CarBodyTypes : int
    {
        Hatch = 1,
        Sedan = 2,
        Wagon = 3,
        SUV = 4,
        Ute = 5,
        Coupe = 6
    }

    public class Car
    {
        public long Id { get; set; }
        public CarBodyTypes BodyType { get; set; }
        public int Year { get; set; }

        public string CustomerId { get; set; }

        public long BrandId { get; set; }
        public long ModelId { get; set; }

        public virtual CarBrand Brand { get; set; }
        public virtual CarModel Model { get; set; }
        public virtual CarRentInfo CarRentInfo { get; set; }
    }

    public class CarBrand
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<CarModel> Models { get; set; }
    }

    public class CarModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long CarBrandId { get; set; }
    }
}
