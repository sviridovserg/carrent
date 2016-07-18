using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using CarRent.DataAccess.Models;
using CarRent.Auth;

namespace CarRent.Initialize
{
    public class SampleDataInitializer
    {
        private readonly string ADMIN_EMAIL = "admin@carrent.com";
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private ApplicationDbContext _context;

        public SampleDataInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task InitDataAsync()
        {
            try
            {
                _context.Database.Migrate();
            }
            catch (Exception ex) {
            }
            
            await CreateRolesAsync();
            await CreateUsersAsync();
            await AssignAdminRoleToAdmin();
            await CreateCarBrandsAsync();
        }

        private async Task CreateUsersAsync()
        {
            var adminUser = await _userManager.FindByEmailAsync(ADMIN_EMAIL);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser()
                {
                    Email = ADMIN_EMAIL,
                    UserName = ADMIN_EMAIL,
                    Name = "Admin",
                    Surname = "Admin",
                };
                var result = await _userManager.CreateAsync(adminUser, "Qwe_123");
            }
            await CreateCustomersAsync();
        }

        private async Task CreateRolesAsync()
        {
            await CreateAdministratorRoleAsync();
            await CreateUserRoleAsync();
        }

        private async Task AssignAdminRoleToAdmin()
        {
            var adminUser = await _userManager.FindByEmailAsync(ADMIN_EMAIL);
            if (!(await _userManager.IsInRoleAsync(adminUser, Roles.Administrator)))
            {
                await _userManager.AddToRoleAsync(adminUser, Roles.Administrator);
            }
        }

        private async Task CreateUserRoleAsync()
        {
            var userRole = await _roleManager.FindByNameAsync(Roles.User);
            if (userRole == null)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(Roles.User));
            }
        }

        private async Task CreateAdministratorRoleAsync()
        {
            var adminRole = await _roleManager.FindByNameAsync(Roles.Administrator);
            if (adminRole == null)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(Roles.Administrator));
            }
        }

        private async Task CreateCarBrandsAsync()
        {
            await AddBrand(new CarBrand()
            {
                Name = "Audi",
                Models = new CarModel[] {
                    new CarModel()
                    {
                        Name = "A4"
                    },
                    new CarModel()
                    {
                        Name = "A3"
                    },
                    new CarModel()
                    {
                        Name = "Q3"
                    },
                    new CarModel()
                    {
                        Name = "Q7"
                    }
                }
            });
            await AddBrand(new CarBrand()
            {
                Name = "Opel",
                Models = new CarModel[] {
                    new CarModel()
                    {
                        Name = "Astra"
                    },
                    new CarModel()
                    {
                        Name = "Mokka"
                    }
                }
            });
            await AddBrand(new CarBrand()
            {
                Name = "VAZ",
                Models = new CarModel[] {
                    new CarModel()
                    {
                        Name = "Granta"
                    },
                    new CarModel()
                    {
                        Name = "Priora"
                    }
                }
            });
            await AddBrand(new CarBrand()
            {
                Name = "Pegout",
                Models = new CarModel[] {
                    new CarModel()
                    {
                        Name = "107"
                    },
                    new CarModel()
                    {
                        Name = "407"
                    },
                    new CarModel()
                    {
                        Name = "3008"
                    }
                }
            });
        }

        private async Task AddBrand(CarBrand brand)
        {
            var dbBrand = await _context.CarBrands.FirstOrDefaultAsync(b => b.Name == brand.Name);
            if (dbBrand != null)
            {
                return;
            }
            _context.CarBrands.Add(brand);
            _context.SaveChanges();
        }

        private async Task CreateCustomersAsync()
        {
            await CreateCustomer(new Customer() {
                Id = Guid.NewGuid().ToString(),
                UserInfo = new ApplicationUser()
                {
                    Name = "Joe",
                    Surname = "Jonson",
                    Email = "joe@example.com",
                    UserName = "joe@example.com",
                    PhoneNumber = "79954445566"
                }
            });
            await CreateCustomer(new Customer()
            {
                Id = Guid.NewGuid().ToString(),
                UserInfo = new ApplicationUser()
                {
                    Name = "John",
                    Surname = "Doe",
                    Email = "john@example.com",
                    UserName = "john@example.com",
                    PhoneNumber = "78884445566"
                }
            });
        }

        private async Task CreateCustomer(Customer c)
        {
            var user = await _userManager.FindByEmailAsync(c.UserInfo.Email);
            if (user == null)
            {
                var result = await _userManager.CreateAsync(c.UserInfo, "Qwe_123");
                c.ApplicationUserId = c.UserInfo.Id;
                _context.Customers.Add(c);
                await _context.SaveChangesAsync();
                await _userManager.AddToRoleAsync(c.UserInfo, Roles.User);
            }
        }
    }
}
