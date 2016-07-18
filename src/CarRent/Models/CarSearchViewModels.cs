using CarRent.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRent.Models
{
    public class CarSearchViewModel
    {
        public IEnumerable<CarBrand> Brands { get; set; }
        public IEnumerable<CarModel> Models { get; set; }

        public IEnumerable<Car> Cars { get; set; }
        public int PageCount { get; set; }

        public bool ShowOnlyOverdue { get; set; }

        public long? SelectedBrandId { get; set; }
        public long? SelectedModelId { get; set; }
        public int? Page { get; set; }
    }
}
