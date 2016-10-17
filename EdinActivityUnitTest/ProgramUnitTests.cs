using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ActivitiEdinSearchScript;


namespace EdinActivityUnitTest
{
    [TestClass]
    public class ProgramUnitTests
    {
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
    }
}
