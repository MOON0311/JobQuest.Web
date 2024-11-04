using System.ComponentModel.DataAnnotations;

namespace JobQuest.Web.Models
{
    public class ApplicationForm
    {
        public JobPost JobPost { get; set; } // Job Post Details
        public Application Application { get; set; } // Application Form

        [Required(ErrorMessage = "Resume file is required.")]
        [DataType(DataType.Upload)]
        public IFormFile ResumeFile { get; set; }

        public bool ResumeValidation()
        {
            if (ResumeFile == null) return false;

            var fileExtension = Path.GetExtension(ResumeFile.FileName);
            return fileExtension.Equals(".pdf", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<string> Upload(IFormFile file)
        {
            // Generate a unique file name and determine the file path
            var fileName = Path.GetFileName(file.FileName);
            var fileExtension = Path.GetExtension(fileName);
            var fileNameToSave = $"{Guid.NewGuid()}{fileExtension}";
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/applicationForms");

            // Ensure the directory exists
            Directory.CreateDirectory(directoryPath);

            var filePath = Path.Combine(directoryPath, fileNameToSave);

            // Copy file to the specified path
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return the relative path to be saved in the database
            return $"/applicationForms/{fileNameToSave}";
        }
    }
}
