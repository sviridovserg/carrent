using CarRent.DataAccess.Interfaces;
using CarRent.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using CarRent.DataAccess.Extensions;

namespace CarRent.DataAccess
{
    internal class CarService: ICarService
    {
        private ApplicationDbContext _context;
        private IMessageService _messageService;

        public CarService(ApplicationDbContext context, IMessageService messageService)
        {
            _context = context;
            _messageService = messageService;
        }

        public async Task<DataResult> CreateAsync(Car car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return DataResult.Succeed();
        }

        public Task<Car> GetAsync(long id)
        {
            return _context.Cars.Where(c => c.Id == id).Include(c => c.Brand).Include(c => c.Model).FirstOrDefaultAsync();
        }

        public Task<CarBrand[]> GetAllBrandsAsync()
        {
            return _context.CarBrands.ToArrayAsync();
        }

        public Task<CarModel[]> GetAllModelsAsync()
        {
            return _context.CarModels.ToArrayAsync();
        }

        public async Task<DataResult> UpdateAsync(Car car)
        {
            var dbCar = await GetAsync(car.Id);
            if (dbCar == null)
            {
                return DataResult.Failed(GetDataError(DataErrorCodes.CarNotFound));
            }
            dbCar.BodyType = car.BodyType;
            dbCar.BrandId = car.BrandId;
            dbCar.ModelId = car.ModelId;
            dbCar.Year = car.Year;
            _context.Cars.Update(dbCar);
            await _context.SaveChangesAsync();
            return DataResult.Succeed();
        }

        public Task<PagedResult<Car>> SearchAsync(CarSearchRequest searchParams)
        {
            return Task.Run(() =>
            {
                var totalCount = _context.Cars.Count();
                var nowDate = DateTime.Today;
                var result = _context.Cars
                    .Include(c => c.Brand)
                    .Include(c => c.Model)
                    .Include(c => c.CarRentInfo)
                    //Done that way because ef7 fails inside on where, orderby, etc
                    .AddFilter(!searchParams.BrandId.HasValue ? null : new Func<Car, bool>(c => c.Brand.Id == searchParams.BrandId))
                    .AddFilter(!searchParams.ModelId.HasValue ? null : new Func<Car, bool>(c => c.Model.Id == searchParams.ModelId))
                    .AddFilter(!searchParams.OverdueOnly ? null : new Func<Car, bool>(c => c.CarRentInfo != null && (nowDate - c.CarRentInfo.RentDate).TotalDays > searchParams.OverdueDays))
                    .OrderBy(c => c.Brand.Name)
                    .Skip(searchParams.PageNumber * searchParams.PageSize)
                    .Take(searchParams.PageSize).ToArray();
                return new PagedResult<Car>()
                {
                    Result = result,
                    TotalCount = totalCount
                };
            });
        }

        public async Task<DataResult> DeleteAsync(long id)
        {
            var car = await this.GetAsync(id);
            if (car == null)
            {
                return DataResult.Failed(GetDataError(DataErrorCodes.CarNotFound));
            }
            if (!string.IsNullOrEmpty(car.CustomerId))
            {
                return DataResult.Failed(GetDataError(DataErrorCodes.CarIsRented));
            }
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            return DataResult.Succeed();
        }

        private DataError GetDataError(string errorCode)
        {
            return new DataError() { Code = errorCode, Description = _messageService.GetErrorMessage(errorCode) };
        }

    }
}
