﻿using MyShop.Core.Contracts;
using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers {
    public class BasketController : Controller {
        public IBasketService basketService;
        public IOrderService orderService;
        public IRepository<Customer> customers;

        public BasketController(IBasketService basketService, IOrderService orderService, IRepository<Customer> customers) {
            this.basketService = basketService;
            this.orderService = orderService;
            this.customers = customers;
        }

        // GET: Basket
        public ActionResult Index() {
            var model = basketService.GetBasketItems(this.HttpContext);
            return View(model);
        }

        public ActionResult AddToBasket(string Id) {
            basketService.AddToBasket(this.HttpContext, Id);
            return RedirectToAction("Index");
        }

        public ActionResult RemoveFromBasket(string Id) {
            basketService.RemoveFromBasket(this.HttpContext, Id);
            return RedirectToAction("Index");
        }

        public PartialViewResult BasketSummary() {
            var basketSummary = basketService.GetBasketSummary(this.HttpContext);

            return PartialView(basketSummary);
        }

        [Authorize]
        public ActionResult CheckOut() {
            Customer customer = customers.Collection().FirstOrDefault(c=>c.Email == User.Identity.Name);
            
            if (customer != null) {
                Order order = new Order() {
                    Email = customer.Email,
                    City = customer.City,
                    State = customer.State,
                    Street = customer.Street,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    ZipCode = customer.ZipCode
                };

                return View(order);
            }

            return RedirectToAction("Error");
        }

        [HttpPost]
        [Authorize]
        public ActionResult Checkout(Order order) {
            var basketItems = basketService.GetBasketItems(this.HttpContext);
            order.OrderStatus = "Order Created";
            order.Email = User.Identity.Name;

            //Process payment

            order.OrderStatus = "Payment Processed";

            orderService.CreateOrder(order, basketItems);
            basketService.ClearBasket(this.HttpContext);

            return RedirectToAction("ThankYou", new { OrderId = order.Id });
        }

        public ActionResult ThankYou(string OrderId) {
            ViewBag.OrderId = OrderId;

            return View();
        }
    }
}