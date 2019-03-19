using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ServiceStack;
using ServiceStack.Text;
// ReSharper disable InconsistentNaming

namespace Client
{
    class Program
    {

        public class Product
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public string Category { get; set; }
        }

        private static readonly HttpClient _client = new HttpClient();

        static async Task<Product> AddOrUpdateProductAsync(Product product)
        {
            //send the product information to the server
            var response = await _client.PutAsync(
                $"api/products/{product.Id}", new StringContent(product.ToJson(), Encoding.UTF8, "application/json"));
         
            //throw exception is something was wrong on the server
            response.EnsureSuccessStatusCode();

            //parse the results
            return JsonObject.Parse(await response.Content.ReadAsStringAsync()).ConvertTo<Product>();
        }


        static void Main(string[] args)
        {
            AddOrUpdateProductAsync(null);
            using (var client = new ServiceStack.JsonServiceClient("http://localhost:5000"))
            {
                var tags = client.Get<string[]>("/tags");
                var random = new Random();
                Console.WriteLine($"Process Id: {Process.GetCurrentProcess().Id}");
                Console.WriteLine("Press any key to start...");
                Console.ReadKey();

                Console.WriteLine("Started requests loop...");
                var mre = new ManualResetEventSlim();

                var tasks = new List<Task>();
                for (int k = 0; k < 15; k++)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        Console.WriteLine($"Starting task {Task.CurrentId}");
                        var tagsToQuery = new List<string>(10);
                        var sw = new Stopwatch();
                        while (!mre.IsSet)
                        {
                            var countOfTags = random.Next(3, 10);
                            tagsToQuery.Clear();
                            for (int i = 0; i < countOfTags; i++)
                            {
                                tagsToQuery.Add(tags[random.Next(0, tags.Length)]);
                            }

                            sw.Reset();
                            sw.Start();
                            var usersQuery = client.Get<User[]>($"/find-users2?tag={string.Join(",", tagsToQuery)}");
                            sw.Stop();
                            Console.WriteLine($"results: {usersQuery.Length}, latency: {sw.Elapsed}");
                        }
                    }));
                }

                Console.ReadKey();
                mre.Set();
                Task.WaitAll(tasks.ToArray());

                Console.WriteLine("Finished.");
            }
        }

        public class User
        {
            public string Id;
            public string Name;
            public string Email;
            public string Address;
            public string[] Tags;
        }
    }
}
