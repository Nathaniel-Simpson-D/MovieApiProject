using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols;
using MovieAPIProject.Models;
using Microsoft.Extensions.Configuration;


namespace MovieAPIProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuation;
       

        public HomeController(IConfiguration configuration)
        {
            _configuation = configuration;
        }
        public IActionResult Index()
        {
           
            return View();
        }
        public IActionResult SearchResult(string query)
        {
            var splited = query.Split(' ');
            string resultQuery = splited[0];
            int i = 0;
            foreach(var item in splited)
            {
                if (i != 0)
                {
                    resultQuery += "+";
                    resultQuery += item;
                }
            }
            var result = GetSearchByKeyWord(resultQuery, _configuation).Result;
            return View(result);
        }
        public IActionResult Details(int id)
        {
            var movie = GetMovieById(id, _configuation).Result;
            return View(movie);
        }
        public static HttpClient GetClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.themoviedb.org/3/");
            return client;
        }
        public static async Task<Movie> GetMovieById(int id, IConfiguration configuration)
        {
            var client = GetClient();
            string apiKey = configuration.GetSection("AppConfiguration")["ApiKey"];
            var response = await client.GetAsync($"movie/{id}?api_key={apiKey}");
            var result = await response.Content.ReadAsAsync<Movie>();
            return result;
        }
        public static async Task<Movie> GetSearchByKeyWord(string search,IConfiguration configuration)
        {
            var client = GetClient();
            string apiKey = configuration.GetSection("AppConfiguration")["ApiKey"];
            var response = await client.GetAsync($"Search/Movie?api_key={apiKey}&query={search}");
            var result = await response.Content.ReadAsAsync<Movie>();
            return result;
        }
        
    }
}
