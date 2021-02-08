using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admins")]
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _repository;


        public OrdersController(IOrderRepository repository)
        {
            _repository = repository;
        }


        public IActionResult List()
        {
            return View(_repository.Orders.Where(o => o.Shipped == false));
        }

        [HttpPost]
        public IActionResult MarkShipped(int orderId)
        {
            Order order = _repository.Orders
                .FirstOrDefault(order => order.OrderId == orderId);

            if (order != null)
            {
                order.Shipped = true;
                _repository.SaveOrder(order);
            }

            return RedirectToAction(nameof(List));
        }
    }
}
