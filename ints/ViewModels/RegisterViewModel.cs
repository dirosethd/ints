using ints.Services;
using ints.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ints.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthService _auth;

        public RegisterViewModel(IAuthService auth)
        {
            _auth = auth;

            RegisterCommand = new RelayCommand(async () => await RegisterAsync(), CanRegister);
            BackToLoginCommand = new RelayCommand(CloseRegisterWindow);
        }

        // ================== PROPERTIES ==================

        private string _username = "";
        public string Username
        {
            get => _username;
            set { if (Set(ref _username, value)) RegisterCommand.RaiseCanExecuteChanged(); }
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set { if (Set(ref _password, value)) RegisterCommand.RaiseCanExecuteChanged(); }
        }

        private string _password2 = "";
        public string Password2
        {
            get => _password2;
            set { if (Set(ref _password2, value)) RegisterCommand.RaiseCanExecuteChanged(); }
        }

        private string _error = "";
        public string Error
        {
            get => _error;
            set => Set(ref _error, value);
        }

        // ================== COMMANDS ==================

        public RelayCommand RegisterCommand { get; }
        public RelayCommand BackToLoginCommand { get; }

        // ================== LOGIC ==================

        private bool CanRegister()
            => !string.IsNullOrWhiteSpace(Username)
               && Password.Length >= 6
               && Password == Password2;

        private async Task RegisterAsync()
        {
            try
            {
                Error = "";

                await _auth.RegisterAsync(Username, Password);

                // успешная регистрация → закрываем окно
                CloseRegisterWindow();
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }
        }

        private void CloseRegisterWindow()
        {
            foreach (Window w in Application.Current.Windows)
            {
                if (w is ints.Views.RegisterWindow)
                {
                    w.Close();
                    break;
                }
            }
        }
    }
}