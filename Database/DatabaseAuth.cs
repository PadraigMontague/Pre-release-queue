using System.Text.RegularExpressions;

namespace Pre_Release_Que.Database
{

 public class DatabaseAuth
 {
     public string prepared = "";
     public string patternOne = @"<[^>]*>";
     public string replacement = "";

     public string preparedStatement(string data) {

        Regex expressionOne = new Regex(patternOne); 
        prepared = expressionOne.Replace(data, replacement);
        return prepared;

     }
 }

}