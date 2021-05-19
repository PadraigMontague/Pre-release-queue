namespace Pre_Release_Que

{
    public class QueuedProducts
    {

        public int QueueID { get; set; }
        public string StoreName { get; set; }
        public int StoreID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string DateCreated { get; set; }

        public QueuedProducts(int queueID, string storeName, int storeID, int productID, string productName, string dateCreated)
        {
            QueueID = queueID;
            StoreName = storeName;
            StoreID = storeID;
            ProductID = productID;
            ProductName = productName;
            DateCreated = dateCreated;
        }
    }
}