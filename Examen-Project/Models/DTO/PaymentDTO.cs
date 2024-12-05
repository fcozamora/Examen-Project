using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Examen_Project.Models.DTO
{
    public class PaymentDTO
    {
        
        public int id { get; set; }
        public string email { get; set; }
        public int clientId { get; set; }
        public decimal amount { get; set; }
        public string status { get; set; }
        public Timestamp dateAdded { get; set; }
    }
}