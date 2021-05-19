using System;
using MySql.Data.MySqlClient;

namespace Pre_Release_Que.Database
{

 public class DatabaseConnection :IDisposable
 {
     public MySqlConnection Connection;

     public DatabaseConnection(string credentials) {
         try{
            Connection = new MySqlConnection(credentials);
            this.Connection.Open();
         } catch {
             Console.WriteLine("Unable to complete request");
         }
     }

     public void Dispose() {
         Connection.Close();
     }
 }

}