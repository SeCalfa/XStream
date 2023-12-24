public class LoginModel
{
    public string usernameOrEmail;
    public string password;

    public LoginModel(string usernameOrEmail, string password)
    {
        this.usernameOrEmail = usernameOrEmail;
        this.password = password;
    }
}
