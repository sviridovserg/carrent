using CarRent.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarRent.Models
{
    public class CustomersListViewModel
    {
        public CustomerViewModel[] Customers { get; set; }

        public int PageCount { get; set; }

        public CustomerSearchParamsViewModel SearchParams { get;set; }
    }

    public class CustomerSearchParamsViewModel
    {
        public string SurnameQuery { get; set; }

        [Range(0, Int32.MaxValue, ErrorMessage = "Page number must be more than or equal to 0")]
        public int PageNumber { get; set; }
    }

    public class CustomerViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage ="Name is required")]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surmame is required")]
        [DataType(DataType.Text)]
        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Email Address is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^([0-9]?)?[-. ]?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Phone number")]
        [Display(Name = "Phone")]
        public string PhoneNumber { get; set; }

        public CustomerViewModel()
        { }

        public CustomerViewModel(Customer customer)
        {
            this.Id = customer.Id;
            this.Name = customer.UserInfo.Name;
            this.Surname = customer.UserInfo.Surname;
            this.Email = customer.UserInfo.Email;
            this.PhoneNumber = customer.UserInfo.PhoneNumber;
        }

        public static explicit operator Customer(CustomerViewModel vm)
        {
            return new Customer()
            {
                Id = vm.Id,
                UserInfo = new ApplicationUser()
                {
                    Name = vm.Name,
                    Surname = vm.Surname,
                    Email = vm.Email,
                    PhoneNumber = vm.PhoneNumber
                }
            };
        }

        public static explicit operator CustomerViewModel(Customer c)
        {
            return new CustomerViewModel(c);
        }
    }
}
