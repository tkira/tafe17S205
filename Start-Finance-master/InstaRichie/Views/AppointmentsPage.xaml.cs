using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using StartFinance.Models;
using SQLite.Net;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppointmentsPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public AppointmentsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

            // Creating table
            Results();
        }

        public void Results()
        {
            // Creating table
            conn.CreateTable<Appointments>();
            var query = conn.Table<Appointments>();
            AppointmentList.ItemsSource = query.ToList();
        }

        // Displays the data when navigation between pages
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }


        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (eventName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Event Name not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (startTime.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Start Time not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (endTime.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("End Time not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (endTime.Text.ToString() == startTime.Text.ToString())
                {
                    MessageDialog dialog = new MessageDialog("Time are same", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    string CDay = eventDate.Date.Value.Day.ToString();
                    string CMonth = eventDate.Date.Value.Month.ToString();
                    string CYear = eventDate.Date.Value.Year.ToString();
                    string EventDateString = "" + CMonth + "/" + CDay + "/" + CYear;
                    // inserts the data
                    conn.Insert(new Appointments()
                    {
                        EventName = eventName.Text,
                        EventDate = EventDateString,
                        StartTime = startTime.Text,
                        EndTime = endTime.Text
                    });
                    Results();
                }
            }
            catch (Exception ex)
            {   // Exception to display when amount is invalid or not numbers
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Event or entered an invalid data", "Oops..!");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Event name already exist, Try Different Name", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }
            }
        }


        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
        }


    }
}