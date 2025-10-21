using Biite.Models;
using Biite.Services;
using SQLite;

namespace Biite.ViewModels
{
    internal class SetUpPageViewModel : ObservableObject
{
    public static SetUpPageViewModel Current { get; set; }
    SQLiteConnection connection;

    private string name;
    private string email;
    private string phoneNumber;
    private string location;

    public SetUpPageViewModel()
    {
        Current = this;
        connection = DatabaseService.Connection;
    }

    public string Name
    {
        get => name;
        set
        {
            name = value;
            OnPropertyChanged();
        }
    }

    public string Email
    {
        get => email;
        set
        {
            email = value;
            OnPropertyChanged();
        }
    }

    public string PhoneNumber
    {
        get => phoneNumber;
        set
        {
            phoneNumber = value;
            OnPropertyChanged();
        }
    }

    public string Location
    {
        get => location;
        set
        {
            location = value;
            OnPropertyChanged();
        }
    }

    public void SaveUser(User user)
    {
        if (user.Id > 0)
        {
            connection.Update(user);
        }
        else
        {
            connection.Insert(user);
        }
    }

    public void SaveCurrentUser()
    {
        var user = new User
        {
            Name = Name,
            Email = Email,
            PhoneNumber = PhoneNumber,
            Location = Location
        };
        SaveUser(user);
    }
}
}