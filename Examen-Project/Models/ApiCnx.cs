﻿using Examen_Project.Models.DAO;
using Examen_Project.Models.DTO;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Examen_Project.Models
{
    public class ApiCnx
    {
        private static readonly HttpClient _client = new HttpClient();
        private readonly string _apiUrl = "https://saacapps.com";
        private static string _token;

        //Authentica user to connect on API
        public async Task<string> AuthenticateAsync(string clientId, string tokenPass)
        {
            if (_token != null) //If there is a token already, just return it back
            {
                return _token;
            }
            else
            {
                using (var clientRequest = new HttpClient())
                {                    
                    try
                    {
                        var request = new HttpRequestMessage(HttpMethod.Post, $"{_apiUrl}/payout/auth.php");
                        var apiKeyBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{tokenPass}"));
                        request.Headers.Add("Authorization", $"Basic {apiKeyBase64}");
                        var response = await clientRequest.SendAsync(request);
                        response.EnsureSuccessStatusCode();

                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        var tokenResponse = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                        _token = tokenResponse.token;

                        return _token;
                    }
                    catch (HttpRequestException e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                        return null;
                    }
                }
            }
        }

        //Authenticates and gets all contacts from the API
        public async Task<List<ContactDTO>> GetContactsAsync(string clientId, string tokenPass)
        {
            var token = await AuthenticateAsync(clientId, tokenPass);

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiUrl}/payout/contact.php");
                request.Headers.Add("Authorization", $"Bearer {token}");

                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();

                var json = JsonConvert.DeserializeObject<List<ContactDTO>>(jsonResponse);
                return json;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could'nt get all contacts: {ex.Message}");
                throw;
            }
        }

        //Authenticates and gets the information of a single contact
        public async Task<ContactDTO> GetContactByEmailAsync(string clientId, string tokenPass, string email)
        {
            var token = await AuthenticateAsync(clientId, tokenPass);

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiUrl}/payout/contact.php?email={email}");
                request.Headers.Add("Authorization", $"Bearer {token}");

                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ContactDTO>(jsonResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could'nt get contact information: {ex.Message}");
                throw;
            }
        }


        //Authenticates and creates a new contact on the API
        public async Task<ContactDTO> CreateContactAsync(string clientId, string tokenPass, ContactDTO contact)
        {
            var token = await AuthenticateAsync(clientId, tokenPass);


            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiUrl}/payout/contact.php");
                request.Headers.Add("Authorization", $"Bearer {token}");

                string jsonBody = JsonConvert.SerializeObject(contact);
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ContactDTO>(jsonResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could'nt add the contact: {ex.Message}");
                throw;
            }
        }

        //Creates a new payment on the API
        public async Task<PaymentDTO> CreatePaymentAsync(string clientId, string tokenPass, PaymentDTO payment)
        {
            var token = await AuthenticateAsync(clientId, tokenPass);

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, $"{_apiUrl}/payout/payout.php");
                request.Headers.Add("Authorization", $"Bearer {token}");

                string jsonBody = JsonConvert.SerializeObject(payment);
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PaymentDTO>(jsonResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could'nt generate the new payment: {ex.Message}");
                throw;
            }
        }


        //Get all payments from the API
        public async Task<List<PaymentDTO>> GetAllPaymentsAsync(string clientId, string tokenPass)
        {
            var token = await AuthenticateAsync(clientId, tokenPass);

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiUrl}/payout/payout.php");
                request.Headers.Add("Authorization", $"Bearer {token}");

                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<PaymentDTO>>(jsonResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could'nt get payments: {ex.Message}");
                throw;
            }
        }

    }
}