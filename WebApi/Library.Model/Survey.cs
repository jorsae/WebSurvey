using System;
using System.ComponentModel.DataAnnotations;

namespace Library.Model
{
    public class Survey
    {
        [Key]
        public int SurveyId { get; set; }
        [Required]
        [MaxLength(64)]
        public string SurveyTitle { get; set; }
        [Required]
        public string SurveyGuid { get; set; }
        public DateTime CreationDate { get; set; }

        public DateTime ClosingDate { get; set; }

        // Empty constructor for EntityFramework
        public Survey()
        {
            SurveyGuid = Guid.NewGuid().ToString();
            CreationDate = DateTime.Now;
            ClosingDate = CreationDate.AddDays(7);
        }

        public Survey(string surveyTitle, DateTime? closingDate = null)
        {
            CreationDate = DateTime.Now;
            SurveyTitle = surveyTitle;
            ClosingDate = (closingDate == null) ? CreationDate.AddDays(7) : (DateTime)closingDate;
            SurveyGuid = Guid.NewGuid().ToString();
        }

        public bool IsActive()
        {
            if (ClosingDate >= DateTime.Now)
                return true;
            else
                return false;
        }
    }
}