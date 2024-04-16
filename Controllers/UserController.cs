using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShelf.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BookShelf.Controllers
{
    [EnableCors("AllowReactApp")]
    public class UserController : Controller
    {
        public static List<User> userList = new List<User>();
        private readonly IDbConnection _connection;

        public UserController(IDbConnection connection)
        {
            _connection = connection;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("/login")]
        public IActionResult LoginUser([FromBody]Login login)
        {
            userList.Clear();
            try
            {
                _connection.Open();
                using (var command = new SqlCommand("manage_employee", (SqlConnection)_connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    command.CommandType = CommandType.StoredProcedure;

                    // Set action parameter for stored procedure
                    command.Parameters.AddWithValue("@action", "S");

                    // Encode password before passing it to the database
                    command.Parameters.AddWithValue("@password", Encodebase64(login.password));

                    // Set email parameter for stored procedure
                    command.Parameters.AddWithValue("@email", login.email);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return Ok(new { message = "user login successfully" });
                        }
                        else
                        {
                            return NotFound(new { message = "user not found" });
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error adding project: {ex.Message}");
            }
            finally
            {
                _connection.Close();
            }
        }

        [HttpGet]
        [Route("/find/{email}")]
        public IActionResult FindUser(string email)
        {
            _connection.Open();
            try
            {
                User userInfo = null;
                var query = "SELECT * from  Employee where email = @email";
                using (var command = new SqlCommand(query, (SqlConnection)_connection))
                {
                    // Set email parameter for stored procedure
                    command.Parameters.AddWithValue("@email", email);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            userInfo = new User
                            {
                                userId = Convert.ToInt32(reader["user_id"]),
                                username = Convert.ToString(reader["username"]),
                                password = DecodeBase64((string)reader["password"]), // Implement this method
                                email = Convert.ToString(reader["email"]),
                                bio = "I am student",
                                reading = Convert.ToInt32(reader["reading"]),
                                contribute = Convert.ToInt32(reader["contribution"]),
                                profileurl = Convert.ToString(reader["profilepicture"]),
                                role = (User.roles)Enum.Parse(typeof(User.roles), reader["role"].ToString())
                            };
                            break;
                        }
                    }
                }
                if (userInfo != null)
                {
                    return Ok(new { message = "user found", userInfo });
                }
                else
                {
                    return NotFound(new { message = "user not found" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error finding user: {ex.Message}");
            }
            finally
            {
                _connection.Close();
            }
        }


        [HttpPost]
        [Route("/AddUsers")]
        public IActionResult AddUser([FromForm]User users)
        {
            _connection.Open();
            try
            {
                using (var command = new SqlCommand("manage_employee", (SqlConnection)_connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "I");
                    command.Parameters.AddWithValue("@username", users.username);
                    command.Parameters.AddWithValue("@password", Encodebase64(users.password));
                    command.Parameters.AddWithValue("@email", users.email);
                    command.Parameters.AddWithValue("@bio", users.bio);
                    command.Parameters.AddWithValue("@reading", users.reading);
                    command.Parameters.AddWithValue("@contribution", users.contribute);
                    command.Parameters.AddWithValue("@profilepicture", users.profileimg);
                    command.Parameters.AddWithValue("@role", users.role.ToString());

                    command.ExecuteNonQuery();
                }

                return Ok(new { message = "user added" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error adding project: {ex.Message}");
            }
            finally
            {
                _connection.Close();
            }
        }

        [HttpPut]
        [Route("/UpdateUsers")]
        public async Task<IActionResult> UpdateUser([FromForm]User users)
        {
            _connection.Open();
            try
            {
                var uploadsFolder = Path.Combine("wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(Path.Combine(uploadsFolder));

                var pdfPath = Path.Combine(uploadsFolder, "profile", users.profileimg.FileName);
                using (var fileStream = new FileStream(pdfPath, FileMode.Create))
                {
                    await users.profileimg.CopyToAsync(fileStream); // Await here
                }

                using (var command = new SqlCommand("manage_employee", (SqlConnection)_connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "U");
                    command.Parameters.AddWithValue("@user_id", users.userId);
                    command.Parameters.AddWithValue("@username", users.username);
                    command.Parameters.AddWithValue("@password", Encodebase64(users.password));
                    command.Parameters.AddWithValue("@email", users.email);
                    command.Parameters.AddWithValue("@bio", users.bio);
                    command.Parameters.AddWithValue("@reading", users.reading);
                    command.Parameters.AddWithValue("@contribution", users.contribute);
                    command.Parameters.AddWithValue("@profilepicture", pdfPath.Substring(7));
                    command.Parameters.AddWithValue("@role", users.role.ToString());

                    await command.ExecuteNonQueryAsync(); // ExecuteNonQueryAsync is asynchronous
                }

                return Ok(new { message = "user added" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error adding project: {ex.Message}");
            }
            finally
            {
                _connection.Close();
            }
        }


        private string Encodebase64(string password)
        {
            byte[] bytesToEncode = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(bytesToEncode);

        }

        private string DecodeBase64(string encodedPassword)
        {
            byte[] bytes = Convert.FromBase64String(encodedPassword);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}