using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRent.DataAccess.Models
{
    public static class DataErrorCodes
    {
        public const string CustomerExists = "ERR_CUSTOMER_EXISTS";

        public const string CustomerNotFound = "ERR_CUSTOMER_NOT_FOUND";

        public const string CustomerHasRentedCars = "ERR_CUSTOMER_HAS_RENTED_CARS";

        public const string CarNotFound = "ERR_CAR_NOT_FOUND";

        public const string CarIsRented = "ERR_CAR_IS_RENTED";
    }
}
