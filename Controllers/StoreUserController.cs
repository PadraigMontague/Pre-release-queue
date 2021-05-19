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
    public class StoreUserController : ControllerBase
    {

        private DatabaseConnection DatabaseConnection { get; set; }
        private DatabaseAuth DatabaseAuth { get; set; }
        private AuthenticationController AuthenticationController { get; set; }
        public StoreUserController(DatabaseConnection databaseConnection, DatabaseAuth databaseAuth)
        {
            this.DatabaseConnection = databaseConnection;
            this.DatabaseAuth = databaseAuth;
            this.AuthenticationController = new AuthenticationController(databaseConnection, databaseAuth);
        }

        public List<StoreUser> stores = new List<StoreUser>() { };

        [HttpGet("GetAllStores")]
        public List<StoreUser> GetAllStores()
        {
            try
            {
                string token = Request.Form["token"];
                bool isValid = this.AuthenticationController.CheckTokenStatus(token);
                if (isValid)
                {
                    var cmd = this.DatabaseConnection.Connection.CreateCommand() as MySqlCommand;
                    cmd.CommandText = @"SELECT * FROM store_users";
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows == false)
                        {
                            return stores;
                        }
                        while (reader.Read())
                        {
                            var id = reader.GetInt32("storeID");
                            var storeName = reader.GetString("storename");
                            var address = reader.GetString("address");
                            stores.Add(new StoreUser(id, storeName, address));
                        }
                    }

                    return stores;
                }
                else
                {
                    return stores;
                }
            }
            catch
            {
                stores.Add(new StoreUser(0, "Warning", "Please enter all requested data"));
                return stores;
            }
        }


        [HttpPost("CreateStore")]
        public string CreateStore()
        {
            string response = null;
            string storeName = this.DatabaseAuth.preparedStatement(Request.Form["storeName"]);
            string address = this.DatabaseAuth.preparedStatement(Request.Form["address"]);
            string password = this.DatabaseAuth.preparedStatement(Request.Form["password"]);
            if (validateForm(storeName, address, password).Code == 200)
            {
                var cmd = this.DatabaseConnection.Connection.CreateCommand() as MySqlCommand;
                cmd.CommandText = @"INSERT INTO store_users(storename, address, password) VALUES (@StoreName, @Address, @Password)";
                cmd.Parameters.AddWithValue("@StoreName", storeName);
                cmd.Parameters.AddWithValue("@Address", address);
                cmd.Parameters.AddWithValue("@Password", Convert.ToBase64String(KeyDerivation.Pbkdf2(password: password, salt: new byte[128 / 8], prf: KeyDerivationPrf.HMACSHA1, iterationCount: 10000, numBytesRequested: 256 / 8)));

                var recs = cmd.ExecuteNonQuery();
                var check = recs == 1 ? response = "OK" : response = "Error";
            }
            else
            {
                response = validateForm(storeName, address, password).Status;
                return response;
            }
            return response;
        }

        [HttpPost("ChangePassword")]
        public string ChangePassword()
        {
            string response = null;
            string storename = this.DatabaseAuth.preparedStatement(Request.Form["storename"]);
            string password = this.DatabaseAuth.preparedStatement(Request.Form["password"]);
            string newPassword = this.DatabaseAuth.preparedStatement(Request.Form["new_password"]);
            StatusResponseGenerator result = validateUser(storename, password);
            if (result.Status == "Authorised")
            {
                var cmd = this.DatabaseConnection.Connection.CreateCommand() as MySqlCommand;
                cmd.CommandText = @"UPDATE store_users SET password = @newPassword WHERE storename = @Storename";
                cmd.Parameters.AddWithValue("@Storename", storename);
                cmd.Parameters.AddWithValue("@newPassword", Convert.ToBase64String(KeyDerivation.Pbkdf2(password: newPassword, salt: new byte[128 / 8], prf: KeyDerivationPrf.HMACSHA1, iterationCount: 10000, numBytesRequested: 256 / 8)));
                var recs = cmd.ExecuteNonQuery();
                var check = recs == 1 ? response = "OK" : response = "Error";
            }
            return response;
        }
        public StatusResponseGenerator validateUser(string storename, string password)
        {
            if (storename == "" && password == "")
            {
                return new StatusResponseGenerator("Please complete the form", 401, DateTime.Now);
            }
            else
            {
                var cmd = this.DatabaseConnection.Connection.CreateCommand() as MySqlCommand;
                cmd.CommandText = @"SELECT COUNT(storeID) FROM store_users WHERE storename = @Storename AND password = @Password";
                cmd.Parameters.AddWithValue("@Storename", storename);
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

        public StatusResponseGenerator validateForm(string StoreName, string Address, string Password)
        {

            if (StoreName == "" && Address == "" && Password == "")
            {
                return new StatusResponseGenerator("Please complete the form", 401, DateTime.Now);
            }
            else if (StoreName == "" && Address != "" && Password != "")
            {
                return new StatusResponseGenerator("Please enter your storename", 401, DateTime.Now);
            }
            else if (StoreName != "" && Address == "" && Password != "")
            {
                return new StatusResponseGenerator("Please enter your address", 401, DateTime.Now);
            }
            else if (StoreName != "" && Address != "" && Password == "")
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
