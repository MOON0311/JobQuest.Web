using JobQuest.Web.Data;
using JobQuest.Web.Factory;
using JobQuest.Web.Models;
using JobQuest.Web.Strategy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JobQuest.Web.Controllers
{
    public class JobPostController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public JobPostController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Return Index page, also used for refresh list when search
        public async Task<IActionResult> Index(string searchTerm, string jobType, string salaryType, int? minSalary, int? maxSalary)
        {
            string? isLoggedIn = HttpContext.Session.GetString("IsLoggedIn");
            int? roleId = HttpContext.Session.GetInt32("RoleId");

            if (isLoggedIn != "true")
            {
                return RedirectToAction("Login", "Account");
            }

            IQueryable<JobPost> jobPosts = _dbContext.JobPosts.AsQueryable();

            // If not Admin
            if (roleId != 1)
            {
                jobPosts = jobPosts.Where(j => j.IsActive);
            }

            // Search bar filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                IQueryable<JobPost> titleSearch = new TitleSearchStrategy().Search(jobPosts, searchTerm);
                IQueryable<JobPost> descriptionSearch = new DescriptionSearchStrategy().Search(jobPosts, searchTerm);

                jobPosts = titleSearch.Union(descriptionSearch);
            }

            // Job type filter
            if (!string.IsNullOrEmpty(jobType))
            {
                jobPosts = new JobTypeSearchStrategy(jobType).Search(jobPosts, null);
            }

            // Salary type filter
            if (!string.IsNullOrEmpty(salaryType))
            {
                jobPosts = new SalaryTypeSearchStrategy(salaryType).Search(jobPosts, null);
            }

            // Salary filter
            SalarySearchStrategy salarySearch = new SalarySearchStrategy(minSalary, maxSalary);
            jobPosts = salarySearch.Search(jobPosts, null);

            List<JobPost> filteredJobPosts = await jobPosts.ToListAsync();

            return View(filteredJobPosts);
        }


        // Add JobPost Page
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        // Add new JobPost
        [HttpPost]
        public async Task<IActionResult> Add(JobPost viewModel)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            // Manual validation for Salary
            if (viewModel.SalaryType == "/Hour")
            {
                if (viewModel.Salary < 24)
                {
                    ModelState.AddModelError("Salary", "Hourly salary must be at least $24.");
                }
            }
            else if (viewModel.SalaryType == "/Annual")
            {
                if (viewModel.Salary < 47000)
                {
                    ModelState.AddModelError("Salary", "Annually salary must be at least $47000.");
                }
            }
            
            // Check any ModelError
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Create JobPost with Factory
            JobPost jobPost = JobPostFactory.CreateJobPost(viewModel, userId.Value);
            
            await _dbContext.JobPosts.AddAsync(jobPost);

            await _dbContext.SaveChangesAsync();

            // Return Temporaray Data for alert
            TempData["SuccessMessage"] = "Job Posted Successfully!";

            return RedirectToAction("ViewPostedJobs", "JobPost");
        }

        // HR ViewPostedJobs page, also use for search
        public async Task<IActionResult> ViewPostedJobs(string searchTerm, string jobType)
        {
            string? isLoggedIn = HttpContext.Session.GetString("IsLoggedIn");
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? userId = HttpContext.Session.GetInt32("UserId");

            Console.WriteLine($"IsLoggedIn: {isLoggedIn}, RoleId: {roleId}, UserId: {userId}");

            if (isLoggedIn != "true" || roleId != 2)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get HRId
            int? hrId = userId.Value;

            // Retrieve posted jobs according to HRId
            IQueryable<JobPost> jobPosts = _dbContext.JobPosts
                .Where(j => j.UserId == hrId);

            // Apply different search with strategy (title and description)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                IQueryable<JobPost> titleSearch = new TitleSearchStrategy().Search(jobPosts, searchTerm);
                IQueryable<JobPost> descriptionSearch = new DescriptionSearchStrategy().Search(jobPosts, searchTerm);

                jobPosts = titleSearch.Union(descriptionSearch);
            }

            // Apply job type filter
            if (!string.IsNullOrEmpty(jobType))
            {
                jobPosts = jobPosts.Where(j => j.JobType == jobType);
            }

            List<JobPost> jobPostList = await jobPosts.ToListAsync();

            // Convert to ViewModel
            var viewModel = jobPostList.Select(j => new JobPostViewModel
            {
                JobPostId = j.JobPostId,
                Title = j.Title,
                CompanyName = j.CompanyName,
                Description = j.Description,
                JobType = j.JobType,
                Location = j.Location,
                Salary = j.Salary,
                SalaryType = j.SalaryType,
                PostedDate = j.PostedDate,
                IsActive = j.IsActive,
            }).ToList();

            return View(viewModel);
        }

        // Retrieve Applicants for the jobPost
        public async Task<IActionResult> ViewApplicants(int jobPostId)
        {
            string? isLoggedIn = HttpContext.Session.GetString("IsLoggedIn");
            int? roleId = HttpContext.Session.GetInt32("RoleId");

            // Check if the user is logged in and is an HR
            if (isLoggedIn != "true" || roleId != 2)
            {
                return RedirectToAction("Login", "Account");
            }

            // Retrieve applications for the specified job post ID
            var applications = await (from a in _dbContext.Applications
                                      join u in _dbContext.Users on a.UserId equals u.UserId
                                      where a.JobPostId == jobPostId
                                      select new ApplicationViewModel
                                      {
                                          ApplicationId = a.ApplicationId,
                                          ApplicantId = a.UserId,
                                          ApplicantName = u.FirstName,
                                          Resume = a.ResumeFilePath,
                                          ApplicationDate = a.ApplicationDate,
                                          Status = a.Status
                                      }).ToListAsync();


            return View(applications); // Return the view with the applications
        }

        // Retrieve Information for the JobPost to Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            JobPost jobPost = await _dbContext.JobPosts.FindAsync(id);

            return View(jobPost);
        }

        // Update the Edited Information
        [HttpPost]
        public async Task<IActionResult> Edit(JobPost viewModel)
        {
            JobPost? jobPost = await _dbContext.JobPosts.FindAsync(viewModel.JobPostId);

            // Manual validation for Salary
            if (viewModel.Salary < 24)
            {
                ModelState.AddModelError("Salary", "Salary must be at least 24.");
            }

            // Check any ModelError
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            if (jobPost != null)
            {
                jobPost.Title = viewModel.Title;
                jobPost.CompanyName = viewModel.CompanyName;
                jobPost.Description = viewModel.Description;
                jobPost.JobType = viewModel.JobType;
                jobPost.Location = viewModel.Location;
                jobPost.Salary = viewModel.Salary;

                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("ViewPostedJobs", "JobPost");
        }

        // Retrieve Information for the Post to Edit Status (Admin)
        [HttpGet]
        public async Task<IActionResult> EditStatus(int id)
        {
            JobPost? jobPost = await _dbContext.JobPosts.FindAsync(id);

            return View(jobPost);
        }

        // Update the jobPost status (Admin)
        [HttpPost]
        public async Task<IActionResult> EditStatus(JobPost viewModel)
        {
            JobPost? jobPost = await _dbContext.JobPosts.FindAsync(viewModel.JobPostId);

            if (jobPost != null)
            {
                jobPost.IsActive = viewModel.IsActive;

                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Index", "JobPost");
        }

        // Delete the jobPost
        [HttpPost]
        public async Task<IActionResult> Delete(JobPost viewModel)
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");

            JobPost? jobPost = await _dbContext.JobPosts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.JobPostId == viewModel.JobPostId);

            if (jobPost != null)
            {
                _dbContext.JobPosts.Remove(viewModel);
                await _dbContext.SaveChangesAsync();
            }

            if (roleId == 1)
            {
                TempData["SuccessMessage"] = "Post Deleted!";

                return RedirectToAction("Index", "JobPost");
            }
            else if (roleId == 2)
            {
                TempData["SuccessMessage"] = "Post Deleted!";

                return RedirectToAction("ViewPostedJobs", "JobPost");
            }

            return RedirectToAction("Error", "Shared");
        }

    }
}
