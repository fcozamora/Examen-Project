using Examen_Project.Models.DTO;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Examen_Project.Models.DAO
{
    public class ContactDAO
    {
        private readonly string _apiUrl = "https://saacapps.com";
        private readonly HttpClient _client = new HttpClient();
        private readonly ApiCnx apiCnx = new ApiCnx();
        private string tokenKey;


        public async Task AuthenticateAsync(string clientId, string tokenPass)
        {
            var _token = await apiCnx.AuthenticateAsync(clientId, tokenPass);
            try
            {                
                var request = new HttpRequestMessage(HttpMethod.Post, $"{_apiUrl}/payout/auth.php");
                request.Headers.Add("Authorization", $"Bearer {_token}");

                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
                if (json.ContainsKey("token"))
                {
                    tokenKey = json["token"];
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to authenticate user: {ex.Message}");
            }
        }
        //This method will show all contacts registered.
        public async Task<List<ContactDTO>> GetContactsAsync(string clientId, string tokenPass)
        {
            var _token = apiCnx.GetContactsAsync(clientId, tokenPass);
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiUrl}/payout/contact.php");
                request.Headers.Add("Authorization", $"Bearer {_token}");

                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadAsStringAsync();
                var json =  JsonConvert.DeserializeObject<List<ContactDTO>>(responseData);
                return json;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could'nt get all contacts: {ex.Message}");
                return new List<ContactDTO>();
            }
        }

        //Get contact information by email.
        public async Task<List<ContactDTO>> GetContactsByEmailAsync(string clientId, string tokenPass, string email)
        {

            var _token = apiCnx.AuthenticateAsync(clientId, tokenPass);

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiUrl}/payout/contact.php?email={email}");
                request.Headers.Add("Authorization", $"Bearer {_token}");

                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<List<ContactDTO>>(responseData);
                return json;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could'nt get contact information: {ex.Message}");
                return new List<ContactDTO>();
            }
        }

        //Attemps to create a new contact on the API
        public async Task<bool> CreateContactAsync(string clientId, string tokenPass, ContactDTO newContact)
        {
            var _token = apiCnx.AuthenticateAsync(clientId, tokenPass);
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, $"{_apiUrl}/payout/contact.php?email={newContact.Email}");
                request.Headers.Add("Authorization", $"Bearer {_token}");

                var content = new StringContent(JsonConvert.SerializeObject(new
                {
                    first_name = newContact.First_Name,
                    last_name = newContact.Last_Name,
                    Phone = newContact.Phone,
                    Email = newContact.Email
                }), null, "application/json");

                request.Content = content;

                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding contact: {ex.Message}");
                return false;
            }
        }

        //Attemps to add contacts to the database
        public string AddContactMysql(ContactDTO contact)
        {
            string response = "Failed";

            try
            {
                using (var connection = MysqlCnx.getCnx())
                {
                    connection.Open();
                    string query = "INSERT INTO contacts (clientId, firstName, lastName, phone, email)" +
                                   "VALUES (@clientId, @firstName, @lastName, @phone, @email)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@clientId", contact.Id);
                    cmd.Parameters.AddWithValue("@firstName", contact.First_Name);
                    cmd.Parameters.AddWithValue("@lastName", contact.Last_Name);
                    cmd.Parameters.AddWithValue("@phone", contact.Phone);
                    cmd.Parameters.AddWithValue("@email", contact.Email);


                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        response = "Success";
                    else
                        response = "No rows affected";
                    connection.Close();
                }
            }
            catch (MySqlException sqlEx)
            {
                Console.WriteLine($"SQL Error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
            }

            return response;
        }
    }
}