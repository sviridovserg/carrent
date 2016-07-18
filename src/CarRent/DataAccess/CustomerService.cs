using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using CarRent.DataAccess.Interfaces;
using CarRent.DataAccess.Models;
using Microsoft.Data.Entity;
using CarRent.Auth;
using CarRent.DataAccess.Extensions;

namespace CarRent.DataAccess
{
    internal class CustomerService : ICustomerService
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private ICarService _carService;
        private IMessageService _messageService;

        public CustomerService(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ICarService carService, IMessageService messageService)
        {
            _userManager = userManager;
            _context =context;
            _carService = carService;
            _messageService = messageService;
        }

        public async Task<DataResult<Customer>> CreateAsync(Customer customer, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == customer.UserInfo.Email);
            if (user != null)
            {
                return DataResult<Customer>.Failed(GetDataError(DataErrorCodes.CustomerExists));
            }
            var applicationUser = customer.UserInfo;
            applicationUser.UserName = applicationUser.Email;
            var result = await _userManager.CreateAsync(applicationUser, password );
            if (!result.Succeeded)
            {
                return DataResult<Customer>.Failed(ConvertErrors(result.Errors));
            }
            customer.Id = Guid.NewGuid().ToString();
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            await _userManager.AddToRoleAsync(customer.UserInfo, Roles.User);
            return DataResult<Customer>.Succeed(customer);
        }

        public Task<Customer> GetAsync(string id)
        {
             //return Task.Run(() =>  _context.Customers.Include(c => c.UserInfo).Where(c => c.Id == id).FirstOrDefault());
            //Fails with error inside Ef
            return _context.Customers.Include(c => c.UserInfo).Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<DataResult> UpdateAsync(Customer customer)
        {
            var user = await GetAsync(customer.Id);
            if (user == null)
            {
                return DataResult.Failed(GetDataError(DataErrorCodes.CustomerNotFound));
            }
            user.UserInfo.Name = customer.UserInfo.Name;
            user.UserInfo.Surname = customer.UserInfo.Surname;
            user.UserInfo.Email = customer.UserInfo.Email;
            user.UserInfo.PhoneNumber = customer.UserInfo.PhoneNumber;
            _context.Users.Update(user.UserInfo);
            await _context.SaveChangesAsync();
            return DataResult.Succeed();
        }

        public Task<PagedResult<Customer>> SearchAsync(string surnameQuery, int pageSize, int pageNumber)
        {
            return Task.Run(() =>
            {
                var totalCount = _context.Customers.Count();
                if (!string.IsNullOrEmpty(surnameQuery))
                {
                    surnameQuery = surnameQuery.ToUpperInvariant();
                }
                var result = _context.Customers
                    .Include(c => c.UserInfo)
                    //Done that way because ef7 fails inside on where, orderby, etc
                    //.ToList()
                    .AddFilter(string.IsNullOrEmpty(surnameQuery) ? null : new Func<Customer, bool>(c => c.UserInfo.Surname.ToUpperInvariant().Contains(surnameQuery)))
                    .OrderBy(c => c.UserInfo.Surname)
                    .Skip(pageNumber*pageSize)
                    .Take(pageSize).ToArray();
                return new PagedResult<Customer>()
                {
                    Result = result,
                    TotalCount = totalCount
                };
            });
        }

        public async Task<DataResult> DeleteAsync(string id)
        {
            var customer = await this.GetAsync(id);
            if (customer == null)
            {
                return DataResult.Failed(GetDataError(DataErrorCodes.CustomerNotFound));
            }
            var customerRentedCars = _context.Customers.Include(c=>c.RentedCars).Where(c => c.Id==id).First();
            if (customerRentedCars.RentedCars != null && customerRentedCars.RentedCars.Count > 0)
            {
                return DataResult.Failed(GetDataError(DataErrorCodes.CustomerHasRentedCars));
            }
            await _userManager.RemoveFromRoleAsync(customer.UserInfo, Roles.User);
            _context.Customers.Remove(customer);
            _context.Users.Remove(customer.UserInfo);
            await _context.SaveChangesAsync();
            return DataResult.Succeed();
        }

        public async Task<DataResult> RentCarAsync(string appUserId, long carId)
        {
            var customer = await _context.Customers.Include(c => c.RentedCars).Where(c => c.ApplicationUserId == appUserId).FirstOrDefaultAsync();
            if (customer == null)
            {
                return DataResult.Failed(GetDataError(DataErrorCodes.CustomerNotFound));
            }
            var car = await _carService.GetAsync(carId);
            if (car == null)
            {
                return DataResult.Failed(GetDataError(DataErrorCodes.CarNotFound));
            }
            customer.RentedCars.Add(new CarRentInfo() {
                CarId = carId,
                CustomerId = customer.Id,
                RentDate = DateTime.Today
            });
            car.CustomerId = customer.Id;
            _context.Customers.Update(customer);
            _context.Cars.Update(car);
            await _context.SaveChangesAsync();
            return DataResult.Succeed();
        }

        private DataError GetDataError(string errorCode)
        {
            return new DataError() { Code = errorCode, Description = _messageService.GetErrorMessage(errorCode) };
        }

        private DataError[] ConvertErrors(IEnumerable<IdentityError> errors)
        {
            return errors.Select(e => new DataError() { Code = e.Code, Description = e.Description }).ToArray();
        }
    }
}
