using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Server
{
    public class UsersController : Controller
    {
        private static IEnumerable<User> _users;     

        [HttpGet, Route("/find-users1")]
        public JsonResult FindUsers1()
        {
            var tags = Request.Query["tag"].ToString().Split(',');
            var results = from u in GetUsers()
                where tags.Any(tag => u.Tags.Contains(tag))
                select u;

            return Json(results);
        }

        [HttpGet, Route("/find-users2")]
        public JsonResult FindUsers2()
        {
            var tags = new HashSet<string>(Request.Query["tag"].ToString().Split(','));
            var results = from u in GetUsers()
                where tags.Overlaps(new HashSet<string>(u.Tags))
                select u;

            return Json(results);
        }

        [HttpGet, Route("/tags")]
        public JsonResult GetTags() => Json(new HashSet<string>(GetUsers().SelectMany(x => x.Tags)));

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static IEnumerable<User> GetUsers()
        {
            if (_users == null)
            {
                using (var fileReader = System.IO.File.OpenText("users.json"))
                using (var jsonReader = new JsonTextReader(fileReader))
                {
                    _users = JToken.ReadFrom(jsonReader).Select(x => x.ToObject<User>()).ToArray();
                }
            }

            return _users;
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
