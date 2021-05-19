namespace Pre_Release_Que
{
    public class StoreUser
    {
        public int Id { get; set; }
        public string StoreName { get; set; }
        public string Address { get; set; }

        public StoreUser(int id, string storeName, string address)
        {
            Id = id;
            StoreName = storeName;
            Address = address;
        }
    }
}