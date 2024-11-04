using Microsoft.AspNetCore.Mvc;
using JobQuest.Web.Models;
using JobQuest.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace JobQuest.Web.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public ApplicationController(ApplicationDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        // Retrieve JobPost Information
        [HttpGet]
        public async Task<IActionResult> Add(int id)
        {
            JobPost? jobPost = await _dbContext.JobPosts.FindAsync(id);

            if (jobPost == null)
            {
                return NotFound();
            }

            ApplicationForm model = new ApplicationForm
            {
                JobPost = jobPost,
                Application = new Application()
            };

            return View(model);
        }

        // Upload Resume
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ApplicationForm viewModel)
        {
            // Model Validation
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Resume Validation
            if (!viewModel.ResumeValidation())
            {
                ModelState.AddModelError(string.Empty, "Only PDF files are allowed.");
                return View(viewModel);
            }

            // Retrieve the JobPost from the database using JobPostId
            JobPost? jobPost = await _dbContext.JobPosts.FindAsync(viewModel.JobPost.JobPostId);
            if (jobPost == null)
            {
                return NotFound();
            }

            // Retrieve the UserId
            int? userId = HttpContext.Session.GetInt32("UserId");

            User? user = await _dbContext.Users.FindAsync(userId);

            // Check application history
            Application? applied = await _dbContext.Applications
                .FirstOrDefaultAsync(a => a.JobPostId == jobPost.JobPostId && a.UserId == userId);

            if (applied != null)
            {
                ModelState.AddModelError(string.Empty, "Application existed! Please check applications page.");
                return View(viewModel);
            }

            string resumePath = null;

            // Check file existance
            if (viewModel.ResumeFile != null)
            {
                // Upload file and get path
                resumePath = await viewModel.Upload(viewModel.ResumeFile);
            }
            else 
            {
                ModelState.AddModelError("ResumeFile", "Please upload a pdf file.");
            }

            // Create a new Application with the necessary fields
            Application application = new Application
            {
                ResumeFilePath = resumePath,
                JobPostId = jobPost.JobPostId,
                UserId = userId.Value,
                ApplicationDate = DateTime.Today, // Add any additional application data
                Status = "Pending" // Set the initial status of the application
            };

            // Add the application to the database
            await _dbContext.Applications.AddAsync(application);
            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your application has been submitted successfully!";

            // Redirect to a confirmation or success page
            return RedirectToAction("Add", "Application", jobPost.JobPostId);
        }

        // Retrieve Application List based on UserId
        [HttpGet]
        public async Task<IActionResult> List()
        {
            // Get the userId from session
            int? userId = HttpContext.Session.GetInt32("UserId");

            // Ensure userId is not null
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Filter applications based on the userId from session
            var applications = await _dbContext.Applications
                .Where(a => a.UserId == userId.Value) // Assuming UserId is a property in the Application model
                .ToListAsync();

            return View(applications);
        }

        // Download Resume from Applicants
        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            // Fetch the application from the database using ApplicationId
            Application? application = await _dbContext.Applications.FindAsync(id);
            if (application == null)
            {
                return NotFound(); // Return 404 if the application is not found
            }

            // Construct the full file path for the resume
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/applicationForms", Path.GetFileName(application.ResumeFilePath));

            Console.WriteLine($"Attempting to download file at: {filePath}");

            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            // Return the PDF file for download
            return PhysicalFile(filePath, "application/pdf", Path.GetFileName(application.ResumeFilePath));
        }


        // Retrieve Application Information to Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Application application = await _dbContext.Applications.FindAsync(id);

            return View(application);
        }

        // Update Application Status
        [HttpPost]
        public async Task<IActionResult> Edit(Application viewModel)
        {
            Application? application = await _dbContext.Applications.FindAsync(viewModel.ApplicationId);

            if (application != null)
            {
                application.Status = viewModel.Status;

                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List", "Application");
        }

        // HR Update Application Status
        [HttpPost]
        public async Task<IActionResult> UpdateApplicationStatus(int applicationId, string status)
        {
            // Retrieve the application using the applicationId
            Application? application = await _dbContext.Applications.FindAsync(applicationId);
            if (application == null)
            {
                return NotFound(); // Handle case when application doesn't exist
            }

            // Update the status of the application
            application.Status = status;

            // Save changes to the database
            await _dbContext.SaveChangesAsync();

            // Get the JobPostId from the application
            int jobPostId = application.JobPostId;

            // Redirect back to the applicants view with the jobPostId
            return RedirectToAction("ViewApplicants", "JobPost", new { jobPostId });
        }

        // JobSeeker Delete Applications
        [HttpPost]
        public async Task<IActionResult> Delete(Application viewModel)
        {
            Application? application = await _dbContext.Applications  
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ApplicationId == viewModel.ApplicationId);

            if (application != null)
            {
                // Retrieve the file path from the application
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", application.ResumeFilePath.TrimStart('/'));

                // Delete the file from the server if it exists
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Remove the application from the database
                _dbContext.Applications.Remove(application);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List", "Application");
        }

    }
}
