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
    public class ContributeController : Controller
    {
        public static List<Contribute> bookList = new List<Contribute>();
        private readonly IDbConnection _connection;

        public ContributeController(IDbConnection connection)
        {
            _connection = connection;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("/addcontribute")]
        [HttpPost]
        public IActionResult AddContribute([FromBody] Contribute contribute)
        {
            _connection.Open();
            using (var command = new SqlCommand("manage_contribute", (SqlConnection)_connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@action", "I");
                command.Parameters.AddWithValue("@user_id", contribute.user_id);
                command.Parameters.AddWithValue("@book_name", contribute.book_name);
                command.Parameters.AddWithValue("@book_author", contribute.book_author);
                command.Parameters.AddWithValue("@book_type", contribute.book_type);
                command.Parameters.AddWithValue("@book_language", contribute.book_language);
                command.Parameters.AddWithValue("@book_format", contribute.book_format);
                command.Parameters.AddWithValue("@reason", contribute.reason);
                command.ExecuteNonQuery();
            }
            _connection.Close();
            return Ok("Contribution Added");
        }

        [Route("/getcontribution/{id}")]
        [HttpGet]
        public IActionResult GetContributionList(int id)
        {
            try
            {
                bookList.Clear();
                _connection.Open();
                using (var command = new SqlCommand("manage_contribute", (SqlConnection)_connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "S");
                    command.Parameters.AddWithValue("@user_id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Contribute item = new Contribute
                            {
                                contribute_id = Convert.ToInt32(reader["contribute_id"]),
                                user_id = Convert.ToInt32(reader["user_id"]),
                                book_name = Convert.ToString(reader["book_name"]), // Implement this method
                                book_image = Convert.ToString(reader["book_image"]),
                                book_author = Convert.ToString(reader["book_author"]),
                                book_type = Convert.ToString(reader["book_type"]),
                                book_language = Convert.ToString(reader["book_language"]),
                                book_format = Convert.ToString(reader["book_format"]),
                                reason = Convert.ToString(reader["reason"])
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
    }
}