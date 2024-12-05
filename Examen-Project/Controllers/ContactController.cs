using Examen_Project.Models;
using Examen_Project.Models.DAO;
using Examen_Project.Models.DTO;
using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Examen_Project.Controllers
{
    
    public class ContactController : Controller
    {
        private readonly ContactDAO _contacts = new ContactDAO();
        private readonly string clientName = "franZa";
        private readonly string key = "test01";
        // GET: Contact
        public async Task<ActionResult> Index()
        {
            try
            {
                var contacts = await _contacts.GetContactsAsync(clientName, key);
                foreach (var contact in contacts)
                { 
                    var addContactMysql = _contacts.AddContactMysql(contact);
                }
                return View(contacts);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something happened" + ex.Message);
                return View("Error");
            }
            
           
        }

        // GET: Contact/Details/5
        public async Task<ActionResult> Details(string client, string keyPass, string email)
        {
            client = clientName;
            keyPass = key;

            var contact = await _contacts.GetContactsByEmailAsync(client, keyPass, email);

            return View(contact);
        }

        // GET: Contact/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Contact/Create
        [HttpPost]
        public async Task<ActionResult> Create(string client, string keyPass, ContactDTO newContact)
        {
            client = clientName;
            keyPass = key;
            try
            {
                await _contacts.CreateContactAsync(client, keyPass, newContact);
                return RedirectToAction("Index");
            }
            catch
            {
                return View("Could'nt create new contact");
            }
        }
    }
}
