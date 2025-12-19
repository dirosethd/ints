using ints.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace ints.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthApiService _authApi;

        public LoginViewModel(AuthApiService authApi)
        {
            _authApi = authApi;

            LoginCommand = new RelayCommand(async () => await LoginAsync(), CanLogin);
            GoRegisterCommand = new RelayCommand(OpenRegister);
        }

        private string _username = "";
        public string Username
        {
            get => _username;
            set { if (Set(ref _username, value)) LoginCommand.RaiseCanExecuteChanged(); }
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set { if (Set(ref _password, value)) LoginCommand.RaiseCanExecuteChanged(); }
        }

        private string _error = "";
        public string Error
        {
            get => _error;
            set => Set(ref _error, value);
        }

        public RelayCommand LoginCommand { get; }
        public RelayCommand GoRegisterCommand { get; }

        private bool CanLogin()
            => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);

        private void OpenRegister()
        {
            var reg = App.Services.GetRequiredService<ints.Views.RegisterWindow>();
            reg.Owner = Application.Current.MainWindow; // можно убрать
            reg.ShowDialog();
        }

        private async Task LoginAsync()
        {
            Error = "";

            // ✅ Вход через API: получаем JWT токен
            var token = await _authApi.LoginAsync(Username, Password);

            if (string.IsNullOrWhiteSpace(token))
            {
                Error = "Неверный логин или пароль.";
                return;
            }

            // ✅ Закрыть LoginWindow и разрешить App.xaml.cs открыть MainWindow
            foreach (Window w in Application.Current.Windows)
            {
                if (w is ints.Views.LoginWindow)
                {
                    w.DialogResult = true;
                    w.Close();
                    break;
                }
            }
        }
    }
}
