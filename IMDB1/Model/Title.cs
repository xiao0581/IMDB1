using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDB1.Model
{
    public class Title
    {
        public string TConst { get; set; }

        public string TitleType { get; set; }
        public string? PrimaryTitle { get; set; }
        public string? OriginalTitle { get; set; }
        public bool? IsAdult { get; set; }
        public int? StartYear { get; set; }
        public int? EndYear { get; set; }
        public int? RuntimeMinutes { get; set; }
        
        public List<Genres> Genres { get; set; }

        public override string ToString()
        {
            string genresString = Genres != null && Genres.Count > 0
              ? string.Join(", ", Genres.Select(g => g.Genre))
              : "No genres";

            return $"TConst: {TConst}, PrimaryTitle: {PrimaryTitle}, OriginalTitle: {OriginalTitle}, " +
                   $"IsAdult: {IsAdult}, StartYear: {StartYear}, EndYear: {EndYear}, " +
                   $"RuntimeMinutes: {RuntimeMinutes}, Genres: {genresString}";
        }
    }
}
