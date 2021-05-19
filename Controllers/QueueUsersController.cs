using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Pre_Release_Que.Database;
using Pre_Release_Que.StatusResponse;

namespace Pre_Release_Que.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QueueUsersController : ControllerBase
    {

        private DatabaseConnection DatabaseConnection { get; set; }
        private DatabaseAuth DatabaseAuth { get; set; }
        private AuthenticationController AuthenticationController { get; set; }
        public List<QueueUsers> queueUsers = new List<QueueUsers>() { };

        public QueueUsersController(DatabaseConnection databaseConnection, DatabaseAuth databaseAuth)
        {
            this.DatabaseConnection = databaseConnection;
            this.DatabaseAuth = databaseAuth;
            this.AuthenticationController = new AuthenticationController(databaseConnection, databaseAuth);
        }

        [HttpGet("GetQueue")]
        public List<QueueUsers> GetQueue()
        {
            try
            {
                string token = Request.Form["token"];
                bool isValid = this.AuthenticationController.CheckTokenStatus(token);
                if (isValid)
                {
                    string queueID = this.DatabaseAuth.preparedStatement(Request.Form["queueID"]);
                    if (queueID == "")
                    {
                        return queueUsers;
                    }
                    else
                    {
                        var cmd = this.DatabaseConnection.Connection.CreateCommand() as MySqlCommand;
                        cmd.CommandText = @"SELECT * FROM queueusers WHERE queueID = @QueueID ";
                        cmd.Parameters.AddWithValue("@QueueID", queueID);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows == false)
                            {
                                return queueUsers;
                            }
                            while (reader.Read())
                            {
                                int QueueID = reader.GetInt32("queueID");
                                int StoreID = reader.GetInt32("storeID");
                                int ProductID = reader.GetInt32("productID");
                                string ProductName = reader.GetString("productName");
                                string Username = reader.GetString("username");
                                string StoreName = reader.GetString("storeName");
                                string DateCreated = reader.GetString("dateCreated");
                                queueUsers.Add(new QueueUsers(QueueID, StoreID, ProductID, ProductName, Username, StoreName, DateCreated));
                            }
                        }
                        return queueUsers;
                    }
                }
                else
                {
                    return queueUsers;
                }
            }
            catch
            {
                queueUsers.Add(new QueueUsers(0, 0, 0, "ERROR", "WARNING", "Please check that all HTTP Body data is provided", "Unauthorised"));
                return queueUsers;
            }
        }

        [HttpPost("AddUser")]
        public string AddUser()
        {
            string response = null;
            try
            {
                string token = Request.Form["token"];
                bool isValid = this.AuthenticationController.CheckTokenStatus(token);

                if (isValid)
                {
                    int QueueID = Int32.Parse(this.DatabaseAuth.preparedStatement(Request.Form["queueID"]));
                    int StoreID = Int32.Parse(this.DatabaseAuth.preparedStatement(Request.Form["storeID"]));
                    int ProductID = Int32.Parse(this.DatabaseAuth.preparedStatement(Request.Form["productID"]));
                    string ProductName = this.DatabaseAuth.preparedStatement(Request.Form["productName"]);
                    string Username = this.DatabaseAuth.preparedStatement(Request.Form["username"]);
                    string StoreName = this.DatabaseAuth.preparedStatement(Request.Form["storeName"]);
                    DateTime DateCreated = DateTime.Now;
                    Console.WriteLine(ifDuplicateExists(Username, QueueID));
                    if (validateForm(QueueID, StoreID, ProductID, Username, ProductName, StoreName).Code == 200 && ifDuplicateExists(Username, QueueID) == false)
                    {
                        try
                        {
                            var cmd = this.DatabaseConnection.Connection.CreateCommand() as MySqlCommand;
                            cmd.CommandText = @"INSERT INTO queueusers(queueID, storeID, productID, username, storeName, productName, dateCreated) VALUES (@QueueID, @StoreID, @ProductID, @Username, @ProductName, @StoreName, @Date)";
                            cmd.Parameters.AddWithValue("@QueueID", QueueID);
                            cmd.Parameters.AddWithValue("@StoreID", StoreID);
                            cmd.Parameters.AddWithValue("@ProductID", ProductID);
                            cmd.Parameters.AddWithValue("@Username", Username);
                            cmd.Parameters.AddWithValue("@ProductName", ProductName);
                            cmd.Parameters.AddWithValue("@StoreName", StoreName);
                            cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                            var recs = cmd.ExecuteNonQuery();

                            if (recs == 1)
                            {
                                response = "OK";
                            }
                            else
                            {
                                response = "Error";
                            }
                        }
                        catch
                        {
                            response = "Unable to complete your request at the moment. Please try again later";
                        }
                    }
                    else
                    {
                        response = validateForm(QueueID, StoreID, ProductID, Username, ProductName, StoreName).Status;
                        return response;
                    }
                }
                else
                {
                    return response;
                }
            }
            catch
            {
                response = "Please check that all HTTP Body data is provided";
            }
            return response;
        }

        [HttpPost("RemoveUserFromQueue")]
        public string RemoveUserFromQueue()
        {
            string response = null;
            try
            {
                string token = Request.Form["token"];
                bool isValid = this.AuthenticationController.CheckTokenStatus(token);

                if (isValid)
                {
                    int QueueID = Int32.Parse(this.DatabaseAuth.preparedStatement(Request.Form["queueID"]));
                    string Username = this.DatabaseAuth.preparedStatement(Request.Form["username"]);

                    if (validateDeletion(QueueID, Username).Code == 200)
                    {
                        try
                        {
                            var cmd = this.DatabaseConnection.Connection.CreateCommand() as MySqlCommand;
                            cmd.CommandText = @"DELETE FROM queueusers WHERE queueID = @QueueID AND username = @Username";
                            cmd.Parameters.AddWithValue("@QueueID", QueueID);
                            cmd.Parameters.AddWithValue("@Username", Username);
                            var recs = cmd.ExecuteNonQuery();
                            var check = recs >= 1 ? response = "OK" : response = "Error";
                        }
                        catch
                        {
                            response = "Unable to complete your request at the moment. Please try again later";
                        }
                    }
                    else
                    {
                        response = validateDeletion(QueueID, Username).Status;
                        return response;
                    }
                }
                else
                {
                    return response;
                }
            }
            catch
            {
                response = "Please check that all HTTP Body data is provided";
            }
            return response;
        }

        public Boolean ifDuplicateExists(string username, int QueueID)
        {
            Boolean result = true;

            try
            {

                var cmd = this.DatabaseConnection.Connection.CreateCommand() as MySqlCommand;
                cmd.CommandText = @"SELECT * FROM queueusers WHERE queueID = @QueueID AND username = @Username";
                cmd.Parameters.AddWithValue("@QueueID", QueueID);
                cmd.Parameters.AddWithValue("@Username", username);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows == false)
                    {
                        return result = false;
                    }
                    else
                    {
                        return result = true;
                    }
                }
            }
            catch
            {
                return result;
            }
        }

        public StatusResponseGenerator validateForm(int QueueID, int StoreID, int ProductID, string Username, string ProductName, string StoreName)
        {

            if (QueueID == 0 && StoreID == 0 && ProductID == 0 && Username == "" && ProductName == "" && StoreName == "")
            {
                return new StatusResponseGenerator("Please complete the form", 401, DateTime.Now);
            }
            else if (QueueID == 0 && StoreID != 0 && ProductID != 0 && Username != "" && ProductName != "" && StoreName != "")
            {
                return new StatusResponseGenerator("Please enter the queue id", 401, DateTime.Now);
            }
            else if (QueueID != 0 && StoreID == 0 && ProductID != 0 && Username != "" && ProductName != "" && StoreName != "")
            {
                return new StatusResponseGenerator("Please enter the store id", 401, DateTime.Now);
            }
            else if (QueueID != 0 && StoreID != 0 && ProductID == 0 && Username != "" && ProductName != "" && StoreName != "")
            {
                return new StatusResponseGenerator("Please enter the product id", 401, DateTime.Now);
            }
            else if (QueueID != 0 && StoreID != 0 && ProductID != 0 && Username == "" && ProductName != "" && StoreName != "")
            {
                return new StatusResponseGenerator("Please enter your username", 401, DateTime.Now);
            }
            else if (QueueID != 0 && StoreID != 0 && ProductID != 0 && Username != "" && ProductName == "" && StoreName != "")
            {
                return new StatusResponseGenerator("Please enter your product name", 401, DateTime.Now);
            }
            else if (QueueID != 0 && StoreID != 0 && ProductID != 0 && Username != "" && ProductName != "" && StoreName == "")
            {
                return new StatusResponseGenerator("Please enter your store name", 401, DateTime.Now);
            }
            else
            {
                return new StatusResponseGenerator("OK", 200, DateTime.Now);
            }
        }

        public StatusResponseGenerator validateDeletion(int QueueID, string Username)
        {
            if (QueueID == 0 && Username == "")
            {
                return new StatusResponseGenerator("Please complete the form", 401, DateTime.Now);
            }
            else if (QueueID == 0 && Username != "")
            {
                return new StatusResponseGenerator("Please enter the queue id", 401, DateTime.Now);
            }
            else if (QueueID != 0 && Username == "")
            {
                return new StatusResponseGenerator("Please enter your username", 401, DateTime.Now);
            }
            else
            {
                return new StatusResponseGenerator("OK", 200, DateTime.Now);
            }
        }
    }
}
