using MyShop.Core.Contracts;
using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyShop.Services {
    public class BasketService {
        public const string BasketSessionName = "eCommerceBasket";

        public IRepository<Product> productContext;
        public IRepository<Basket> basketContext;

        public BasketService(IRepository<Product> productContext, IRepository<Basket> basketContext) { 
            this.basketContext = basketContext;
            this.productContext = productContext;
        }

        private Basket GetBasket(HttpContextBase httpContext, bool createIfNull) {
            HttpCookie cookie = httpContext.Request.Cookies.Get(BasketSessionName);

            Basket basket = new Basket();

            if (cookie != null) {
                string basketId = cookie.Value;
                if (!string.IsNullOrEmpty(basketId)) {
                    basket = basketContext.Find(basketId);
                } else {
                    if (createIfNull) {
                        basket = CreateNewBasket(httpContext);
                    }
                }
            } else {
                if (createIfNull) {
                    basket = CreateNewBasket(httpContext);
                }
            }

            return basket;
        }
        
        private Basket CreateNewBasket(HttpContextBase httpContext) {
            Basket basket = new Basket();
            basketContext.Insert(basket);
            basketContext.Commit();

            HttpCookie cookie = new HttpCookie(BasketSessionName);
            cookie.Value = basket.Id;
            cookie.Expires = DateTime.Now.AddDays(1);
            httpContext.Response.Cookies.Add(cookie);

            return basket;
        }

        public void AddToBasket(HttpContextBase httpContext, string productId) {
            Basket basket = GetBasket(httpContext, true);
            BasketItem item = basket.BasketItems.FirstOrDefault(i=>i.ProductId == productId);

            if (item == null) {
                item = new BasketItem();
                item.BasketId = basket.Id;
                item.ProductId = productId;
                item.Quantity = 1;

                basket.BasketItems.Add(item);
            } else {
                item.Quantity += 1;
            }

            basketContext.Commit();
        }

        public void RemoveFromBasket(HttpContextBase httpContext, string itemId) {
            Basket basket = GetBasket(httpContext, true);
            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.Id == itemId);

            if (item != null) {
                basket.BasketItems.Remove(item);
                basketContext.Commit();
            }
        }
    }
}
