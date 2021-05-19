using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MySql.Data.MySqlClient;
using Pre_Release_Que.Database;
using Pre_Release_Que.StatusResponse;

namespace Pre_Release_Que.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NormalUserController : ControllerBase
    {
        private DatabaseConnection DatabaseConnection { get; set; }
        private DatabaseAuth DatabaseAuth { get; set; }
        private AuthenticationController AuthenticationController;

        public NormalUserController(DatabaseConnection databaseConnection, DatabaseAuth databaseAuth)
        {
            this.DatabaseConnection = databaseConnection;
            this.DatabaseAuth = databaseAuth;
            this.AuthenticationController = new AuthenticationController(databaseConnection, databaseAuth);
        }

        public List<NormalUser> users = new List<NormalUser>() { };


        [HttpGet("GetAllUsers")]
        public List<NormalUser> Get()
        {
            try
            {
                string token = Request.Form["token"];
                bool isValid = this.AuthenticationController.CheckTokenStatus(token);
                if (isValid)
                {
                    var cmd = this.DatabaseConnection.Connection.CreateCommand() as MySqlCommand;
                    cmd.CommandText = @"SELECT * FROM users";
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows == false)
                        {
                            return users;
                        }
                        while (reader.Read())
                        {
                            var id = reader.GetInt32("userID");
                            var username = reader.GetString("username");
                            var firstname = reader.GetString("firstname");
                            var lastname = reader.GetString("lastname");
                            users.Add(new NormalUser(id, username, firstname, lastname));
                        }
                    }

                    return users;
                }
                else
                {
                    return users;
                }
            }
            catch
            {
                users.Add(new NormalUser(0, "Warning", "Please enter all requested data", "Warning"));
                return users;
            }
        }

        [HttpPost("CreateUser")]
        public string CreateUser()
        {
            string response = null;
            string username = this.DatabaseAuth.preparedStatement(Request.Form["username"]);
            string firstname = this.DatabaseAuth.preparedStatement(Request.Form["firstname"]);
            string lastname = this.DatabaseAuth.preparedStatement(Request.Form["lastname"]);
            string password = this.DatabaseAuth.preparedStatement(Request.Form["password"]);

            if (validateForm(username, firstname, lastname, password).Code == 200)
            {
                var cmd = this.DatabaseConnection.Connection.CreateCommand() as MySqlCommand;
                cmd.CommandText = @"INSERT INTO users(username, firstname, lastname, password) VALUES (@Username, @Firstname, @Lastname, @Password)";
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Firstname", firstname);
                cmd.Parameters.AddWithValue("@Lastname", lastname);
                cmd.Parameters.AddWithValue("@Password", Convert.ToBase64String(KeyDerivation.Pbkdf2(password: password, salt: new byte[128 / 8], prf: KeyDerivationPrf.HMACSHA1, iterationCount: 10000, numBytesRequested: 256 / 8)));
                var recs = cmd.ExecuteNonQuery();
                var check = recs == 1 ? response = "OK" : response = "Error";
            }
            else
            {
                response = validateForm(username, firstname, lastname, password).Status;
                return response;
            }
            return response;
        }

        [HttpPost("ChangePassword")]
        public string ChangePassword()
        {
            string response = null;
            string username = this.DatabaseAuth.preparedStatement(Request.Form["username"]);
            string password = this.DatabaseAuth.preparedStatement(Request.Form["password"]);
            string newPassword = this.DatabaseAuth.preparedStatement(Request.Form["new_password"]);
            StatusResponseGenerator result = validateUser(username, password);
            if (result.Status == "Authorised")
            {
                var cmd = this.DatabaseConnection.Connection.CreateCommand() as MySqlCommand;
                cmd.CommandText = @"UPDATE users SET password = @newPassword WHERE username = @Username";
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@newPassword", Convert.ToBase64String(KeyDerivation.Pbkdf2(password: newPassword, salt: new byte[128 / 8], prf: KeyDerivationPrf.HMACSHA1, iterationCount: 10000, numBytesRequested: 256 / 8)));
                var recs = cmd.ExecuteNonQuery();
                var check = recs == 1 ? response = "OK" : response = "Error";
            }
            return response;
        }
        public StatusResponseGenerator validateUser(string username, string password)
        {
            if (username == "" && password == "")
            {
                return new StatusResponseGenerator("Please complete the form", 401, DateTime.Now);
            }
            else
            {
                var cmd = this.DatabaseConnection.Connection.CreateCommand() as MySqlCommand;
                cmd.CommandText = @"SELECT COUNT(userID) FROM users WHERE username = @Username AND password = @Password";
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", Convert.ToBase64String(KeyDerivation.Pbkdf2(password: password, salt: new byte[128 / 8], prf: KeyDerivationPrf.HMACSHA1, iterationCount: 10000, numBytesRequested: 256 / 8)));
                int count = (int)(long)cmd.ExecuteScalar();
                if (count == 1)
                {
                    return new StatusResponseGenerator("Authorised", 200, DateTime.Now);
                }
                else
                {
                    return new StatusResponseGenerator("Unauthorised", 200, DateTime.Now);
                }
            }
        }
        public StatusResponseGenerator validateForm(string username, string firstname, string lastname, string password)
        {
            if (username == "" && firstname == "" && lastname == "" && password == "")
            {
                return new StatusResponseGenerator("Please complete the form", 401, DateTime.Now);
            }
            else if (username == "" && firstname != "" && lastname != "" && password != "")
            {
                return new StatusResponseGenerator("Please enter your username", 401, DateTime.Now);
            }
            else if (username != "" && firstname == "" && lastname != "" && password != "")
            {
                return new StatusResponseGenerator("Please enter your firstname", 401, DateTime.Now);
            }
            else if (username != "" && firstname != "" && lastname == "" && password != "")
            {
                return new StatusResponseGenerator("Please enter your lastname", 401, DateTime.Now);
            }
            else if (username != "" && firstname != "" && lastname != "" && password == "")
            {
                return new StatusResponseGenerator("Please enter your password", 401, DateTime.Now);
            }
            else
            {
                return new StatusResponseGenerator("OK", 200, DateTime.Now);
            }
        }
    }
}
