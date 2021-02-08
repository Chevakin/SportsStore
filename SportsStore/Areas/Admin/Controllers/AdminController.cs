using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsStore.Areas.Admin.Controllers;
using SportsStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly IProductRepository _repository;

        public AdminController(IProductRepository repository)
        {
            _repository = repository;
        }


        public IActionResult Index() 
        {
            return View(_repository.Products);
        }

        public IActionResult Edit(int productId)
        {
            return View(_repository.Products
                .FirstOrDefault(p => p.ProductId == productId));
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _repository.SaveProduct(product);
                TempData["message"] = $"{product.Name} has been saved";

                return RedirectToAction("Index");
            }
            else
            {
                return View(product);
            }
        }

        public IActionResult Create()
        {
            return View("Edit", new Product());
        }

        [HttpPost]
        public IActionResult Delete(int productId)
        {
            Product deleteProduct = _repository.DeleteProduct(productId);

            if (deleteProduct != null)
            {
                TempData["message"] = $"{deleteProduct.Name} was deleted";
            }

            return RedirectToAction("Index");
        }
    }
}
