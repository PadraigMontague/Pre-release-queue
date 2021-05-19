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
    public class QueueGeneratorController : ControllerBase
    {

        private DatabaseConnection DatabaseConnection { get; set; }
        private AuthenticationController AuthenticationController { get; set; }
        private DatabaseAuth DatabaseAuth { get; set; }

        public QueueGeneratorController(DatabaseConnection databaseConnection, DatabaseAuth databaseAuth)
        {
            this.DatabaseConnection = databaseConnection;
            this.DatabaseAuth = databaseAuth;
            this.AuthenticationController = new AuthenticationController(databaseConnection, databaseAuth);
        }


        public List<QueueGenerator> queues = new List<QueueGenerator>() { };
        public List<string> messages = new List<string>() { };

        [HttpGet("GetAllQueues")]
        public List<QueueGenerator> GetAllQueues()
        {
            string token = Request.Form["token"];
            bool isValid = this.AuthenticationController.CheckTokenStatus(token);
            if (isValid)
            {
                var cmd = this.DatabaseConnection.Connection.CreateCommand() as MySqlCommand;
                cmd.CommandText = @"SELECT * FROM queues";
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows == false)
                    {
                        return queues;
                    }
                    while (reader.Read())
                    {
                        int QueueID = reader.GetInt32("queueID");
                        string Storename = reader.GetString("storename");
                        int StoreID = reader.GetInt32("storeID");
                        string Address = reader.GetString("address");
                        string Productname = reader.GetString("product");
                        string DateCreated = reader.GetString("date");
                        queues.Add(new QueueGenerator(QueueID, Storename, StoreID, Address, Productname, DateCreated));
                    }
                }
            }
            else
            {
                return queues;
            }
            return queues;
        }

        [HttpPost("CreateQueue")]
        public string CreateQueue()
        {
            string response = null;
            string token = Request.Form["token"];
            bool isValid = this.AuthenticationController.CheckTokenStatus(token);
            if (isValid)
            {
                string storename = this.DatabaseAuth.preparedStatement(Request.Form["storeName"]);
                string storeID = this.DatabaseAuth.preparedStatement(Request.Form["storeID"]);
                string address = this.DatabaseAuth.preparedStatement(Request.Form["address"]);
                string product = this.DatabaseAuth.preparedStatement(Request.Form["product"]);

                if (validateForm(storename, address, product, storeID).Code == 200)
                {
                    var cmd = this.DatabaseConnection.Connection.CreateCommand() as MySqlCommand;
                    cmd.CommandText = @"INSERT INTO queues(storeName, storeID, address, product, date) VALUES (@StoreName, @StoreID, @Address,  @Product, @Date)";
                    cmd.Parameters.AddWithValue("@StoreName", storename);
                    cmd.Parameters.AddWithValue("@StoreID", storeID);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.Parameters.AddWithValue("@Product", product);
                    cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                    var recs = cmd.ExecuteNonQuery();
                    var check = recs == 1 ? response = "OK" : response = "Error";
                }
                else
                {
                    response = validateForm(storename, address, product, storeID).Status;
                    return response;
                }
            }
            else
            {
                return response;
            }
            return response;
        }

        [HttpPost("deleteQueue")]
        public string deleteQueue()
        {
            string response = null;
            string token = Request.Form["token"];
            bool isValid = this.AuthenticationController.CheckTokenStatus(token);
            if (isValid)
            {
                string queueID = this.DatabaseAuth.preparedStatement(Request.Form["queueID"]);
                string storeName = this.DatabaseAuth.preparedStatement(Request.Form["StoreName"]);

                if (queueID != "" && storeName != "")
                {
                    var cmd = this.DatabaseConnection.Connection.CreateCommand() as MySqlCommand;
                    cmd.CommandText = @"DELETE FROM queues WHERE queueID = @QueueID AND storeName = @StoreName";
                    cmd.Parameters.AddWithValue("@QueueID", queueID);
                    cmd.Parameters.AddWithValue("@StoreName", storeName);
                    var recs = cmd.ExecuteNonQuery();
                    var check = recs == 1 ? response = "OK" : response = "Error";
                }
                else
                {
                    response = "Please enter all requested data";
                    return response;
                }
            }
            else
            {
                return response;
            }
            return response;
        }


        public StatusResponseGenerator validateForm(string storename, string address, string product, string storeID)
        {

            if (storename == "" && address == "" && product == "" && storeID == "")
            {
                return new StatusResponseGenerator("Please complete the form", 401, DateTime.Now);
            }
            else if (storename == "" && address != "" && product != "" && storeID != "")
            {
                return new StatusResponseGenerator("Please enter your storename", 401, DateTime.Now);
            }
            else if (storename != "" && address == "" && product != "" && storeID != "")
            {
                return new StatusResponseGenerator("Please enter your stores address", 401, DateTime.Now);
            }
            else if (storename != "" && address != "" && product == "" && storeID != "")
            {
                return new StatusResponseGenerator("Please enter the name of your product", 401, DateTime.Now);
            }
            else if (storename != "" && address != "" && product != "" && storeID == "")
            {
                return new StatusResponseGenerator("Please enter the store id", 401, DateTime.Now);
            }
            else
            {
                return new StatusResponseGenerator("OK", 200, DateTime.Now);
            }
        }
    }
}
