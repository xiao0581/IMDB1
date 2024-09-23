using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDB1.Model
{
    public class Genres
    {
  
        public string TConst { get; set; }
        public string Genre { get; set; }
        public Title Title { get; set; }


        public override string ToString()
        {
            return $" {TConst} {Genre}";
        }
    }
}
