using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;
using CarRent.Auth;
using CarRent.DataAccess.Interfaces;
using CarRent.Models;
using CarRent.Extensions;
using CarRent.DataAccess.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CarRent.Controllers
{
    [Authorize(Roles = Roles.Administrator)]
    public class CarManageController : Controller
    {
        private readonly ICarService _carService;
        private readonly IMessageService _messageService;
        private readonly int _pageSize = 10;
        private readonly int _overdueDays = 14;

        public CarManageController(ICarService carService, IMessageService messageService)
        {
            _carService = carService;
            _messageService = messageService;
        }

        private bool IsYearValid(ManageCarViewModel model)
        {
            if (model.Year < 1970 || model.Year > DateTime.Now.Year)
            {
                ModelState.AddModelError("", "Year should be in range from 1970 to current year");
                return false;
            }
            return true;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index(long? selectedBrandId, long? selectedModelId, bool showOnlyOVerdue, int? page)
        {
            
            var searchResult = await _carService.SearchAsync(
                new CarSearchRequest() {
                    BrandId = selectedBrandId,
                    ModelId = selectedModelId,
                    OverdueOnly = showOnlyOVerdue,
                    OverdueDays = _overdueDays,
                    PageSize = _pageSize,
                    PageNumber = page.GetValueOrDefault()
                });
            return View(
                new CarSearchViewModel()
                {
                    Cars = searchResult.Result,
                    PageCount = Convert.ToInt32(Math.Ceiling((double)searchResult.TotalCount / _pageSize)),
                    Brands = await _carService.GetAllBrandsAsync(),
                    Models = await _carService.GetAllModelsAsync(),
                    SelectedBrandId = selectedBrandId,
                    SelectedModelId = selectedModelId,
                    ShowOnlyOverdue = showOnlyOVerdue,
                    Page = page.GetValueOrDefault()
                });
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(new ManageCarViewModel() {
                Brands = await _carService.GetAllBrandsAsync(),
                Models = await _carService.GetAllModelsAsync(),
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ManageCarViewModel model)
        {
            if (ModelState.IsValid || IsYearValid(model))
            {
                var result = await _carService.CreateAsync((Car)model);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    this.AddErrorFromResult(result);
                }
            }
            model.Brands = await _carService.GetAllBrandsAsync();
            model.Models = await _carService.GetAllModelsAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _carService.DeleteAsync(id);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            return View("Error", result.Errors);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var car = await _carService.GetAsync(id);
            if (car == null)
            {
                return View("Error", new DataError[] { new DataError() { Code = DataErrorCodes.CarNotFound, Description = _messageService.GetErrorMessage(DataErrorCodes.CarNotFound) } });
            }
            var viewModel = new ManageCarViewModel(car);
            viewModel.Brands = await _carService.GetAllBrandsAsync();
            viewModel.Models = await _carService.GetAllModelsAsync();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ManageCarViewModel model)
        {
            if (ModelState.IsValid || IsYearValid(model))
            {
                var result = await _carService.UpdateAsync((Car)model);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    this.AddErrorFromResult(result);
                }
            }

            return View(model);
        }
    }
}
