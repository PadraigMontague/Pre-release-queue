using System;

namespace Pre_Release_Que
{
    public class Authentication
    {

        public string Issued_by { get; set; }
        public string Secret { get; set; }
        public DateTime Date { get; set; }
        public string id { get; set; }
        public string username { get; set; }
        public DateTime date_created { get; set; }
        public DateTime expiry_date { get; set; }
        public string revoked { get; set; }

        public Authentication(string issued_by, string secret, DateTime date, string Id, string Username, DateTime Date_created, DateTime Expiry_date, string Revoked)
        {
            Issued_by = issued_by;
            Secret = secret;
            Date = date;
            id = Id;
            username = Username;
            date_created = Date_created;
            expiry_date = Expiry_date;
            revoked = Revoked;
        }

    }
}