using CarRent.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarRent.Models
{
    public class ManageCarViewModel
    {
        private const int _initialYear = 1970;

        public IEnumerable<CarBrand> Brands { get; set; }
        public IEnumerable<CarModel> Models { get; set; }
        public int MinYear { get; set; }
        public int MaxYear { get; set; }

        public long? Id { get; set; }

        [Required(ErrorMessage ="Brand is required")]
        [Display(Name ="Brand")]
        public long BrandId { get; set; }

        [Required(ErrorMessage = "Model is required")]
        [Display(Name = "Model")]
        public long ModelId { get; set; }

        [Required(ErrorMessage = "Year is required")]
        [Display(Name ="Year")]
        [Range(_initialYear, Int32.MaxValue, ErrorMessage ="Year should be in range from 1970 to current year")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Body Type is required")]
        [Display(Name = "Body Type")]
        public string BodyType { get; set; }

        public ManageCarViewModel()
        {
            MinYear = _initialYear;
            MaxYear = DateTime.Today.Year;
            Year = _initialYear;
        }

        public ManageCarViewModel(Car car):this()
        {
            this.Id = car.Id;
            this.BrandId = car.BrandId;
            this.ModelId = car.ModelId;
            this.Year = car.Year;
            this.BodyType = Enum.GetName(typeof(CarBodyTypes), car.BodyType);
            
        }

        public static explicit operator Car(ManageCarViewModel vm)
        {
            return new Car()
            {
                Id = vm.Id.GetValueOrDefault(),
                BrandId = vm.BrandId,
                ModelId = vm.ModelId,
                Year = vm.Year,
                BodyType = (CarBodyTypes)Enum.Parse(typeof(CarBodyTypes), vm.BodyType)
            };
        }

        public static explicit operator ManageCarViewModel(Car c)
        {
            return new ManageCarViewModel(c);
        }
    }

    

}
