using Library.Model;
using NUnit.Framework;
using System;

namespace WebAPI.Tests
{
    [TestFixture]
    public class SurveyTests
    {
        private Survey survey;

        [SetUp]
        public void SetupBeforeEachTest()
        {
            survey = new Survey("Test");
        }

        [Test]
        public void Assert_ClosingDate_is_default_7days_after_creation()
        {
            DateTime newDate = survey.CreationDate.AddDays(7);
            Assert.True(newDate.Equals(survey.ClosingDate));
        }
    }
}