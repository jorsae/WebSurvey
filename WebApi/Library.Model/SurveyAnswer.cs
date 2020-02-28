using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Library.Model
{
    public class SurveyAnswer
    {
        [Key]
        public int SurveyAnswerId { get; set; }
        [Required]
        [Range(1, 10)]
        public int Answer { get; set; }

        [ForeignKey("SurveyQuestion")]
        public int SurveyQuestionId { get; set; }
        [IgnoreDataMember]
        public SurveyQuestion SurveyQuestion { get; set; }

        // Empty constructor for EntityFramework
        public SurveyAnswer()
        {

        }

        public SurveyAnswer(int surveyQuestionId, int answer)
        {
            SurveyQuestionId = surveyQuestionId;
            Answer = answer;
        }
    }
}