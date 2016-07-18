using CarRent.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRent.DataAccess.Interfaces
{
    public interface ICarService
    {
        Task<PagedResult<Car>> SearchAsync(CarSearchRequest searchParams);

        Task<Car> GetAsync(long id);

        Task<CarBrand[]> GetAllBrandsAsync();

        Task<CarModel[]> GetAllModelsAsync();

        Task<DataResult> UpdateAsync(Car car);

        Task<DataResult> CreateAsync(Car car);

        Task<DataResult> DeleteAsync(long id);
    }
}
