using FarmerToConsumer.Repos;
using FarmerToConsumer.Shared;
using FarmerToConsumer.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace FarmerToConsumer.Web.Controllers;

public class CustomerCartController(CustomerShopRepo repo, CurrentUserHelper currentUser) : Controller
{
    public IActionResult Index()
    {
        var result = repo.GetCart();
        if (result.HasError)
        {
            ViewBag.Error = result.Message;
        }

        return View(result.Data);
    }

    public IActionResult Add(int dataId)
    {
        var result = repo.AddToCart(dataId);
        TempData[result.HasError ? "Error" : "Success"] = result.HasError ? result.Message : "Product added to cart.";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult Update(int stockId, int quantity)
    {
        var result = repo.UpdateCart(stockId, quantity);
        TempData[result.HasError ? "Error" : "Success"] = result.HasError ? result.Message : "Cart updated.";
        return RedirectToAction("Index");
    }

    public IActionResult Remove(int dataId)
    {
        var result = repo.UpdateCart(dataId, 0);
        TempData[result.HasError ? "Error" : "Success"] = result.HasError ? result.Message : "Product removed from cart.";
        return RedirectToAction("Index");
    }

    public IActionResult Checkout()
    {
        var cart = repo.GetCart();
        if (cart.HasError || cart.Data == null || cart.Data.Items.Count == 0)
        {
            TempData["Error"] = cart.Message == "" ? "Cart is empty." : cart.Message;
            return RedirectToAction("Index");
        }

        if (currentUser.UserId <= 0)
        {
            TempData["Error"] = "Please login before checkout.";
            return RedirectToAction("Login", "Account");
        }

        return View(new CustomerCheckoutModel());
    }

    [HttpPost]
    public IActionResult Checkout(CustomerCheckoutModel model)
    {
        if (currentUser.UserId <= 0)
        {
            TempData["Error"] = "Please login before checkout.";
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = repo.Checkout(currentUser.UserId, model);
        if (result.HasError || result.Data == null)
        {
            ViewBag.Error = result.Message;
            return View(model);
        }

        TempData["Success"] = $"Order #{result.Data.ID} placed successfully.";
        return RedirectToAction("Detail", "CustomerOrder", new { dataId = result.Data.ID });
    }
}
