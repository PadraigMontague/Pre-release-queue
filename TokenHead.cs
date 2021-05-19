using System;

namespace Pre_Release_Que
{
    public class TokenHead
    {
        public string Issued_by { get; set; }
        public string Secret { get; set; }
        public DateTime Date { get; set; }

        public TokenHead(string issued_by, string secret, DateTime date)
        {
            Issued_by = issued_by;
            Secret = secret;
            Date = date;
        }
    }
}