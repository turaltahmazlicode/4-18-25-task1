using static System.Net.WebRequestMethods;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

namespace ConsoleApp
{
    internal class Program
    {
        /*1.DirectoryInfo classi vasitesile bir Users adinda folder yaradin.
        2. FileInfo vasitesile hemin Users folderi daxilinde users.json yaradin.
        3. https://jsonplaceholder.typicode.com/users
        linke HttpClient vasitesile sorgu gonderin.Linkden gelen sorgunun contentini oxuyub, User obyektine deserialize edin. Deserialize etdiyiniz obyekti yuxarida yaratdiginiz users.json
        file-na yazin. (Streamwriter vasitesile)
        Taskin ne qederini bacarirsizsa yazin. Tam yaza bilmesezde bacardiginizi yazin.*/
        static readonly HttpClient client = new HttpClient();
        static async Task Main(string[] args)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(@"C:\Users\tural\source\repos\4-18-25-task1\ConsoleApp\Users");

            Console.WriteLine(directoryInfo.FullName);
            if (!directoryInfo.Exists)
                directoryInfo.Create();

            FileInfo fileInfo = new FileInfo(Path.Combine(directoryInfo.FullName, "users.json"));

            if (!fileInfo.Exists)
                using (fileInfo.Create())
                    ;

            string url = @"https://jsonplaceholder.typicode.com/users";
            string? json = await GetData(url);

            if (json is null)
            {
                Console.WriteLine("Failed to retrieve data.");
                return;
            }

            JArray users = JsonConvert.DeserializeObject<JArray>(json);

            if (users is null)
            {
                Console.WriteLine("Failed to parse JSON.");
                return;
            }

            var user = users[0];

            var userObj = JsonConvert.DeserializeObject<User>(user.ToString());

            if (userObj != null)
            {
                string userJson = JsonConvert.SerializeObject(userObj, Formatting.Indented);
                using (StreamWriter writer = new StreamWriter(fileInfo.FullName))
                    writer.WriteLine(userJson);
            }
        }

        static async Task<string?> GetData(string url)
        {
            try
            {
                using HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }

        public class Rootobject
        {
            public User[] users { get; set; }
        }

        public class User
        {
            public int id { get; set; }
            public string name { get; set; }
            public string username { get; set; }
            public string email { get; set; }
            public Address address { get; set; }
            public string phone { get; set; }
            public string website { get; set; }
            public Company company { get; set; }
        }

        public class Address
        {
            public string street { get; set; }
            public string suite { get; set; }
            public string city { get; set; }
            public string zipcode { get; set; }
            public Geo geo { get; set; }
        }

        public class Geo
        {
            public string lat { get; set; }
            public string lng { get; set; }
        }

        public class Company
        {
            public string name { get; set; }
            public string catchPhrase { get; set; }
            public string bs { get; set; }
        }
    }
}
