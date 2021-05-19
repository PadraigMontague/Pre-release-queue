namespace Pre_Release_Que
{
    public class QueueGenerator
    {
        public int QueueID { get; set; }
        public string StoreName { get; set; }
        public int StoreID { get; set; }
        public string Address { get; set; }
        public string Product { get; set; }
        public string Date { get; set; }

        public QueueGenerator(int queueID, string storeName, int storeID, string address, string product, string date)
        {
            QueueID = queueID;
            StoreName = storeName;
            StoreID = storeID;
            Address = address;
            Product = product;
            Date = date;
        }
    }
}