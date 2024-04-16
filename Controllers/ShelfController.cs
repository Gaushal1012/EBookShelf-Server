using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BookShelf.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookShelf.Controllers
{
    public class ShelfController : Controller
    {
        public static List<Shelf> bookList = new List<Shelf>();
        private readonly IDbConnection _connection;

        public ShelfController(IDbConnection connection)
        {
            _connection = connection;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Route("/getshelf/{id}")]
        [HttpGet]
        public IActionResult GetShelfList(int id)
        {
            try
            {
                bookList.Clear();
                _connection.Open();
                using (var command = new SqlCommand("manage_shelf", (SqlConnection)_connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "S");
                    command.Parameters.AddWithValue("@emp_id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Shelf item = new Shelf
                            {
                                shelf_id = Convert.ToInt32(reader["shelf_id"]),
                                emp_id = Convert.ToInt32(reader["emp_id"]),
                                book_id = Convert.ToInt32(reader["book_id"]), // Implement this method
                                book_img = Convert.ToString(reader["book_img"]),
                                book_name = Convert.ToString(reader["book_name"]),
                                issuedate = Convert.ToDateTime(reader["issuedate"]),
                                returndate = Convert.ToDateTime(reader["returndate"]),
                                description = Convert.ToString(reader["description"])
                            };
                            bookList.Add(item);
                        }
                    }
                }
                return Ok(new { message = "user added", bookList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error finding bookshelf: {ex.Message}");
            }
            finally
            {
                _connection.Close();
            }
        }

        [Route("/Shelf")]
        [HttpPost]
        public async Task<IActionResult> UploadFile([FromBody] Shelf book)
        {
            _connection.Open();
            using (var command = new SqlCommand("manage_shelf", (SqlConnection)_connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@action", "I");
                command.Parameters.AddWithValue("@emp_id", book.emp_id);
                command.Parameters.AddWithValue("@book_id", book.book_id);
                command.Parameters.AddWithValue("@book_name", book.book_name);
                command.Parameters.AddWithValue("@book_img", book.book_img);
                command.Parameters.AddWithValue("@issuedate", book.issuedate);
                command.Parameters.AddWithValue("@returndate", book.returndate);
                command.Parameters.AddWithValue("@description", book.description);
                command.ExecuteNonQuery();
            }
            return Ok("Book Added in shelf");
        }
    }
}