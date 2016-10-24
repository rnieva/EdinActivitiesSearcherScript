using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ActivitiEdinSearchScript;


namespace EdinActivityUnitTest
{   
    [TestClass]
    public class ProgramUnitTests
    {
        [TestMethod]
        public void GetNumberOfKeyWords_givenUrlWithoutParameters_thenReturnAnEmptyString()
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
        public void GetNumberOfKeyWords_givenUrlWithOnlyOneParameter_thenReturnAStringWithTheParameter()
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
        public void GetNumberOfKeyWords_givenUrlWithTwoAparameter_thenReturnAStringWithTwoParameterNextToDash()
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
        public void GetNumberOfKeyWords_givenDifferentUrlWithAparameter_thenReturnAStringWithTheParameterNextToDash()
        {
            //Arrange
            string url = "https://www.google.es/?gws_rd=ssl#q=guion+en+ingles";

            //Act
            string result = Program.GetNumberOfKeyWords(url);

            //Assert--
            string expected = "ssl#q=guion+en+ingles - ";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetWebContent_givenAnUrl_then_getUrlContent()
        {
            //Arrange
            string url = "https://www.google.es/?gws_rd=ssl#q=guion+en+ingles";

            //Act
            string result = Program.GetWebContent(url);

            //Assert
            Assert.AreNotEqual(null, result);
        }

        [TestMethod]
        //[ExpectedException(typeof(Exception))]
        public void GetWebContent_givenInvalidUrl_then_ThrowException()
        {
        //    Arrange
        //    string url = "https://ww.gole.es/?gws_rd=ssl#q=gn+ingles";

        //    Act
        //    string result = Program.GetWebContent(url);

        //    Assert

        //    try
        //    {
        //        string result = Program.GetWebContent(url);
        //        Assert.Fail("An exception should have been thrown");
        //    }
        //    catch (Exception ae)
        //    {
        //        Assert.AreEqual("Unable to connect to the remote server", ae.Message);
        //    }


        }
    }
}
