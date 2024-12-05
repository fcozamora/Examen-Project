using Examen_Project.Models.DAO;
using Examen_Project.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Examen_Project.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PaymentDAO _payments = new PaymentDAO();
        private readonly string clientName = "franZa";
        private readonly string key = "test01";

        // GET: Payment/Index
        public async Task<ActionResult> Index()
        {
            try
            {
                
                var payments = await _payments.GetPaymentsAsync(clientName, key);
                string errorMessage = string.Empty;

                
                foreach (var payment in payments)
                {                    
                    var result = _payments.MySqlCreatePayment(payment);               
                }

                return View(payments);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); ;
                return View("Error");
            }
        }


        // GET: Create Payment
        public ActionResult Create()
        {
            return View();
        }

        // POST: Payment/Create
        [HttpPost]
        public async Task<ActionResult> Create(PaymentDTO payment)
        {
            try
            {                
                if (string.IsNullOrEmpty(payment.email) || payment.amount <= 0)
                {
                    Console.WriteLine("Please type a valid email or amount.");
                    return View(payment);
                }

                await _payments.AuthenticateAsync(clientName, key);

                var paymentResult = await _payments.CreatePaymentAsync(payment);

                if (paymentResult)
                {
                    Console.WriteLine("Payment processed successfully.");
                    return RedirectToAction("Create");
                }
                else
                {
                    Console.WriteLine("Failed to create payment");
                    return View(payment);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View(payment);
            }
        }
    }
}