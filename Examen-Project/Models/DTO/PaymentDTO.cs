using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace Examen_Project.Models.DTO
{
    public class PaymentDTO
    {
        
        public int id { get; set; }
        [EmailAddress]
        [Required]
        public string email { get; set; }
        public int clientId { get; set; }

        [Required(ErrorMessage = "An amount is required")]
        
        public decimal amount { get; set; }

        public string status { get; set; }
        public Timestamp dateAdded { get; set; }
    }
}