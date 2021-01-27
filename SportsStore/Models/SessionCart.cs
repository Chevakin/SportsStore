using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SportsStore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Models
{
    public class SessionCart : Cart
    {
        [JsonIgnore]
        public ISession Session { get; set; }


        public static Cart GetCart (IHttpContextAccessor contextAccessor)
        {
            ISession session = contextAccessor.HttpContext.Session;

            SessionCart cart = session?.GetJson<SessionCart>("Cart")
                ?? new SessionCart();

            cart.Session = session;

            return cart;
        }

        public override void AddItem(Product product, int quantity)
        {
            base.AddItem(product, quantity);

            Session.SetJson("Cart", this);
        }

        public override void RemoveLine(Product product)
        {
            base.RemoveLine(product);

            Session.SetJson("Cart", this);
        }

        public override void Clear()
        {
            base.Clear();

            Session.Remove("Cart");
        }
    }
}
