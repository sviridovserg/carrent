using CarRent.DataAccess.Interfaces;
using CarRent.DataAccess.Models;
using System;

namespace CarRent.DataAccess
{
    class MessageService: IMessageService
    {
        public string GetErrorMessage(string errorCode)
        {
            switch (errorCode)
            {
                case DataErrorCodes.CustomerExists:
                    return "Customer with the same Email Adress is already exists";
                case DataErrorCodes.CustomerNotFound:
                    return "Customer was not found";
                case DataErrorCodes.CustomerHasRentedCars:
                    return "Customer has rented cars";
                case DataErrorCodes.CarIsRented:
                    return "Car is currently rented";
                case DataErrorCodes.CarNotFound:
                    return "Car was not found";
            }
            throw new ArgumentException("Invalid error code", "errorCode");
        }
    }
}
