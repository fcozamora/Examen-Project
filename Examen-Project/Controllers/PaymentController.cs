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

        // GET: Payment/Index (List all payments)
        public async Task<ActionResult> Index()
        {
            try
            {
                // Fetch all payments from the API
                var payments = await _payments.GetAllPaymentsAsync(clientName, key);
                bool anyErrors = false;
                string errorMessage = string.Empty;

                // Insert each payment into the database using PaymentDao
                foreach (var payment in payments)
                {
                    // Insert the payment into the database
                    var result = _payments.MySqlCreatePayment(payment);
                    if (result != "Success")
                    {
                        // Log or handle failed inserts
                        anyErrors = true;
                        errorMessage += $"Failed to insert payment ID: {payment.id}, Reason: {result}\n";
                    }
                    else
                    {
                        Console.WriteLine($"Payment ID: {payment.id} inserted successfully.");
                    }
                }

                // Add success or error messages
                if (anyErrors)
                {
                    TempData["ErrorMessage"] = "Some payments failed to be added to the database. Details: " + errorMessage;
                }
                else
                {
                    TempData["SuccessMessage"] = "All payments processed and saved successfully.";
                }

                // Return the view with fetched payments
                return View(payments);
            }
            catch (InvalidOperationException ex)
            {
                // Handle authentication failure
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
            catch (Exception ex)
            {
                // Handle other types of errors
                ViewBag.ErrorMessage = "An unexpected error occurred: " + ex.Message;
                return View("Error");
            }
        }


        // GET: Create Payment
        public ActionResult Create()
        {
            return View();
        }

        // POST: Payment/Create (Handle the creation of a new payment)
        [HttpPost]
        public async Task<ActionResult> Create(PaymentDTO payment)
        {
            try
            {
                // Validate payment fields
                if (string.IsNullOrEmpty(payment.email) || payment.amount <= 0)
                {
                    TempData["ErrorMessage"] = "Please fill all fields and provide a valid payment amount.";
                    return View(payment);
                }

                //Authenticates contact information
                await _payments.AuthenticateAsync(clientName, key);

                //Creates a new payment
                var paymentResult = await _payments.CreatePaymentAsync(payment);

                if (paymentResult)
                {
                    // Payment was processed successfully via the API
                    TempData["SuccessMessage"] = "Payment processed successfully.";
                    return RedirectToAction("Create");
                }
                else
                {
                    TempData["ErrorMessage"] = "Payment processing failed. Please try again.";
                    return View(payment);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while processing the payment: " + ex.Message;
                return View(payment);
            }
        }
    }
}