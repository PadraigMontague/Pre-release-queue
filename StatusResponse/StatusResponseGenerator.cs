using System;

namespace Pre_Release_Que.StatusResponse
{

 public class StatusResponseGenerator
 {
        public string Status { get; set; }
        public int Code { get; set; }
        public DateTime Date { get; set; }

        public StatusResponseGenerator(string status, int code, DateTime date) {
            Status = status;
            Code = code;
            Date = date;
        }

 }

}