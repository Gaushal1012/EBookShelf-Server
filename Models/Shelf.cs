using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShelf.Models
{
    public class Shelf
    {
        public int shelf_id { get; set; }
        public int emp_id { get; set; }
        public int book_id { get; set; }
        public string book_img { get; set; }
        public string book_name { get; set; }
        public DateTime issuedate { get; set; }
        public DateTime returndate { get; set; }
        public string description { get; set; }
    }
}
