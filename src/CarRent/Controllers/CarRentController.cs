using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using CarRent.Models;
using CarRent.DataAccess.Interfaces;
using CarRent.DataAccess.Models;
using Microsoft.AspNet.Authorization;
using System.Security.Claims;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CarRent.Controllers
{
    [Authorize]
    public class CarRentController : Controller
    {
        private readonly ICarService _carService;
        private readonly ICustomerService _customerService;
        private readonly int _pageSize = 10;

        public CarRentController(ICarService carService, ICustomerService customerService)
        {
            _carService = carService;
            _customerService = customerService;
        }
        
        [AllowAnonymous]
        public async Task<IActionResult> Index(long? selectedBrandId, long? selectedModelId, int? page)
        {
            var searchResult = await _carService.SearchAsync(
                 new CarSearchRequest()
                 {
                     BrandId = selectedBrandId,
                     ModelId = selectedModelId,
                     OverdueOnly = false,
                     PageSize = _pageSize,
                     PageNumber = page.GetValueOrDefault()
                 });
            int pc = Convert.ToInt32(Math.Ceiling((double)searchResult.TotalCount / _pageSize));
            return View(
                new CarSearchViewModel()
                {
                    Cars = searchResult.Result,
                    PageCount = Convert.ToInt32(Math.Ceiling((double)searchResult.TotalCount / _pageSize)),
                    Brands = await _carService.GetAllBrandsAsync(),
                    Models = await _carService.GetAllModelsAsync(),
                    SelectedBrandId = selectedBrandId,
                    SelectedModelId = selectedModelId,
                    Page = page.GetValueOrDefault()
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rent(long carId)
        {
            var appUserId = User.GetUserId();
            
            var result =  await _customerService.RentCarAsync(appUserId, carId) ;
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            return View("Error", result.Errors);
        }
    }
}
