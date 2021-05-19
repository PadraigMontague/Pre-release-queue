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
    public class ProductController : ControllerBase
    {

        private DatabaseConnection DatabaseConnection { get; set; }
        private DatabaseAuth DatabaseAuth { get; set; }
        private AuthenticationController AuthenticationController { get; set; }

        public ProductController(DatabaseConnection databaseConnection, DatabaseAuth databaseAuth)
        {
            this.DatabaseConnection = databaseConnection;
            this.DatabaseAuth = databaseAuth;
            this.AuthenticationController = new AuthenticationController(databaseConnection, databaseAuth);
        }
        public List<Product> products = new List<Product>() { };

        [HttpGet("GetAllProducts")]
        public List<Product> GetAllProducts()
        {
            try
            {
                string token = Request.Form["token"];
                bool isValid = this.AuthenticationController.CheckTokenStatus(token);
                if (isValid)
                {
                    var cmd = this.DatabaseConnection.Connection.CreateCommand() as MySqlCommand;
                    cmd.CommandText = @"SELECT * FROM products";
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows == false)
                        {
                            return products;
                        }
                        while (reader.Read())
                        {
                            var ProductName = reader.GetString("ProductName");
                            var ProductDescription = reader.GetString("ProductDescription");
                            var DateCreated = reader.GetString("DateCreated");
                            products.Add(new Product(ProductName, ProductDescription, DateCreated));
                        }
                    }
                    return products;
                }
                else
                {
                    return products;
                }
            }
            catch
            {
                products.Add(new Product("WARNING", "Please check that all HTTP Body data is provided", "Unauthorised"));
                return products;
            }
        }

        [HttpPost("CreateProduct")]
        public string CreateProduct()
        {
            string response = null;
            try
            {
                string productName = this.DatabaseAuth.preparedStatement(Request.Form["product"]);
                string productDescription = this.DatabaseAuth.preparedStatement(Request.Form["description"]);
                string storeName = this.DatabaseAuth.preparedStatement(Request.Form["storename"]);
                DateTime dateCreated = DateTime.Now;
                string token = Request.Form["token"];
                bool isValid = this.AuthenticationController.CheckTokenStatus(token);
                if (isValid)
                {
                    if (validateForm(productName, productDescription).Code == 200)
                    {
                        var cmd = this.DatabaseConnection.Connection.CreateCommand() as MySqlCommand;
                        cmd.CommandText = @"INSERT INTO products(productName, productDescription, storeName, dateCreated) VALUES (@ProductName, @ProductDescription, @Storename, @DateCreated)";
                        cmd.Parameters.AddWithValue("@ProductName", productName);
                        cmd.Parameters.AddWithValue("@ProductDescription", productDescription);
                        cmd.Parameters.AddWithValue("@Storename", storeName);
                        cmd.Parameters.AddWithValue("@DateCreated", dateCreated);
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
            }
            catch
            {
                response = "Please check that all HTTP Body data is provided";
            }
            return response;
        }

        public StatusResponseGenerator validateForm(string productName, string productDescription)
        {

            if (productName == "" && productDescription == "")
            {
                return new StatusResponseGenerator("Please complete the form", 401, DateTime.Now);
            }
            else if (productName == "" && productDescription != "")
            {
                return new StatusResponseGenerator("Please enter the product name", 401, DateTime.Now);
            }
            else if (productName != "" && productDescription == "")
            {
                return new StatusResponseGenerator("Please enter the product description", 401, DateTime.Now);
            }
            else
            {
                return new StatusResponseGenerator("OK", 200, DateTime.Now);
            }
        }
        [HttpPost("DeleteProduct")]
        public string deleteProduct()
        {
            string response = null;
            try
            {
                string productID = this.DatabaseAuth.preparedStatement(Request.Form["productID"]);
                string token = Request.Form["token"];
                bool isValid = this.AuthenticationController.CheckTokenStatus(token);
                if (isValid)
                {
                    if (productID != "")
                    {
                        var cmd = this.DatabaseConnection.Connection.CreateCommand() as MySqlCommand;
                        cmd.CommandText = @"DELETE FROM products WHERE productID = @ProductID";
                        cmd.Parameters.AddWithValue("@ProductID", productID);
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
            }
            catch
            {
                response = "Please check that all HTTP Body data is provided";
            }
            return response;
        }

        [HttpPost("UpdateProduct")]
        public string UpdateProduct()
        {
            string response = null;
            try
            {
                string productID = this.DatabaseAuth.preparedStatement(Request.Form["productID"]);
                string productName = this.DatabaseAuth.preparedStatement(Request.Form["product"]);
                string productdescription = this.DatabaseAuth.preparedStatement(Request.Form["productDescription"]);
                string storeName = this.DatabaseAuth.preparedStatement(Request.Form["storename"]);
                StatusResponseGenerator result = validateUpdate(productID, productName, productdescription, storeName);
                string token = Request.Form["token"];
                bool isValid = this.AuthenticationController.CheckTokenStatus(token);
                if (isValid)
                {
                    if (result.Status == "OK")
                    {
                        var cmd = this.DatabaseConnection.Connection.CreateCommand() as MySqlCommand;
                        cmd.CommandText = @"UPDATE products SET productName = @ProductName, productDescription = @ProductDescription, storeName = @StoreName WHERE productID = @ProductID";
                        cmd.Parameters.AddWithValue("@ProductID", productID);
                        cmd.Parameters.AddWithValue("@ProductName", productName);
                        cmd.Parameters.AddWithValue("@ProductDescription", productdescription);
                        cmd.Parameters.AddWithValue("@StoreName", storeName);
                        var recs = cmd.ExecuteNonQuery();
                        var check = recs == 1 ? response = "OK" : response = "Error";
                    }
                    else
                    {
                        return result.Status;
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

        public StatusResponseGenerator validateUpdate(string productID, string productName, string productdescription, string storeName)
        {

            if (productID == "" && productName == "" && productdescription == "" && storeName == "")
            {
                return new StatusResponseGenerator("Please complete the form", 401, DateTime.Now);
            }
            else if (productID == "" && productName != "" && productdescription != "" && storeName != "")
            {
                return new StatusResponseGenerator("Please enter the product id", 401, DateTime.Now);
            }
            else if (productID != "" && productName == "" && productdescription != "" && storeName != "")
            {
                return new StatusResponseGenerator("Please enter the product name", 401, DateTime.Now);
            }
            else if (productID != "" && productName != "" && productdescription == "" && storeName != "")
            {
                return new StatusResponseGenerator("Please enter the product description", 401, DateTime.Now);
            }
            else if (productID != "" && productName != "" && productdescription != "" && storeName == "")
            {
                return new StatusResponseGenerator("Please enter the store name", 401, DateTime.Now);
            }
            else
            {
                return new StatusResponseGenerator("OK", 200, DateTime.Now);
            }
        }
    }
}
