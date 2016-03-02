using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EShop.Models.Enitites;
using EShop.Services;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;
using EShop.ViewModels.Home;
using Microsoft.Data.Entity;

namespace EShop.Controllers.API
{

    [Produces("application/json")]
    [Route("api/Order")]
    public class OrderController : Controller
    {
        private readonly ITelegramSender _telegram;
        private readonly DataContext _context;

        public OrderController(ITelegramSender telegram, DataContext context)
        {
            _telegram = telegram;
            _context = context;
        }

        // POST api/order
        [HttpPost]
        public bool Post(OrderViewModel order)
        {
            try
            {
                var customer = new Customer
                {
                    Name = order.CustomerName,
                    Phone = order.CustomerPhone,
                    Email = order.CustomerEmail
                };

                _context.Customers.Add(customer);
                _context.SaveChanges();

                _context.Orders.Add(new Order
                {
                    ProductId = order.ProductId,
                    Quantity = order.Quantity,
                    CutomerId = customer.Id,
                    ShipmentMethodId = order.ShipmentMethodId,
                    Address = order.Address,
                    PaymentMethodId = order.PaymentMethodId,
                    Comment = order.Comment
                });

                _context.SaveChanges();
            }
            catch (System.Exception)
            {
                _telegram.SendMessageAsync($"WARNING: the order form {order.CustomerName} ({order.CustomerEmail}) has NOT been added to the database!");
            }

            _telegram.SendMessageAsync(order.ToString());

            return true;
        }
    }
}