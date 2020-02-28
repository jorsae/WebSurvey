using Library.DataAccess;
using Library.Model;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using WebAPI.Controllers;

namespace WebAPI.Tests
{
    [TestFixture]
    class SurveyQuestionControllerTests
    {
        private DatabaseContext context;
        private SurveyQuestionController sqController;
        private SurveyController surveyController;
        private SurveyQuestion surveyQuestion;
        private Survey survey;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            context = new DatabaseContext();
            context.Database.Connection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=WebApi_TestDatabase;Integrated Security=True;Pooling=False";
            context.Database.CreateIfNotExists();
        }

        [SetUp]
        public async Task SetUp()
        {
            surveyController = new SurveyController();
            sqController = new SurveyQuestionController();

            survey = new Survey("test survey");
            await surveyController.PutSurvey(survey);
            surveyQuestion = new SurveyQuestion(survey.SurveyId, 1, "test question");
        }

        [Test]
        public async Task Assert_put_surveyquestion()
        {
            IHttpActionResult result = await sqController.PutSurveyQuestion(surveyQuestion);
            Assert.IsInstanceOf(typeof(OkNegotiatedContentResult<SurveyQuestion>), result);
        }

        [Test]
        public async Task Assert_fail_put_surveyquestion_wrong_surveyid()
        {
            IHttpActionResult result = await sqController.PutSurveyQuestion(new SurveyQuestion(1000, 1, "question"));
            Assert.IsNotInstanceOf(typeof(OkNegotiatedContentResult<SurveyQuestion>), result);
        }

        [Test]
        public async Task Assert_get_surveyquestion()
        {
            IHttpActionResult result = await sqController.PutSurveyQuestion(surveyQuestion);
            Assert.IsInstanceOf(typeof(OkNegotiatedContentResult<SurveyQuestion>), result);

            IHttpActionResult testResult = await sqController.GetSurveyQuestion(surveyQuestion.SurveyQuestionId);
            Assert.IsInstanceOf(typeof(OkNegotiatedContentResult<SurveyQuestion>), testResult);
        }

        [Test]
        public async Task Assert_get_surveyquestion_by_surveyid()
        {
            IHttpActionResult result1 = await sqController.PutSurveyQuestion(surveyQuestion);
            Assert.IsInstanceOf(typeof(OkNegotiatedContentResult<SurveyQuestion>), result1);
            IHttpActionResult result2 = await sqController.PutSurveyQuestion(new SurveyQuestion(survey.SurveyId, 1, "asd"));
            Assert.IsInstanceOf(typeof(OkNegotiatedContentResult<SurveyQuestion>), result2);

            IHttpActionResult testResult = await sqController.GetSurveyQuestionsBySurveyId(surveyQuestion.SurveyId);
            Assert.IsInstanceOf(typeof(OkNegotiatedContentResult<List<SurveyQuestion>>), testResult);
        }

        [Test]
        public async Task Assert_sets_question_number_to_1()
        {
            IHttpActionResult result = await sqController.PutSurveyQuestion(new SurveyQuestion(survey.SurveyId, 4000, "a"));
            var contentResult = result as OkNegotiatedContentResult<SurveyQuestion>;
            SurveyQuestion sq = contentResult.Content;
            Assert.AreEqual(1, sq.QuestionNumber);
        }

        [Test]
        public async Task Assert_delete_surveyquestion()
        {
            IHttpActionResult putResult = await sqController.PutSurveyQuestion(surveyQuestion);
            Assert.IsInstanceOf(typeof(OkNegotiatedContentResult<SurveyQuestion>), putResult);

            IHttpActionResult result = await sqController.DeleteSurveyQuestion(surveyQuestion.SurveyQuestionId);
            Assert.IsInstanceOf(typeof(OkResult), result);
        }
    }
}