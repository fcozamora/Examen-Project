using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Examen_Project.Models
{
    public class MysqlCnx
    {
        private static string cnx = "Server=localhost; port=3307; Database=payment; UserID=root";
        public static MySqlConnection getCnx()
        {
            return new MySqlConnection(cnx);
        }
    }
}