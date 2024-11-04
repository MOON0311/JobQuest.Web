using JobQuest.Web.Data;
using JobQuest.Web.Models;
using JobQuest.Web.Strategy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace JobQuest.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public AccountController(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        // Register Page
        public IActionResult Register()
        {
            return View();
        }

        // RegisterForAdmin Page
        public IActionResult RegisterForAdmin()
        {
            return View();
        }

        // Account Registration Logic
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Registration model)
        {
            if (ModelState.IsValid)
            {
                int? RoleId = HttpContext.Session.GetInt32("RoleId");

                // Check if the username already exists
                User? usedUserName = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.UserName == model.UserName);

                // Check if the email already exists
                User? usedEmail = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (usedUserName != null)
                {
                    // Add a ModelState error if the username is already taken
                    ModelState.AddModelError("UserName", "Username taken.");
                }

                if (usedEmail != null)
                {
                    // Add a ModelState error if the email is already in use
                    ModelState.AddModelError("Email", "Email registered.");
                }

                if (usedUserName != null || usedEmail != null)
                {
                    return View(model);
                }

                // Validate FirstName
                bool isFirstNameValid = model.ValidateName(model.FirstName, "FirstName", ModelState);

                // Validate LastName
                bool isLastNameValid = model.ValidateName(model.LastName, "LastName", ModelState);

                // Check if the model state is valid
                if (!ModelState.IsValid)
                {
                    return View(model); // Return the view with the current model to show validation errors
                }

                User user = new User
                {
                    UserName = model.UserName,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Status = "Active",
                    RoleId = model.RoleId
                };
                    
                user.SetPassword(model.Password);
                
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();

                if (RoleId != 1)
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    return RedirectToAction("List", "Account");
                }
            }

            return View(model);
        }

        // Login Page
        public IActionResult Login()
        {
            return View();
        }

        // Login Authentication Logic
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login model)
        {
            if (ModelState.IsValid)
            {
                User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName);

                if (user != null && user.CheckPassword(model.Password))
                {
                    if (user.Status == "Active")
                    {
                        // Set session variable to indicate the user is logged in
                        HttpContext.Session.SetString("IsLoggedIn", "true");
                        HttpContext.Session.SetInt32("RoleId", user.RoleId);
                        HttpContext.Session.SetInt32("UserId", user.UserId);

                        switch (user.RoleId)
                        {
                            case 1: // Assuming RoleId 1 is for Admin
                                return RedirectToAction("List", "Account");

                            case 2: // Assuming RoleId 2 is for HR
                                return RedirectToAction("ViewPostedJobs", "JobPost");

                            case 3: // Assuming RoleId 3 is for JobSeeker
                                return RedirectToAction("Index", "JobPost");

                            default:
                                // If an unknown role is detected, redirect to a default page
                                return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Error", "Home");
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }

        // Logout Logic
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            // Clear the session variable
            HttpContext.Session.Remove("IsLoggedIn");
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("RoleId");

            return RedirectToAction("Index", "Home");
        }

        // Retrieve Account List
        [HttpGet]
        public async Task<IActionResult> List(string searchTerm, int? role)
        {
            // Check if the user is logged in and has the admin role
            string? isLoggedIn = HttpContext.Session.GetString("IsLoggedIn");
            int? roleId = HttpContext.Session.GetInt32("RoleId");

            if (isLoggedIn != "true" || roleId != 1)
            {
                return RedirectToAction("Login", "Account");
            }

            IQueryable<User> filter = _dbContext.Users.Where(u => u.RoleId != 1);

            // Apply username Search
            UsernameSearchStrategy usernameSearch = new UsernameSearchStrategy(searchTerm);
            filter = usernameSearch.Search(filter, searchTerm);

            if (role.HasValue)
            {
                RoleSearchStrategy roleSearch = new RoleSearchStrategy(role.Value); // Pass the int value
                filter = roleSearch.Search(filter, null); // Pass null as searchTerm is not used here
            }

            // Retrieve filtered users
            var users = await filter
                .Select(u => new UserViewModel
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Status = u.Status,
                    RoleName = u.RoleId == 3 ? "JobSeeker" : (u.RoleId == 2 ? "HR" : "Error") // Set RoleName based on RoleId
                })
                .ToListAsync();

            return View(users);
        }

        // Retrieve Account Information to Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _dbContext.Users
                .Where(u => u.UserId == id) // Exclude Admins
                .Select(u => new UserViewModel
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Status = u.Status,
                    RoleName = u.RoleId == 3 ? "JobSeeker" : (u.RoleId == 2 ? "HR" : "Error") // Set RoleName based on RoleId
                })
                .FirstOrDefaultAsync();

            return View(user);
        }

        // Update Edited Account Information
        [HttpPost]
        public async Task<IActionResult> Edit(User viewModel)
        {
            User? user = await _dbContext.Users.FindAsync(viewModel.UserId);

            if (user != null)
            {
                user.Status = viewModel.Status;

                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List", "Account");
        }

        // Delete User
        [HttpPost]
        public async Task<IActionResult> Delete(User viewModel)
        {
            User? user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == viewModel.UserId);

            if (user != null)
            {
                _dbContext.Users.Remove(viewModel);
                await _dbContext.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "User Deleted!";

            return RedirectToAction("List", "Account");
        }

    }
}
