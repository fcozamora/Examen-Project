using Examen_Project.Models.DTO;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Examen_Project.Models.DAO
{
    public class PaymentDAO
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly ApiCnx apiCnx = new ApiCnx();
        private readonly string _apiUrl = "https://saacapps.com";
        private string _token;

        public async Task AuthenticateAsync(string clientId, string tokenPass)
        {
            try
            {
                var credentials = apiCnx.AuthenticateAsync(clientId, tokenPass);
                var request = new HttpRequestMessage(HttpMethod.Post, $"{_apiUrl}/payout/auth.php");
                request.Headers.Add("Authorization", $"Bearer {credentials}");

                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to authenticate user: {ex.Message}");
                throw;
            }
        }

        //Get all payments from the API
        public async Task<List<PaymentDTO>> GetPaymentsAsync(string clientId, string tokenPass)
        {
            var _token = apiCnx.AuthenticateAsync(clientId, tokenPass); //Gets the bearer token from API

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiUrl}/payout/payout.php");
                request.Headers.Add("Authorization", $"Bearer {_token}");

                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<List<PaymentDTO>>(responseData);
                return json;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could'nt get payments: {ex.Message}");
                throw;
            }
        }

        //Creates a new payment in the API
        public async Task<bool> CreatePaymentAsync(PaymentDTO payment)
        {
            var content = new StringContent(JsonConvert.SerializeObject(payment), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_apiUrl}/payout/payout.php")
            {
                Content = content
            };
            request.Headers.Add("Authorization", $"Bearer {_token}");

            var response = await _client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }


        //Stores all the payments created in the local DataBase
        public string MySqlCreatePayment(PaymentDTO payment)
        {
            string response = "Failed";

            try
            {
                using (var connection = MysqlCnx.getCnx())
                {
                    connection.Open();
                    string query = "INSERT INTO payout (id, contactId, amount, status, dateAdded) VALUES (@id, @clientId, @amount, @status, @dateAdded)";
                    MySqlCommand sqlCmd = new MySqlCommand(query, connection);

                    sqlCmd.Parameters.AddWithValue("@id", payment.id);
                    sqlCmd.Parameters.AddWithValue("@clientId", payment.clientId);
                    sqlCmd.Parameters.AddWithValue("@amount", payment.amount);
                    sqlCmd.Parameters.AddWithValue("@status", payment.status);
                    sqlCmd.Parameters.AddWithValue("@dateAdded", payment.dateAdded);

                    int rowsAffected = sqlCmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        response = "Success";
                    else
                        response = "No rows affected";

                    connection.Close();
                }
            }
            catch (MySqlException Ex)
            {
                Console.WriteLine($"SQL Error: {Ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
            }

            return response;
        }

    }
}