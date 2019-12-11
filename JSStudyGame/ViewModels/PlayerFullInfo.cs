using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSStudyGame.ViewModels
{
    public class PlayerFullInfo : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string _email;
        public string Email
        {
            get { return this._email; }
            set
            {
                if (this._email != value)
                {
                    this._email = value;
                    this.NotifyPropertyChanged("Email");
                }
            }
        }

        private string _login;
        public string Login
        {
            get { return this._login; }
            set
            {
                if (this._login != value)
                {
                    this._login = value;
                    this.NotifyPropertyChanged("Login");
                }
            }
        }

        private string _password;
        public string Password
        {
            get { return this._password; }
            set
            {
                if (this._password != value)
                {
                    this._password = value;
                    this.NotifyPropertyChanged("Password");
                }
            }
        }

        private bool _isAdmin;
        public bool IsAdmin
        {
            get { return this._isAdmin; }
            set
            {
                if (this._isAdmin != value)
                {
                    this._isAdmin = value;
                    this.NotifyPropertyChanged("IsAdmin");
                }
            }
        }
        private string _name;
        public string Name
        {
            get { return this._name; }
            set
            {
                if (this._name != value)
                {
                    this._name = value;
                    this.NotifyPropertyChanged("Name");
                }
            }
        }

        private string _surname;
        public string Surname
        {
            get { return this._surname; }
            set
            {
                if (this._surname != value)
                {
                    this._surname = value;
                    this.NotifyPropertyChanged("Surname");
                }
            }
        }
        private string _photo;
        public string Photo
    {
            get { return this._photo; }
            set
            {
                if (this._photo != value)
                {
                    this._photo = value;
                    this.NotifyPropertyChanged("Photo");
                }
            }
        }

        private DateTime _birthDate;
        public DateTime BirthDate
        {
            get { return this._birthDate; }
            set
            {
                if (this._birthDate != value)
                {
                    this._birthDate = value;
                    this.NotifyPropertyChanged("BirthDate");
                }
            }
        }

        private string _gender;
        public string Gender
        {
            get { return this._gender; }
            set
            {
                if (this._gender != value)
                {
                    this._gender = value;
                    this.NotifyPropertyChanged("Gender");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
