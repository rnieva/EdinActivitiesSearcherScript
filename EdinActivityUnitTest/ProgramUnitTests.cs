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
        //[ExpectedException(typeof(ArgumentException), "No es posible conectar con el servidor remoto")]
        //[ExpectedException(typeof(ArgumentException),"Unable to connect to the remote server")]
        [ExpectedException(typeof(ArgumentException))]
        //[ExpectedException(typeof(System.FormatException), "No es posible conectar con el servidor remoto")]
        //[ExpectedException(typeof(ArgumentException), ExpectedMessage = "No es posible conectar con el servidor remoto")]
        public void GetWebContent_givenInvalidUrl_then_ThrowException()
        {
            //Arrange
            string url = "https://ww.gole.es/?gws_rd=ssl#q=gn+ingles";
            //string url = "https://www.google.es/?gws_rd=ssl#q=guion+en+ingles";

            //// Act
            //string result = Program.GetWebContent(url);
            ////Assert

            //try
            //{
            //    // This line should throw an exception
            //    string result = Program.GetWebContent(url);
            //    //Assert.Fail("No es posible conectar con el servidor remoto");
            //}
            //catch (Exception ex)
            //{
            //    Assert.IsInstanceOfType(typeof(ArgumentException), ex);
            //}
                        try
                           {
                                 string result = Program.GetWebContent(url);
                            }
                      catch
                             {                                                Assert.Fail();
             
                            }
        }

        [TestMethod]
        public void GetNumberOfResults_given_theUrl_then_return_stringWithAnumberOfResults()
        {
            //Arrange
            string url = "https://www.joininedinburgh.org/?q=english&at=46&a=&distance=&pc=&location=&ds_month_year=&de_month_year=&t=morning";

            //Act
            string webContent = Program.GetWebContent(url);
            string  numberResults = Program.GetNumberOfResults(webContent);
            bool result = numberResults.Contains("Found");
            
            //Assert
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void GetNumberOfResults_given_InvalidUrl_then_return_emptyString()
        {
            //Arrange
            string url = "https://www.joininburgh.org/?q=english&at=46&a=&distance=&pc=&location=&ds_month_year=&de_month_year=&t=morning";

            //Act
            string webContent = Program.GetWebContent(url);
            string numberResults = Program.GetNumberOfResults(webContent);


            //Assert
            Assert.AreEqual("", numberResults);
        }
    }
}
