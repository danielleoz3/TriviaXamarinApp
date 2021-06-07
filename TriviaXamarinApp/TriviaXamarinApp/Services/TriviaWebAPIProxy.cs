using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TriviaXamarinApp.Models;
using System.Text;
using System.Text.Json;

namespace TriviaXamarinApp.Services
{
    class TriviaWebAPIProxy
    {
        private HttpClient client;
        private string baseUri;
        private static TriviaWebAPIProxy proxy = null;

        // singleton
        public static TriviaWebAPIProxy CreateProxy(string baseUri = "https://ramonws.azurewebsites.net/AmericanQuestions/")
        {
            if (proxy == null)
                proxy = new TriviaWebAPIProxy(baseUri);
            return proxy;
        }

        private TriviaWebAPIProxy(string baseUri)
        {
            //Set client handler to support cookies!!
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = new System.Net.CookieContainer();

            //Create client with the handler!
            this.client = new HttpClient(handler, true);
            this.baseUri = baseUri;
        }

        //Login - if email and password are correct User object is returned. otherwise a null will be returned
        public async Task<User> LoginAsync(string email, string pass)
        {
            try
            {
                HttpResponseMessage response = await this.client.GetAsync($"{this.baseUri}/Login?email={email}&pass={pass}");
                if (response.IsSuccessStatusCode)
                {
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    string content = await response.Content.ReadAsStringAsync();
                    User u = JsonSerializer.Deserialize<User>(content, options);
                    return u;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        //This method gets from the server a random question if it does not suceed it returns null (no previous login is required)
        public async Task<AmericanQuestion> GetRandomQuestion()
        {
            try
            {
                HttpResponseMessage response = await this.client.GetAsync($"{this.baseUri}/GetRandomQuestion");
                if (response.IsSuccessStatusCode)
                {
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    string content = await response.Content.ReadAsStringAsync();
                    AmericanQuestion q = JsonSerializer.Deserialize<AmericanQuestion>(content, options);
                    if (q.QText == null || q.CorrectAnswer == null || (q.OtherAnswers.Length < 3))
                        return await GetRandomQuestion();
                    return q;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        //This method post new question to the server. A previous login is required! The nick name in the question must match the logged in user!
        //it returns true is succeeded or false otherwise
        public async Task<bool> PostNewQuestion(AmericanQuestion q)
        {
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                string json = JsonSerializer.Serialize<AmericanQuestion>(q, options);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await this.client.PostAsync($"{this.baseUri}/PostNewQuestion",content);
                if (response.IsSuccessStatusCode)
                {
                    
                    string jsonContent = await response.Content.ReadAsStringAsync();
                    bool  b = JsonSerializer.Deserialize<bool>(jsonContent, options);
                    return b;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        //This method delete (!) a question in the server database. A previous login is required! The nick name in the question must match the logged in user!
        //it returns true is succeeded or false otherwise
        public async Task<bool> DeleteQuestion(AmericanQuestion q)
        {
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                string json = JsonSerializer.Serialize<AmericanQuestion>(q, options);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await this.client.PostAsync($"{this.baseUri}/DeleteQuestion", content);
                if (response.IsSuccessStatusCode)
                {

                    string jsonContent = await response.Content.ReadAsStringAsync();
                    bool b = JsonSerializer.Deserialize<bool>(jsonContent, options);
                    return b;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        //This method register a new user into the server database. A previous login is NOT required! The nick name and email must be uniqe!
        //it returns true is succeeded or false otherwise
        //questions are ignored upon registering a user and shoul dbe added seperatly.
        //if succeeded - the user is automatically logged in on the server
        public async Task<bool> RegisterUser(User u)
        {
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                string json = JsonSerializer.Serialize<User>(u, options);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await this.client.PostAsync($"{this.baseUri}/RegisterUser", content);
                if (response.IsSuccessStatusCode)
                {

                    string jsonContent = await response.Content.ReadAsStringAsync();
                    bool b = JsonSerializer.Deserialize<bool>(jsonContent, options);
                    return b;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

    }
}
