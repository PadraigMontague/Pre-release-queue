namespace Pre_Release_Que

{
    public class Product
    {

        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string DateCreated { get; set; }

        public Product(string productName, string productDescription, string dateCreated)
        {
            ProductName = productName;
            ProductDescription = productDescription;
            DateCreated = dateCreated;
        }

    }
}