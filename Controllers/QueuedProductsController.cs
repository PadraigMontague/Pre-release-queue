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
    public class QueuedProductsController : ControllerBase
    {

        private DatabaseConnection DatabaseConnection { get; set; }
        private DatabaseAuth DatabaseAuth { get; set; }
        private AuthenticationController AuthenticationController { get; set; }
        public List<QueuedProducts> queuedProducts = new List<QueuedProducts>() { };

        public QueuedProductsController(DatabaseConnection databaseConnection, DatabaseAuth databaseAuth)
        {
            this.DatabaseConnection = databaseConnection;
            this.DatabaseAuth = databaseAuth;
            this.AuthenticationController = new AuthenticationController(databaseConnection, databaseAuth);
        }

        [HttpGet("GetAllQueuedProducts")]
        public List<QueuedProducts> GetAllQueuedProducts()
        {
            try
            {
                string token = Request.Form["token"];
                bool isValid = this.AuthenticationController.CheckTokenStatus(token);
                if (isValid)
                {
                    var cmd = this.DatabaseConnection.Connection.CreateCommand() as MySqlCommand;
                    cmd.CommandText = @"SELECT * FROM queuedProducts";
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows == false)
                        {
                            return queuedProducts;
                        }
                        while (reader.Read())
                        {
                            int QueueID = reader.GetInt32("queueID");
                            int StoreID = reader.GetInt32("storeID");
                            int ProductID = reader.GetInt32("productID");
                            string ProductName = reader.GetString("productName");
                            string StoreName = reader.GetString("storeName");
                            string DateCreated = reader.GetString("dateCreated");
                            queuedProducts.Add(new QueuedProducts(QueueID, StoreName, StoreID, ProductID, ProductName, DateCreated));
                        }

                        return queuedProducts;

                    }
                }
                else
                {
                    return queuedProducts;
                }
            }
            catch
            {
                queuedProducts.Add(new QueuedProducts(0, "", 0, 0, "Warning", "Please check that all HTTP Body data is provided"));
                return queuedProducts;
            }
        }

        [HttpPost("AddProductToQueue")]
        public string AddProductToQueue()
        {
            string response = null;
            string token = Request.Form["token"];
            bool isValid = this.AuthenticationController.CheckTokenStatus(token);
            if (isValid)
            {
                string productName = this.DatabaseAuth.preparedStatement(Request.Form["product"]);
                string queueID = this.DatabaseAuth.preparedStatement(Request.Form["queueID"]);
                string storeID = this.DatabaseAuth.preparedStatement(Request.Form["storeID"]);
                string productID = this.DatabaseAuth.preparedStatement(Request.Form["productID"]);
                string storeName = this.DatabaseAuth.preparedStatement(Request.Form["storeName"]);
                DateTime dateCreated = DateTime.Now;

                if (validateForm(productName, queueID, storeID, storeName).Code == 200)
                {
                    var cmd = this.DatabaseConnection.Connection.CreateCommand() as MySqlCommand;
                    cmd.CommandText = @"INSERT INTO queuedproducts(queueID, storeID, productID, productName, storeName, dateCreated) VALUES (@QueueID, @StoreID, @ProductID, @ProductName, @StoreName, @DateCreated)";
                    cmd.Parameters.AddWithValue("@ProductName", productName);
                    cmd.Parameters.AddWithValue("@QueueID", queueID);
                    cmd.Parameters.AddWithValue("@StoreID", storeID);
                    cmd.Parameters.AddWithValue("@ProductID", productID);
                    cmd.Parameters.AddWithValue("@StoreName", storeName);
                    cmd.Parameters.AddWithValue("@DateCreated", dateCreated);
                    var recs = cmd.ExecuteNonQuery();
                    var check = recs == 1 ? response = "OK" : response = "Error";
                }
                else
                {
                    response = validateForm(productName, queueID, storeID, storeName).Status;
                    return response;
                }
            }
            else
            {
                return response;
            }
            return response;
        }

        public StatusResponseGenerator validateForm(string productName, string queueID, string storeID, string storeName)
        {

            if (productName == "" && storeName == "" && queueID == "" && storeID == "")
            {
                return new StatusResponseGenerator("Please complete the form", 401, DateTime.Now);
            }
            else if (productName == "" && storeName != "" && queueID != "" && storeID != "")
            {
                return new StatusResponseGenerator("Please enter the product name", 401, DateTime.Now);
            }
            else if (productName != "" && storeName == "" && queueID != "" && storeID != "")
            {
                return new StatusResponseGenerator("Please enter your store name", 401, DateTime.Now);
            }
            else if (productName != "" && storeName != "" && queueID == "" && storeID != "")
            {
                return new StatusResponseGenerator("Please enter your queue id", 401, DateTime.Now);
            }
            else if (productName != "" && storeName != "" && queueID != "" && storeID == "")
            {
                return new StatusResponseGenerator("Please enter your store id", 401, DateTime.Now);
            }
            else
            {
                return new StatusResponseGenerator("OK", 200, DateTime.Now);
            }
        }

        [HttpPost("deleteQueuedProduct")]

        public string deleteQueuedProduct()
        {
            string response = null;
            string token = Request.Form["token"];
            bool isValid = this.AuthenticationController.CheckTokenStatus(token);
            if (isValid)
            {
                string queuedProductsID = this.DatabaseAuth.preparedStatement(Request.Form["queuedProductsID"]);

                if (queuedProductsID != "")
                {
                    var cmd = this.DatabaseConnection.Connection.CreateCommand() as MySqlCommand;
                    cmd.CommandText = @"DELETE FROM queuedproducts WHERE queuedProductsID = @QueuedProductsID";
                    cmd.Parameters.AddWithValue("@QueuedProductsID", queuedProductsID);
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
    }
}
