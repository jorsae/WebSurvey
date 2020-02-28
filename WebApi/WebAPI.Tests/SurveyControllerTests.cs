using Library.DataAccess;
using Library.Model;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using WebAPI.Controllers;

namespace WebAPI.Tests
{
    [TestFixture]
    public class SurveyControllerTests
    {
        private DatabaseContext context;
        private SurveyController surveyController;
        private Survey survey;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            context = new DatabaseContext();
            context.Database.Connection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=WebApi_TestDatabase;Integrated Security=True;Pooling=False";
            context.Database.CreateIfNotExists();
        }

        [SetUp]
        public void SetUp()
        {
            survey = new Survey("Test");
            surveyController = new SurveyController();
        }

        [Test]
        public async Task Assert_put_survey()
        {
            IHttpActionResult result = await surveyController.PutSurvey(survey);
            Assert.IsInstanceOf(typeof(OkNegotiatedContentResult<Survey>), result);
        }

        [Test]
        public async Task Assert_get_survey_by_id()
        {
            IHttpActionResult actionResult = await surveyController.PutSurvey(survey);
            var contentResult = actionResult as OkNegotiatedContentResult<Survey>;
            Assert.IsNotNull(contentResult);

            IHttpActionResult result = await surveyController.GetSurveyById(survey.SurveyId);
            Assert.IsInstanceOf(typeof(OkNegotiatedContentResult<Survey>), result);
        }

        [Test]
        public async Task Assert_get_survey_by_guid()
        {
            IHttpActionResult actionResult = await surveyController.PutSurvey(survey);
            var contentResult = actionResult as OkNegotiatedContentResult<Survey>;
            Assert.IsNotNull(contentResult);

            IHttpActionResult result = await surveyController.GetSurveyByGuid(survey.SurveyGuid);
            Assert.IsInstanceOf(typeof(OkNegotiatedContentResult<Survey>), result);
        }

        [Test]
        public async Task Assert_post_change_survey()
        {
            IHttpActionResult actionResult = await surveyController.PutSurvey(survey);
            var putResult = actionResult as OkNegotiatedContentResult<Survey>;
            Assert.IsNotNull(putResult);

            // Make changes to survey
            survey.SurveyTitle = "asd";
            survey.ClosingDate = DateTime.Now;
            await surveyController.PostSurveyChange(survey);

            IHttpActionResult newResult = await surveyController.GetSurveyById(survey.SurveyId);
            var result = newResult as OkNegotiatedContentResult<Survey>;
            Survey newSurvey = result.Content;
            Assert.AreEqual(survey, newSurvey);
        }

        [Test]
        public async Task Assert_make_surve_inactive()
        {
            Survey surveyTest = new Survey("survey");

            IHttpActionResult actionResult = await surveyController.PutSurvey(surveyTest);
            var putResult = actionResult as OkNegotiatedContentResult<Survey>;
            Survey oldSurvey = putResult.Content;
            Assert.True(oldSurvey.IsActive());

            // Make survey inactive
            await surveyController.PostSurveyInactive(surveyTest.SurveyId);

            IHttpActionResult newResult = await surveyController.GetSurveyById(surveyTest.SurveyId);
            var result = newResult as OkNegotiatedContentResult<Survey>;
            Survey newSurvey = result.Content;
            Assert.False(newSurvey.IsActive());
        }

        [Test]
        public async Task Assert_survey_is_deleted()
        {
            // add survey
            IHttpActionResult actionResult = await surveyController.PutSurvey(survey);
            var contentResult = actionResult as OkNegotiatedContentResult<Survey>;
            Assert.IsNotNull(contentResult);

            // Delete survey
            IHttpActionResult result = await surveyController.DeleteSurvey(survey.SurveyId);
            Assert.IsInstanceOf(typeof(OkResult), result);
        }
    }
}
