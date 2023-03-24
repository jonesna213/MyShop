﻿using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services {
    public class OrderService : IOrderService {
        IRepository<Order> OrderContext;

        public OrderService(IRepository<Order> OrderContext) { 
            this.OrderContext = OrderContext;
        }

        public void CreateOrder(Order baseOrder, List<BasketItemViewModel> basketItems) {
            foreach (var item in basketItems) {
                baseOrder.OrderItems.Add(new OrderItem() {
                    ProductId = item.Id,
                    Image = item.Image,
                    Price = item.Price,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity
                });
            }

            OrderContext.Insert(baseOrder);
            OrderContext.Commit();
        }

        public List<Order> GetOrderList() {
            return OrderContext.Collection().ToList();
        }

        public Order GetOrder(string Id) {
            return OrderContext.Find(Id);
        }

        public void UpdateOrder(Order order) {
            OrderContext.Update(order);
            OrderContext.Commit();
        }
    }
}
