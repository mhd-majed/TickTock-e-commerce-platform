namespace e_commerce_platform.Models
{
    public class UserViewModel
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public IList<string> Roles { get; set; }
    }

}
