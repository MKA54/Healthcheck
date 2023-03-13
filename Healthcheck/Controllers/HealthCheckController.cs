using Healthcheck.JSONParser;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Healthcheck.Controllers
{
    public class HealthCheckController : Controller
    {
        private string? _url;
        private int _secondsInterval;

        [Route("requestcheck")]
        [HttpPost]
        public void RequestCheckUrl([FromBody] object model)
        {
            var input = model.ToString();

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Input null."); 
            }

            var JSON = JsonEntity.Parse(input);

            try
            {
                _url =  JSON["url"].GetValue<string>();
                _secondsInterval = JSON["interval"].GetValue<int>();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            
            if (!UrlCheckPattern(_url))
            {
                Console.WriteLine("URL parsing error.");
                return;
            }

            if (_secondsInterval < 1)
            {
                Console.WriteLine("Interval < 1.");
                return;
            }

            try
            {
                TimerCallback tm = new(CheckUrl);
                Timer timer = new (tm, null, 0, _secondsInterval * 1000);
            }
            catch (NotSupportedException e)
            {
                Console.WriteLine("URL parsing error.");
            }
        }

        private static bool UrlCheckPattern(string url)
        {
            var regex = new Regex("^((http?)|(https?)|(ftp))://" +
                                    "(([a-z0-9$_\\.\\+!\\*\\'\\(\\),;\\?&=-]|%[0-9a-f]{2})+" +
                                    "(:([a-z0-9$_\\.\\+!\\*\\'\\(\\),;\\?&=-]|%[0-9a-f]{2})+)?" +
                                    "@)?(?#" +
                                    ")((([a-z0-9]\\.|[a-z0-9][a-z0-9-]*[a-z0-9]\\.)*" +
                                    "[a-z][a-z0-9-]*[a-z0-9]" +
                                    "|((\\d|[1-9]\\d|1\\d{2}|2[0-4][0-9]|25[0-5])\\.){3}" +
                                    "(\\d|[1-9]\\d|1\\d{2}|2[0-4][0-9]|25[0-5])" +
                                    ")(:\\d+)?" +
                                    ")(((\\/+([a-z0-9$_\\.\\+!\\*\\'\\(\\),;:@&=-]|%[0-9a-f]{2})*)*" +
                                    "(\\?([a-z0-9$_\\.\\+!\\*\\'\\(\\),;:@&=-]|%[0-9a-f]{2})*)" +
                                    "?)?)?" +
                                    "(#([a-z0-9$_\\.\\+!\\*\\'\\(\\),;:@&=-]|%[0-9a-f]{2})*)?" +
                                    "$");

            return regex.IsMatch(url);
        }

        private void CheckUrl(object? state)
        {
            using var httpClient = new HttpClient();
            using var request = new HttpRequestMessage(new HttpMethod("GET"), _url);
            var response = httpClient.SendAsync(request).Result;

            Console.WriteLine(response.ReasonPhrase + " (" + response.StatusCode + ")");
        }
    }
}
