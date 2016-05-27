namespace IdentityServer.Models.AccountViewModels
{
    public class LoginViewModel : LoginInputModel
    {
        public LoginViewModel()
        {
        }

        public LoginViewModel(LoginInputModel other)
        {
            Email = other.Email;
            Password = other.Password;
            RememberMe = other.RememberMe;
            SignInId = other.SignInId;
        }

        public string ErrorMessage { get; set; }
    }
}
