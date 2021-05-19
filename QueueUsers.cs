namespace Pre_Release_Que

{
    public class QueueUsers
    {

        public int QueueID { get; set; }
        public int StoreID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Username { get; set; }
        public string StoreName { get; set; }
        public string DateCreated { get; set; }

        public QueueUsers(int queueID, int storeID, int productID, string productName, string username, string storeName, string dateCreated)
        {
            ProductName = productName;
            QueueID = queueID;
            StoreID = storeID;
            ProductID = productID;
            Username = username;
            StoreName = storeName;
            DateCreated = dateCreated;
        }

    }
}