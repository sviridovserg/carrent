using CarRent.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRent.DataAccess.Interfaces
{
    public interface IMessageService
    {
        string GetErrorMessage(string errorCode);
    }
}
