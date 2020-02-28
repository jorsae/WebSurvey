using Library.DataAccess;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace WebAPI.Controllers
{
    public class SurveyQuestionController : ApiController
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: api/surveyquestion
        public IQueryable<SurveyQuestion> GetSurveyQuestions()
        {
            return db.SurveyQuestions;
        }

        // GET: api/surveyquestion/5
        /// <summary>
        /// Gets the SurveyQuestion based on id
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [Route("api/surveyquestion/{id}")]
        [HttpGet]
        [ResponseType(typeof(SurveyQuestion))]
        public async Task<IHttpActionResult> GetSurveyQuestion(int id)
        {
            SurveyQuestion surveyQuestion = await db.SurveyQuestions.FindAsync(id);
            if (surveyQuestion == null)
                return StatusCode(HttpStatusCode.NoContent);

            return Ok(surveyQuestion);
        }

        // GET: api/surveyquestion/surveyid/5
        [Route("api/surveyquestion/surveyid/{surveyId}")]
        [HttpGet]
        [ResponseType(typeof(SurveyQuestion))]
        public async Task<IHttpActionResult> GetSurveyQuestionsBySurveyId(int surveyId)
        {
            List<SurveyQuestion> surveyQuestions = await (from surveyQuestion in db.SurveyQuestions
                                                    where surveyQuestion.SurveyId == surveyId
                                                    select surveyQuestion).OrderBy(sq => sq.QuestionNumber).ToListAsync();
            if (surveyQuestions.Count <= 0)
                return StatusCode(HttpStatusCode.NoContent);

            return Ok(surveyQuestions);
        }

        // PUT: api/surveyquestion/{surveyquestion}
        /// <summary>
        /// Puts the survey question in database
        /// </summary>
        /// <param name="surveyQuestion">The survey question.</param>
        /// <returns></returns>
        [HttpPut]
        [ResponseType(typeof(SurveyQuestion))]
        public async Task<IHttpActionResult> PutSurveyQuestion(SurveyQuestion surveyQuestion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int questionNumber = db.SurveyQuestions
                                .Where(sq => sq.SurveyId == surveyQuestion.SurveyId)
                                .Select(sq => sq.QuestionNumber)
                                .DefaultIfEmpty(0)
                                .Max() + 1;

            if (questionNumber > Settings.MaxQuestionsInSurvey)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            surveyQuestion.QuestionNumber = questionNumber;
            db.SurveyQuestions.Add(surveyQuestion);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return Ok(surveyQuestion);
        }

        // DELETE: api/surveyquestion/{surveyquestion}
        /// <summary>
        /// Deletes the SurveyQuestion from database
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [Route("api/surveyquestion/{surveyQuestionId:int}")]
        [HttpDelete]
        [ResponseType(typeof(SurveyQuestion))]
        public async Task<IHttpActionResult> DeleteSurveyQuestion(int surveyQuestionId)
        {
            SurveyQuestion surveyQuestion = await db.SurveyQuestions.FindAsync(surveyQuestionId);
            if (surveyQuestion == null)
            {
                return NotFound();
            }

            List < SurveyQuestion > surveyQuestionsChangeNumber = await db.SurveyQuestions
                                                                    .Where(sq => sq.SurveyId == surveyQuestion.SurveyId &&
                                                                            sq.QuestionNumber > surveyQuestion.QuestionNumber)
                                                                    .ToListAsync();
            foreach (SurveyQuestion sq in surveyQuestionsChangeNumber)
            {
                --sq.QuestionNumber;
                db.Entry(sq).State = EntityState.Modified;
            }
            db.SurveyQuestions.Remove(surveyQuestion);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return Ok();
        }

        [Route("api/surveyquestion/stats/{surveyQuestionId}")]
        [HttpGet]
        [ResponseType(typeof(float))]
        public IHttpActionResult GetSurveyQuestionStats(int surveyQuestionId)
        {
            var stats = db.SurveyAnswers.Where(i => i.SurveyQuestionId == surveyQuestionId)
                .GroupBy(g => g.SurveyQuestionId)
                .Select(g => new
                {
                    Count = g.Count(),
                    Max = g.Max(i => i.Answer),
                    Min = g.Min(i => i.Answer),
                    Average = g.Average(i => i.Answer)
                }).First();
            return Ok(stats);
        }

        [Route("api/surveyquestion/frequency/{surveyQuestionId}")]
        [HttpGet]
        [ResponseType(typeof(float))]
        public IHttpActionResult GetSurveyAnswerFrequency(int surveyQuestionId)
        {
            var frequencyStats = (from answer in db.SurveyAnswers
                                  where answer.SurveyQuestionId == surveyQuestionId
                                  group answer by answer.Answer into g
                                  let surveyAnswer = new
                                  {
                                      Answer = g.Key,
                                      Frequency = g.Count()
                                  }
                                  select surveyAnswer).OrderBy(s => s.Answer);
            return Ok(frequencyStats);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
