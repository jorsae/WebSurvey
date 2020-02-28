using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Library.Model
{
    public class SurveyQuestion
    {
        [Key]
        public int SurveyQuestionId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Question { get; set; }
        public int QuestionNumber { get; set; }

        [ForeignKey("Survey")]
        public int SurveyId { get; set; }
        [IgnoreDataMember]
        public Survey Survey { get; set; }

        // Empty constructor for EntityFramework
        public SurveyQuestion()
        {

        }

        public SurveyQuestion(int surveyId, int questionNumber, string question)
        {
            SurveyId = surveyId;
            QuestionNumber = questionNumber;
            Question = question;
        }
    }
}