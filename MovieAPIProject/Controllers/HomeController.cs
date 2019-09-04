﻿using System;
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
using Microsoft.EntityFrameworkCore;

namespace MovieAPIProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly MovieAPIDbContext _context;

        public HomeController(IConfiguration configuration, MovieAPIDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SearchResult(string query)
        {
            var splited = query.Split(' ');
            string resultQuery = "";
            int i = 0;
            foreach (var item in splited)
            {
                resultQuery += item;
                if (i != splited.Count() - 1)
                {
                    resultQuery += "%20";
                }

                i++;
            }
            var result = GetSearchByKeyWord(resultQuery, _configuration).Result;
            return View(result.results);
        }
        public IActionResult Details(int id)
        {
            var movie = GetMovieById(id, _configuration).Result;
            return View(movie);
        }

        public IActionResult AddMovieToFavorites(int id)
        {
            var movie = GetMovieById(id, _configuration).Result;
            AspNetUsers thisUser = _context.AspNetUsers.Where(u => u.UserName == User.Identity.Name).First();

            FavoriteMovies newFav = new FavoriteMovies();
            
            newFav.UserId = thisUser.Id;
            newFav.Movie = movie.title;
            newFav.MovieId = movie.id;

            if (ModelState.IsValid)
            {
                _context.FavoriteMovies.Add(newFav);
                _context.SaveChanges();
                return RedirectToAction("Index");

            }
            return View("Index");
        }

        public IActionResult GetFavoritesMovieList()
        {
            AspNetUsers thisUser = _context.AspNetUsers.Where(u => u.UserName == User.Identity.Name).First();
            List<FavoriteMovies> favoriteList = _context.FavoriteMovies.Where(u => u.UserId == thisUser.Id).ToList();
            return View(favoriteList);
        }

        public IActionResult RemoveFavorite(int id)
        {
            foreach (var item in _context.FavoriteMovies.ToList())
            {
                if(item.MovieId == id)
                {
                    _context.FavoriteMovies.Remove(item);
                }
            }
            _context.SaveChanges();
            return RedirectToAction("GetFavoritesMovieList");

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
        public static async Task<MovieList> GetSearchByKeyWord(string query, IConfiguration configuration)
        {

            var client = GetClient();
            string apiKey = configuration.GetSection("AppConfiguration")["ApiKey"];
            var response = await client.GetAsync($"search/movie?api_key={apiKey}&language=en-US&query={query}&page=1&include_adult=false");
            var result = await response.Content.ReadAsAsync<MovieList>();
            return result;
        }

    }
}
