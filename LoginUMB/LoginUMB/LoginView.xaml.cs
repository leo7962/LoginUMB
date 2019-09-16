using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LoginUMB
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginView : ContentView
    {
        public event EventHandler<string> LoginChanged;

        public LoginView()
        {
            InitializeComponent();
        }

        private async void LoginClicked(object sender, EventArgs e)
        {
            var loginProvider = DependencyService.Get<ILoginProvider>();
            var idToken = await loginProvider.LoginAsync();

            string userName = null;
            if (idToken != null)
            {
                var jwHandler = new JwtSecurityTokenHandler();
                var token = jwHandler.ReadJwtToken(idToken);
                userName = token.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
            }

            if (LoginChanged != null) LoginChanged(this, userName);

            if (userName == null)
            {
                ErrorLabel.Text = "Inicio de sesión fallido.";
                return;
            }

            LoginPanel.IsVisible = false;
            LoginPanel.IsVisible = true;
            ErrorLabel.Text = "";
            LoggedInLabel.Text = "Estas logueado como" + userName;
        }

        private void LogoutClicked(object sender, EventArgs e)
        {
            LoginPanel.IsVisible = true;
            LogoutPanel.IsVisible = false;
            if (LoginChanged != null) LoginChanged(this, null);
        }


    }
}