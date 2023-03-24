using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using MyShop.WebUI;
using MyShop.WebUI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MyShop.WebUI.Tests.Controllers {
    [TestClass]
    public class HomeControllerTest {
        [TestMethod]
        public void IndexPageDoesReturnProducts() {
            IRepository<Product> productContext = new Mocks.MockContext<Product>();
            IRepository<ProductCategory> productCateroryContext = new Mocks.MockContext<ProductCategory>();
            HomeController controller = new HomeController(productContext, productCateroryContext);

            productContext.Insert(new Product());

            var result = controller.Index() as ViewResult;
            var viewModel = (ProductListViewModel)result.ViewData.Model;

            Assert.AreEqual(1, viewModel.products.Count());
        }
    }
}
