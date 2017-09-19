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
using System.Globalization;

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

        int idSelected; //need for editing. This helps me send selected id accross functions.

        public AppointmentsPage()
        {
            this.InitializeComponent();
            savebtn.Visibility = Visibility.Collapsed; //Make Sure the Save Button is not visible.
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
                else
                {
                    string CDay = eventDate.Date.Date.Day.ToString();
                    string CMonth = eventDate.Date.Date.Month.ToString();
                    string CYear = eventDate.Date.Date.Year.ToString();
                    string EventDateString = "" + CMonth + "/" + CDay + "/" + CYear;

                    string hour = startTime.Time.Hours.ToString(); //Convert All Time and date into string for database.
                    string minutes = startTime.Time.Minutes.ToString();
                    string StartTimeString = "" + hour + ":" + minutes;

                    string hour2 = endTime.Time.Hours.ToString();
                    string minutes2 = endTime.Time.Minutes.ToString();
                    string EndTimeString = "" + hour2 + ":" + minutes2;
                    // inserts the data
                    conn.Insert(new Appointments()
                    {
                        EventName = eventName.Text,
                        EventDate = EventDateString,
                        StartTime = StartTimeString,
                        EndTime = EndTimeString
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
            MessageDialog ShowConf = new MessageDialog("Are you sure?", "Important");
            ShowConf.Commands.Add(new UICommand("Yes, Delete")
            {
                Id = 0
            });
            ShowConf.Commands.Add(new UICommand("Cancel")
            {
                Id = 1
            });
            ShowConf.DefaultCommandIndex = 0;
            ShowConf.CancelCommandIndex = 1;

            var result = await ShowConf.ShowAsync();
            if ((int)result.Id == 0)
            {
                // checks if data is null else inserts
                try
                {
                    int appointmentD = ((Appointments)AppointmentList.SelectedItem).AppointmentID;
                    var querydel = conn.Query<Appointments>("DELETE FROM Appointments WHERE AppointmentID='" + appointmentD + "'");
                    Results();
                }
                catch (NullReferenceException)
                {
                    MessageDialog ClearDialog = new MessageDialog("Please select the item to Delete", "Oops..!");
                    await ClearDialog.ShowAsync();
                }
            }
            else
            {
                //
            }
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
                if (AppointmentList.SelectedIndex == -1)
                {
                    MessageDialog dialog = new MessageDialog("No selected event", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    idSelected = ((Appointments)AppointmentList.SelectedItem).AppointmentID; //Store ID to be able to update data.
                    eventName.Text = ((Appointments)AppointmentList.SelectedItem).EventName;

                    DateTime eventDateConvert = DateTime.Parse(((Appointments)AppointmentList.SelectedItem).EventDate);
                    this.eventDate.Date = eventDateConvert;
                                  
                    TimeSpan startTimeConvert = TimeSpan.Parse(((Appointments)AppointmentList.SelectedItem).StartTime);
                    this.startTime.Time = startTimeConvert;

                    TimeSpan endTimeConvert = TimeSpan.Parse(((Appointments)AppointmentList.SelectedItem).EndTime);
                    this.endTime.Time = endTimeConvert;

                    addbtn.Visibility = Visibility.Collapsed;
                    editbtn.Visibility = Visibility.Collapsed;
                    deletebtn.Visibility = Visibility.Collapsed;
                    savebtn.Visibility = Visibility.Visible;

            }
          }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string CDay = eventDate.Date.Date.Day.ToString();
            string CMonth = eventDate.Date.Date.Month.ToString();
            string CYear = eventDate.Date.Date.Year.ToString();
            string EventDateString = "" + CMonth + "/" + CDay + "/" + CYear;

            string hour = startTime.Time.Hours.ToString(); //Convert All Time and date into string for database.
            string minutes = startTime.Time.Minutes.ToString();
            string StartTimeString = "" + hour + ":" + minutes;

            string hour2 = endTime.Time.Hours.ToString();
            string minutes2 = endTime.Time.Minutes.ToString();
            string EndTimeString = "" + hour2 + ":" + minutes2;
            // inserts the data

            conn.Query<Appointments>("UPDATE Appointments SET EventName = '" + eventName.Text + "', EventDate = '" + EventDateString + "', StartTime = '" + StartTimeString + "', EndTime = '" + EndTimeString + "' WHERE AppointmentID ='" + idSelected + "'");

            addbtn.Visibility = Visibility.Visible;
            editbtn.Visibility = Visibility.Visible;
            deletebtn.Visibility = Visibility.Visible;
            savebtn.Visibility = Visibility.Collapsed;

            Results();
        }
        }
    }
