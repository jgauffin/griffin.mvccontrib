namespace Griffin.MvcContrib.Areas.Griffin.Models.Account
{
    public class CreateModel
    {
        public string Password { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsApproved { get; set; }
        public string PasswordQuestion { get; set; }
        public string PasswordAnswer { get; set; }
    }
}