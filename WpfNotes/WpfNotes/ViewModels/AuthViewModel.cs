using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfNotes.Models;

namespace WpfNotes.ViewModels
{
    public class AuthViewModel : ViewModelBase, IChangeWindows
    {
        private readonly AuthModel _authModel;
        private string _email = "";
        private string _password = "";
        private string _confirmPassword = "";
        private bool _isRememberMe = false;
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
        public bool IsRememberMe
        {
            get => _isRememberMe;
            set
            {
                _isRememberMe = value;
                OnPropertyChanged(nameof(IsRememberMe));
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
            _authModel = new AuthModel();
            LoginCommand = new ViewModelCommand(ExecuteLoginCommand);
            RegisterCommand = new ViewModelCommand(ExecuteRegisterCommand);
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
            ErrorMessage = "Logging in...";
            var result = await _authModel.LoginAsync(Email, Password, IsRememberMe);
            if (result)
            {
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
            ErrorMessage = "Registering...";
            var result = await _authModel.RegisterAsync(Email, Password, ConfirmPassword, IsRememberMe);
            if (result)
            {
                ChangeWindow();
            }
            else
            {
                ErrorMessage = "Invalid email or password";
            }
        }
    }
}
