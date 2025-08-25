using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfNotes.Models;
using WpfNotes.Services;

namespace WpfNotes.ViewModels
{
    public class AuthViewModel : ViewModelBase, IChangeWindows
    {
        private readonly AuthService _apiService;
        private string _email = "";
        private string _password = "";
        private string _confirmPassword = "";
        private string _errorMessage = "";

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
            }
        }
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        public Action Change { get; set; }

        void ChangeWindow()
        {
            Change?.Invoke();
        }


        public AuthViewModel()
        {
            _apiService = new AuthService();
            LoginCommand = new ViewModelCommand(ExecuteLoginCommand);
            RegisterCommand = new ViewModelCommand(ExecuteRegisterCommand);
            ErrorMessage = "";
        }


        private bool CanExecuteLoginCommand(object obj)
        {
            if (string.IsNullOrWhiteSpace(Email) || Email.Length < 3) 
            {
                ErrorMessage = "Email is required";
                return false;
            }
            if (Password == null || Password.Length < 3)
            {
                ErrorMessage = "Password is required";
                return false;
            }
            return true;
        }

        private async void ExecuteLoginCommand(object obj)
        {
            if (!CanExecuteLoginCommand(obj))
            {
                return;
            }
            var token = await _apiService.LoginAsync(new LoginModel { Email = Email, Password = Password });
            if (token != null)
            {
                Settings.Default.JwtToken = token;
                Settings.Default.Save();
                ChangeWindow();
            }
            else
            {
                ErrorMessage = "Invalid email or password";
            }
        }

        private bool CanExecuteRegisterCommand(object obj)
        {
            if (!string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Email is required";
                return false;
            }
            if (Password == null || Password.Length < 3)
            {
                ErrorMessage = "Password is required";
                return false;
            }
            if (ConfirmPassword == null || ConfirmPassword.Length < 3)
            {
                ErrorMessage = "Confirm Password is required";
                return false;
            }
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords must be same";
                return false;
            }
            return true;
        }

        private async void ExecuteRegisterCommand(object obj)
        {
            if (!CanExecuteRegisterCommand(obj))
            {
                return;
            }
            var token = await _apiService.RegisterAsync(
                new RegisterModel { 
                    Email = Email, 
                    Password = Password, 
                    ConfirmPassword = ConfirmPassword 
                });
            if (token != null)
            {
                Settings.Default.JwtToken = token;
                Settings.Default.Save();
                ChangeWindow();
            }
            else
            {
                ErrorMessage = "Invalid email or password";
            }
        }
    }

    interface IChangeWindows
    {
        Action Change { get; set; }
    }
}
