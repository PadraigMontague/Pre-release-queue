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
    public class ItemController : ControllerBase
    {

        private DatabaseConnection DatabaseConnection { get; set; }
        private DatabaseAuth DatabaseAuth { get; set; }
        private AuthenticationController AuthenticationController { get; set; }

        public ItemController(DatabaseConnection databaseConnection, DatabaseAuth databaseAuth)
        {
            this.DatabaseConnection = databaseConnection;
            this.DatabaseAuth = databaseAuth;
            this.AuthenticationController = new AuthenticationController(databaseConnection, databaseAuth);
        }
        public List<Item> item = new List<Item>() { };

        [HttpGet]
        public List<Item> Get()
        {
            return item;
        }

        [HttpPost("CreateItem")]
        public string CreateItem()
        {
            string response = null;
            try
            {
                string token = Request.Form["token"];
                bool isValid = this.AuthenticationController.CheckTokenStatus(token);
                if (isValid)
                {
                    string Firstname = this.DatabaseAuth.preparedStatement(Request.Form["firstname"]);
                    string Lastname = this.DatabaseAuth.preparedStatement(Request.Form["lastname"]);
                    string userID = this.DatabaseAuth.preparedStatement(Request.Form["userID"]);
                    string Product = this.DatabaseAuth.preparedStatement(Request.Form["product"]);
                    string queueID = this.DatabaseAuth.preparedStatement(Request.Form["queueID"]);
                    string StoreName = this.DatabaseAuth.preparedStatement(Request.Form["storeName"]);
                    string storeID = this.DatabaseAuth.preparedStatement(Request.Form["storeID"]);
                    DateTime Date = DateTime.Now;

                    if (validateForm(Firstname, Lastname, Product, StoreName).Code == 200)
                    {
                        var cmd = this.DatabaseConnection.Connection.CreateCommand() as MySqlCommand;
                        cmd.CommandText = @"INSERT INTO items(firstname, lastname, userID, product, queueID, storename, storeID, date) VALUES (@Firstname, @Lastname, @UserID, @Product, @QueueID, @StoreName, @StoreID, @Date)";
                        cmd.Parameters.AddWithValue("@Firstname", Firstname);
                        cmd.Parameters.AddWithValue("@Lastname", Lastname);
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        cmd.Parameters.AddWithValue("@Product", Product);
                        cmd.Parameters.AddWithValue("@QueueID", queueID);
                        cmd.Parameters.AddWithValue("@StoreName", StoreName);
                        cmd.Parameters.AddWithValue("@StoreID", storeID);
                        cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                        var recs = cmd.ExecuteNonQuery();
                        var check = recs == 1 ? response = "OK" : response = "Error";
                    }
                    else
                    {
                        response = validateForm(Firstname, Lastname, Product, StoreName).Status;
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

        public StatusResponseGenerator validateForm(string Firstname, string Lastname, string Product, string StoreName)
        {

            if (Firstname == "" && Lastname == "" && StoreName == "")
            {
                return new StatusResponseGenerator("Please complete the form", 401, DateTime.Now);
            }
            else if (Firstname == "" && Lastname != "" && StoreName != "")
            {
                return new StatusResponseGenerator("Please enter your firstname", 401, DateTime.Now);
            }
            else if (Firstname != "" && Lastname == "" && StoreName != "")
            {
                return new StatusResponseGenerator("Please enter your lastname", 401, DateTime.Now);
            }
            else if (Firstname != "" && Lastname != "" && StoreName == "")
            {
                return new StatusResponseGenerator("Please enter your store name", 401, DateTime.Now);
            }
            else
            {
                return new StatusResponseGenerator("OK", 200, DateTime.Now);
            }
        }

    }
}
