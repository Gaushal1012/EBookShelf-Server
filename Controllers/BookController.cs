using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BookShelf.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BookShelf.Controllers
{
    [EnableCors("AllowReactApp")]
    public class BookController : Controller
    {
        public static List<Book> bookList = new List<Book>();
        private readonly IDbConnection _connection;

        public BookController(IDbConnection connection)
        {
            _connection = connection;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Route("/addbook")]
        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] Book book)
        {
            if (book != null && book.book_pdf.ContentType == "application/pdf" || book.book_image.ContentType == "application/png" || book.book_image.ContentType == "application/jpg")
            {
                var uploadsFolder = Path.Combine("wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(Path.Combine(uploadsFolder));

                var pdfPath = Path.Combine(uploadsFolder, "pdf", book.book_pdf.FileName);
                using (var fileStream = new FileStream(pdfPath, FileMode.Create))
                {
                    await book.book_pdf.CopyToAsync(fileStream); // Await here
                }

                var imgPath = Path.Combine(uploadsFolder, "image", book.book_image.FileName);
                using (var fileStream = new FileStream(imgPath, FileMode.Create))
                {
                    await book.book_image.CopyToAsync(fileStream); // Await here
                }

                _connection.Open();
                using (var command = new SqlCommand("manage_books", (SqlConnection)_connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "I");
                    command.Parameters.AddWithValue("@book_name", book.book_name);
                    command.Parameters.AddWithValue("@author", book.author);
                    command.Parameters.AddWithValue("@book_image", imgPath.Substring(7));
                    command.Parameters.AddWithValue("@book_pdf", pdfPath.Substring(7));
                    command.Parameters.AddWithValue("@pages", book.pages);
                    command.Parameters.AddWithValue("@rating", book.rating);
                    command.Parameters.AddWithValue("@description", book.description);
                    command.Parameters.AddWithValue("@availability", book.availability.ToString());
                    command.Parameters.AddWithValue("@category", book.category.ToString());
                    command.ExecuteNonQuery();
                }

                return Ok("Book Added");
            }

            return BadRequest("Invalid file type");
        }


        [Route("/getbook")]
        public IActionResult GetAllBooks()
        {
            try
            {
                bookList.Clear();
                List<Book> books = new List<Book>();
                _connection.Open();
                using (var command = new SqlCommand("SELECT * FROM Book", (SqlConnection)_connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Book book = new Book
                                {
                                    book_id = Convert.ToInt32(reader["book_id"]),
                                    book_name = reader["book_name"].ToString(),
                                    author = reader["author"].ToString(),
                                    pages = Convert.ToInt32(reader["pages"]),
                                    rating = Convert.ToDecimal(reader["rating"]),
                                    description = reader["description"].ToString(),
                                    book_img = reader["book_image"].ToString(),
                                    book_pdfFile = reader["book_pdf"].ToString(),
                                    availability = reader["availability"] == DBNull.Value ? Book.availablites.Unknown : (Book.availablites)Enum.Parse(typeof(Book.availablites), reader["availability"].ToString()),
                                    category = reader["category"] == DBNull.Value ? Book.categories.Unknown : (Book.categories)Enum.Parse(typeof(Book.categories), reader["category"].ToString())
                                };
                            books.Add(book);
                            }
                        }
                    }
                return Ok(new { message = "book finded", books } );
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("/book/{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            try
            {
                bookList.Clear();
                _connection.Open();
                using (var command = new SqlCommand("manage_books", (SqlConnection)_connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "S");
                    command.Parameters.AddWithValue("@book_id", id);
                    command.Parameters.AddWithValue("@book_name", DBNull.Value);
                    command.Parameters.AddWithValue("@author", DBNull.Value);
                    command.Parameters.AddWithValue("@book_image", DBNull.Value);
                    command.Parameters.AddWithValue("@book_pdf", DBNull.Value);
                    command.Parameters.AddWithValue("@pages", DBNull.Value);
                    command.Parameters.AddWithValue("@rating", DBNull.Value);
                    command.Parameters.AddWithValue("@description", DBNull.Value);
                    command.Parameters.AddWithValue("@availability", DBNull.Value);
                    command.Parameters.AddWithValue("@category", DBNull.Value);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Book book = new Book
                            {
                                book_id = Convert.ToInt32(reader["book_id"]),
                                book_name = reader["book_name"].ToString(),
                                author = reader["author"].ToString(),
                                pages = Convert.ToInt32(reader["pages"]),
                                rating = Convert.ToDecimal(reader["rating"]),
                                description = reader["description"].ToString(),
                                availability = reader["availability"] == DBNull.Value ? Book.availablites.Unknown : (Book.availablites)Enum.Parse(typeof(Book.availablites), reader["availability"].ToString()),
                                category = reader["category"] == DBNull.Value ? Book.categories.Unknown : (Book.categories)Enum.Parse(typeof(Book.categories), reader["category"].ToString())
                            };

                            book.book_img = reader["book_image"].ToString();
                            book.book_pdfFile = reader["book_pdf"].ToString();

                            bookList.Add(book);
                        }
                    }
                }
                return Ok(new { message = "book finded", bookList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
            finally
            {
                _connection.Close();
            }
        }

        [HttpGet]
        [Route("/booklist/{author}")]
        public async Task<IActionResult> GetBookImageByAuthor(string author)
        {
            try
            {
                bookList.Clear();
                _connection.Open();
                using (var command = new SqlCommand("manage_books", (SqlConnection)_connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "SELECTBYAUTHOR");
                    command.Parameters.AddWithValue("@book_id", DBNull.Value);
                    command.Parameters.AddWithValue("@book_name", DBNull.Value);
                    command.Parameters.AddWithValue("@author", author);
                    command.Parameters.AddWithValue("@book_image", DBNull.Value);
                    command.Parameters.AddWithValue("@book_pdf", DBNull.Value);
                    command.Parameters.AddWithValue("@pages", DBNull.Value);
                    command.Parameters.AddWithValue("@rating", DBNull.Value);
                    command.Parameters.AddWithValue("@description", DBNull.Value);
                    command.Parameters.AddWithValue("@availability", DBNull.Value);
                    command.Parameters.AddWithValue("@category", DBNull.Value);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Book book = new Book
                            {
                                //book_img = reader["book_img"].ToString()
                            };
                            book.book_img = reader["book_image"].ToString();

                            bookList.Add(book);
                        }
                    }
                }
                return Ok(new { message = "book image finded", bookList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}