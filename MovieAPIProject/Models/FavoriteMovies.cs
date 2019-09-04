using System;
using System.Collections.Generic;

namespace MovieAPIProject.Models
{
    public partial class FavoriteMovies
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Movie { get; set; }
        public int MovieId { get; set; }
    }
}
