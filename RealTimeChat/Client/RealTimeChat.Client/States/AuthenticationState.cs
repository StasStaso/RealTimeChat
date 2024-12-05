﻿using RealTimeChat.Shared.DTOs;
using System.ComponentModel;

namespace RealTimeChat.Client.States
{
    public class AuthenticationState : INotifyPropertyChanged
    {
        public const string AuthStoreKey = "authkey";

        public event PropertyChangedEventHandler? PropertyChanged;
        public UserDto User { get; set; } = default;
        public string? Token { get; set; }

        private bool _isAuthenticated;
        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            set
            {
                if (_isAuthenticated != value)
                {
                    _isAuthenticated = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAuthenticated)));
                }
            }
        }

        public void LoadState(AuthResponseDto authResponseDto)
        {
            //Id = authResponseDto.User.Id;
            //Name = authResponseDto.User.Name;
            User= authResponseDto.User;
            Token = authResponseDto.Token;
            IsAuthenticated = true;
        }
        public void UnLoadState()
        {
            //Id = 0;
            //Name = null;
            User = default;
            Token = null;
            IsAuthenticated = false;
        }
    }
}
