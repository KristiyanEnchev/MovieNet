namespace Models.User
{
    public class UserRegisterRequestModel : UserRequestModel
    {
        public UserRegisterRequestModel(string firstName, string lastName, string email, string password)
            : base(email, password)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}