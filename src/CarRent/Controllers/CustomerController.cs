using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using CarRent.DataAccess.Interfaces;
using CarRent.DataAccess.Models;
using CarRent.Models;
using CarRent.Auth;
using CarRent.Extensions;

namespace CarRent.Controllers
{
    [Authorize(Roles=Roles.Administrator)]
    public class CustomerController : Controller
    {
        private readonly int _pageSize = 10;
        private readonly ICustomerService _customerService;
        private readonly IMessageService _messageService;

        public CustomerController(ICustomerService customerService, IMessageService messageService)
        {
            _customerService = customerService;
            _messageService = messageService;
        }

        private async Task<CustomersListViewModel> SearchAsync(string query, int pageSize, int pageNumber)
        {
            var searchResult = await _customerService.SearchAsync(query, pageSize, pageNumber);
            CustomersListViewModel customersList = new CustomersListViewModel();
            customersList.Customers = searchResult.Result.Select(c => new CustomerViewModel(c)).ToArray();
            customersList.PageCount = Convert.ToInt32(Math.Ceiling((double) searchResult.TotalCount / _pageSize));
            customersList.SearchParams = new CustomerSearchParamsViewModel()
            {
                SurnameQuery = query,
                PageNumber = pageNumber
            };
            return customersList;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string surnameQuery, int? page)
        {
            return View(await SearchAsync(surnameQuery, _pageSize, page.GetValueOrDefault()));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var customer = await _customerService.GetAsync(id);
            if (customer == null)
            {
                return View("Error", new DataError[] { new DataError() { Code = DataErrorCodes.CustomerNotFound, Description = _messageService.GetErrorMessage(DataErrorCodes.CustomerNotFound) } });
            }
            return View(new CustomerViewModel(customer));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _customerService.UpdateAsync((Customer)model);
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

        [HttpGet]
        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerViewModel model)
        {
            if (ModelState.IsValid) {
                var result = await _customerService.CreateAsync((Customer)model, "Qwe_123");
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

        public async Task<IActionResult> Delete(string id)
        {
            var result = await _customerService.DeleteAsync(id);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            return View("Error", result.Errors);
        }

    }
}
