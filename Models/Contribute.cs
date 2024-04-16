using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShelf.Models
{
    public class Contribute
    {
        public int contribute_id { get; set; }
        public int user_id { get; set; }
        public string book_name { get; set; }
        public string book_image { get; set; }
        public string book_author { get; set; }
        public string book_type { get; set; }
        public string book_language { get; set; }
        public string book_format { get; set; }
        public string reason { get; set; }
    }
}
