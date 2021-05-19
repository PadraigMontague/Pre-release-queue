using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MySql.Data.MySqlClient;
using Pre_Release_Que.Database;
using Pre_Release_Que.StatusResponse;

namespace Pre_Release_Que.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AuthenticationController : ControllerBase
    {

        private DatabaseConnection DatabaseConnection { get; set; }
        private DatabaseAuth DatabaseAuth { get; set; }

        public AuthenticationController(DatabaseConnection databaseConnection, DatabaseAuth databaseAuth)
        {
            this.DatabaseConnection = databaseConnection;
            this.DatabaseAuth = databaseAuth;
        }

        [HttpPost("Login")]
        public GenerateToken Login()
        {
            string username = this.DatabaseAuth.preparedStatement(Request.Form["username"]);
            string password = this.DatabaseAuth.preparedStatement(Request.Form["password"]);
            string convertedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(password: password, salt: new byte[128 / 8], prf: KeyDerivationPrf.HMACSHA1, iterationCount: 10000, numBytesRequested: 256 / 8));
            string type = this.DatabaseAuth.preparedStatement(Request.Form["type"]);
            if (validateLogin(username, password, type).Code == 200)
            {
                var cmd = this.DatabaseConnection.Connection.CreateCommand() as MySqlCommand;
                if (type == "store")
                {
                    cmd.CommandText = @"SELECT * FROM store_users WHERE password = @Password AND storename = @StoreName";
                    cmd.Parameters.AddWithValue("@Password", convertedPassword);
                    cmd.Parameters.AddWithValue("@StoreName", username);
                }
                else
                {
                    cmd.CommandText = @"SELECT * FROM users WHERE password = @Password AND username = @Username";
                    cmd.Parameters.AddWithValue("@Password", convertedPassword);
                    cmd.Parameters.AddWithValue("@Username", username);
                }

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    var generatedToken = GenerateToken(reader, type);
                    return generatedToken;
                }
            }
            else
            {
                string tokenBody = "Please enter all requested data";
                return new GenerateToken(tokenBody);
            }
        }

        public StatusResponseGenerator validateLogin(string username, string password, string type)
        {

            if (username == "" && password == "" && type == "")
            {
                return new StatusResponseGenerator("Please complete the form", 401, DateTime.Now);
            }
            else if (username == "" && password != "" && type != "")
            {
                return new StatusResponseGenerator("Please enter your username", 401, DateTime.Now);
            }
            else if (username != "" && password == "" && type != "")
            {
                return new StatusResponseGenerator("Please enter your password", 401, DateTime.Now);
            }
            else if (username != "" && password != "" && type == "")
            {
                return new StatusResponseGenerator("Please enter your user type", 401, DateTime.Now);
            }
            else
            {
                return new StatusResponseGenerator("OK", 200, DateTime.Now);
            }
        }

        public bool CheckTokenStatus(string token)
        {
            try
            {
                if (token == "")
                {
                    return false;
                }
                else
                {
                    try
                    {
                        string tokenData = this.DatabaseAuth.preparedStatement(token);
                        string rawToken = DecodingData(tokenData);
                        var generateToken = JsonSerializer.Deserialize<List<CloneToken>>(rawToken);
                        var head = generateToken[0];
                        int res = DateTime.Compare(DateTime.Parse(head.expiry_date), DateTime.Now);
                        string username = head.username;
                        Console.WriteLine(username);
                        int userExist = usernameExists(username);
                        int storeExist = storenameExists(username);

                        if (res > 0 && userExist == 1)
                        {
                            Console.WriteLine("Valid");
                            return true;
                        }
                        else if (res > 0 && storeExist == 1)
                        {
                            Console.WriteLine("Valid");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("Invalid");
                            return false;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public GenerateToken GenerateToken(MySqlDataReader reader, string type)
        {
            DateTime valid = DateTime.Now;
            string tokenBody = "Unauthorised Access";

            if (reader.HasRows == false)
            {
                Console.WriteLine(reader.HasRows);
                return new GenerateToken(tokenBody);
            }
            while (reader.Read())
            {
                string id = "";
                string user = "";
                if (type == "user")
                {
                    id = reader.GetInt32("userID").ToString();
                    user = reader.GetString("username");
                }
                else
                {
                    id = reader.GetInt32("storeID").ToString();
                    user = reader.GetString("storename");
                }
                var username = Convert.ToBase64String(KeyDerivation.Pbkdf2(password: user, salt: new byte[128 / 8], prf: KeyDerivationPrf.HMACSHA1, iterationCount: 10000, numBytesRequested: 256 / 8));
                DateTime dateCreated = valid;
                DateTime validTo = valid.AddHours(1);
                string status = "false";
                var token = new List<Authentication>() { new Authentication(Environment.GetEnvironmentVariable("SERTVICE_ISSUER"), Environment.GetEnvironmentVariable("TOKEN_SECRET"), DateTime.Now, id, user, dateCreated, validTo, status) };
                string result = EncodingData(JsonSerializer.Serialize(token));
                var geneneratedToken = new GenerateToken(result);
                return geneneratedToken;
            }

            return new GenerateToken(tokenBody);
        }

        public string EncodingData(string tokenData)
        {
            var tokenDataBytes = System.Text.Encoding.UTF8.GetBytes(tokenData);
            return System.Convert.ToBase64String(tokenDataBytes);
        }

        public string DecodingData(string tokenData)
        {
            var data = System.Convert.FromBase64String(tokenData);
            return System.Text.Encoding.UTF8.GetString(data);
        }
        public int usernameExists(string username)
        {
            var cmd = this.DatabaseConnection.Connection.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT COUNT(userID) FROM users WHERE username = @Username";
            cmd.Parameters.AddWithValue("@Username", username);
            int count = (int)(long)cmd.ExecuteScalar();
            return count;
        }

        public int storenameExists(string username)
        {
            var cmd = this.DatabaseConnection.Connection.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT COUNT(storeID) FROM store_users WHERE storename = @Username";
            cmd.Parameters.AddWithValue("@Username", username);
            int count = (int)(long)cmd.ExecuteScalar();
            return count;
        }
    }

}