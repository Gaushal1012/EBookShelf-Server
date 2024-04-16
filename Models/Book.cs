using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShelf.Models
{
    public class Book
    {
        public int book_id { get; set; }
        public string book_name { get; set; }
        public string author { get; set; }
        public IFormFile book_image { get; set; }
        public IFormFile book_pdf { get; set; }
        public string book_img { get; set; }
        public string book_pdfFile { get; set; }
        public int pages { get; set; }
        public decimal rating { get; set; }
        public string description { get; set; }
        public availablites availability { get; set; }
        public categories category { get; set; }

        public enum categories
        {
            Adventure,
            Autobiography,
            Novel,
            Mystery,
            Science,
            FairyTales,
            Unknown
        }

        public enum availablites
        {
            Hardcopy,
            Ebook,
            Audiobook,
            Unknown
        }
    }
}
