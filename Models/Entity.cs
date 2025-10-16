﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Models
{
    public abstract class Entity : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
