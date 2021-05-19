using System;

namespace Pre_Release_Que
{
    public class Item
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Product { get; set; }
        public string StoreName { get; set; }
        public DateTime Date { get; set; }

        public Item(int id, string firstname, string lastname, string product, string storeName, DateTime date)
        {
            Id = id;
            Firstname = firstname;
            Lastname = lastname;
            Product = product;
            StoreName = storeName;
            Date = date;
        }
    }
}
