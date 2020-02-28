using Library.DataAccess;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace WebAPI.Controllers
{
    public class SurveyAnswerController : ApiController
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: api/SurveyAnswer
        [Route("api/surveyanswer/")]
        [HttpGet]
        public IQueryable<SurveyAnswer> GetSurveyAnswers()
        {
            return db.SurveyAnswers;
        }

        // GET: api/Survey/5
        /// <summary>
        /// Gets the survey answer based on id
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [Route("api/surveyanswer/{id}")]
        [HttpGet]
        [ResponseType(typeof(SurveyAnswer))]
        public async Task<IHttpActionResult> GetSurveyAnswer(int id)
        {
            SurveyAnswer surveyAnswer = await db.SurveyAnswers.FindAsync(id);
            if (surveyAnswer == null)
                return StatusCode(HttpStatusCode.NoContent);

            return Ok(surveyAnswer);
        }

        // GET: api/surveyanswer/question/5
        /// <summary>
        /// Gets the survey question answers.
        /// </summary>
        /// <param name="questionId">The question identifier.</param>
        /// <returns></returns>
        [Route("api/surveyanswer/question/{questionId}")]
        [HttpGet]
        [ResponseType(typeof(SurveyAnswer))]
        public async Task<IHttpActionResult> GetSurveyQuestionAnswers(int questionId)
        {
            List<SurveyAnswer> surveyAnswers = await (from answer in db.SurveyAnswers
                                                      where answer.SurveyQuestionId == questionId
                                                      select answer).ToListAsync();

            if (surveyAnswers.Count <= 0)
                return StatusCode(HttpStatusCode.NoContent);

            return Ok(surveyAnswers);
        }

        // PUT: api/surveyanswer
        [Route("api/surveyanswer/")]
        [HttpPut]
        [ResponseType(typeof(IEnumerable<SurveyAnswer>))]
        public async Task<IHttpActionResult> PutSurveyAnswers(IEnumerable<SurveyAnswer> surveyAnswers)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            foreach(SurveyAnswer answer in surveyAnswers)
            {
                SurveyQuestion surveyQuestion = await db.SurveyQuestions.FindAsync(answer.SurveyQuestionId);
                Survey survey = await db.Surveys.FindAsync(surveyQuestion.SurveyId);

                if (!survey.IsActive())
                {
                    return StatusCode(HttpStatusCode.Unauthorized); // 401
                }

                db.SurveyAnswers.Add(answer);
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                Debug.WriteLine("DbUpdateConcurrencyException");
                return StatusCode(HttpStatusCode.InternalServerError);
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return Ok(surveyAnswers);
        }
    }
}