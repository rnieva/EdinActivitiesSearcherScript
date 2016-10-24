using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ActivitiEdinSearchScript;


namespace EdinActivityUnitTest
{   
    [TestClass]
    public class ProgramUnitTests
    {
        //comment
        [TestMethod]
        public void givenUrlWithoutParameters_thenReturnAnEmptyString()
        {
            //Arrange
            string url = "https://www.joininedinburgh.org/";

            //Act
            string result = Program.GetNumberOfKeyWords(url);

            //Assert--
            string expected = "";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void givenUrlWithOnlyOneParameter_thenReturnAStringWithTheParameter()
        {
            //Arrange
            string url = "https://www.joininedinburgh.org/?q=english&at=46&a=&distance=&pc=&location=&ds_month_year=&de_month_year=";

            //Act
            string result = Program.GetNumberOfKeyWords(url);

            //Assert--
            string expected = "english - ";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void givenUrlWithTwoAparameter_thenReturnAStringWithTwoParameterNextToDash()
        {
            //Arrange
            string url = "https://www.joininedinburgh.org/?q=english&at=46&a=&distance=&pc=&location=&ds_month_year=&de_month_year=&t=morning";

            //Act
            string result = Program.GetNumberOfKeyWords(url);

            //Assert--
            string expected = "english - morning - ";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void givenDifferentUrlWithAparameter_thenReturnAStringWithTheParameterNextToDash()
        {
            //Arrange
            string url = "https://www.google.es/?gws_rd=ssl#q=guion+en+ingles";

            //Act
            string result = Program.GetNumberOfKeyWords(url);

            //Assert--
            string expected = "ssl#q=guion+en+ingles - ";
            Assert.AreEqual(expected, result);
        }
        //Duda: En vez de hacer diferentes UT para diferentes URLs, probar todas las URLs en el mismo UT con diferentes Asserts

    }
}
