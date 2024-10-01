using NextGenRobotics.Context;
using NextGenRobotics.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;

namespace NextGenRobotics.Controllers
{
    public class OrdersController : Controller
    {
        private AspRoboDB dbContext;


        public OrdersController()
        {
            dbContext = new AspRoboDB();
        }


        public ActionResult Index()
        {
            if (Session["username"] != null && Session["Role"].ToString().Equals("Admin"))
            {


                var orders = dbContext.Orders.Select(order => new OrderViewModel
                {
                    OrderId = order.OrderId,
                    OrderNo = order.OrderNo.ToString(),
                    OrderDate = (DateTime)order.OrderDate,
                    UserName = order.User.Name,
                    UserPhoneNumber = order.User.PhoneNumber,
                    PaymentType = order.Payment.PaymentMode,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    ShippingAddress = order.ShippingAddress,
                    Products = order.OrderDetails.Select(p => new OrderProductViewModel
                    {
                        ProductName = p.Product.Name,
                        Quantity = p.Quantity,
                        TotalPrice = p.TotalPrice
                    }).ToList(),
                }).ToList();


                var totalProducts = dbContext.Products.Count();

                var totalCategory = dbContext.Categories.Count();


                var orderSummary = new OrderSummaryViewModel
                {

                    TotalSales = orders.Where(o => o.Status == "Confirmed").Sum(o => o.TotalAmount),

                    TotalOrders = orders.Count,
                    TotalProducts = dbContext.Products.Count(),
                    TotalCategories = dbContext.Categories.Count(),

                    Orders = orders
                };


                return View(orderSummary);
            }
            return RedirectToAction("Login", "Users");
        }

        // 
        [HttpPost]
        public ActionResult UpdateStatus(int orderId, string status)
        {

            var order = dbContext.Orders.SingleOrDefault(o => o.OrderId == orderId);

            if (order != null)
            {

                order.Status = status;
                dbContext.SaveChanges();
            }


            return RedirectToAction("Index");
        }



        public ActionResult OrderSummary()
        {
            int userId = Convert.ToInt32(Session["id"]);


            var invoices = (from order in dbContext.Orders
                            join orderDetail in dbContext.OrderDetails on order.OrderId equals orderDetail.OrderId
                            join product in dbContext.Products on orderDetail.ProductId equals product.ProductId
                            join payment in dbContext.Payments on order.PaymentId equals payment.PaymentId
                            where order.UserId == userId
                            select new OrderHistoryViewModel
                            {
                                OrderNo = order.OrderNo,
                                ProductName = product.Name,
                                UnitPrice = (double)product.UnitPrice,
                                Quantity = orderDetail.Quantity,
                                TotalPrice = orderDetail.TotalPrice,
                                TotalAmount = (double)order.TotalAmount,


                                PaymentMode = payment.PaymentMode,
                                CardNo = "**" + payment.CardNo.Substring(payment.CardNo.Length - 4),


                                Status = order.Status
                            }).ToList();



            return View(invoices);
        }






























        // Dispose the database context to release resources
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
