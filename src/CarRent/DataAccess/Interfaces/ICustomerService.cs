using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarRent.DataAccess.Models;

namespace CarRent.DataAccess.Interfaces
{
    public interface ICustomerService
    {
        Task<PagedResult<Customer>> SearchAsync(string surnameQuery, int pageSize, int pageNumber);

        Task<Customer> GetAsync(string id);

        Task<DataResult> UpdateAsync(Customer customer);

        Task<DataResult<Customer>> CreateAsync(Customer customer, string password);

        Task<DataResult> DeleteAsync(string id);

        Task<DataResult> RentCarAsync(string customerId, long carId);

    }
}
