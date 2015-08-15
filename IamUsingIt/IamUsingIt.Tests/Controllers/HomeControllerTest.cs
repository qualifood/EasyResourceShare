using System.Web.Mvc;
using IamUsingIt.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IamUsingIt.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

       


      
    }
}
