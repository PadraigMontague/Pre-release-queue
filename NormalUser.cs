namespace Pre_Release_Que
{
    public class NormalUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public NormalUser(int id, string username, string firstname, string lastname)
        {
            Id = id;
            Username = username;
            Firstname = firstname;
            Lastname = lastname;
        }
    }
}