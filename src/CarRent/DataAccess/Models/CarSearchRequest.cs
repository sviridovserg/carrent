using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRent.DataAccess.Models
{
    public class CarSearchRequest
    {
        public long? BrandId { get; set; }
        public long? ModelId { get; set; }
        public bool OverdueOnly { get; set; }
        public int OverdueDays { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
