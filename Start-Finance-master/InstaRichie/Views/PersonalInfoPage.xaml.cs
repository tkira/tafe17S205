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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PersonalInfoPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");
        string sex = "";

        public PersonalInfoPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

            // Creating table
            Results();
        }

        public string FinalDate()
        {
            string CDay = dob.Date.Value.Day.ToString();
            string CMonth = dob.Date.Value.Month.ToString();
            string CYear = dob.Date.Value.Year.ToString();
            string EndDate = "" + CDay + "/" + CMonth + "/" + CYear;

            return EndDate;
        }

        public void ClearAll()
        {
            fName.Text = "";
            lName.Text = "";
            email.Text = "";
            phone.Text = "";
            RadBtnM.IsChecked = false;
            RadBtnF.IsChecked = false;
            dob.Date = null;

        }

        public void Results()
        {
            // Creating table
            conn.CreateTable<PersonalInfo>();
            var query = conn.Table<PersonalInfo>();
            InfoList.ItemsSource = query.ToList();
        }

        private string DetSex()
        {
            if (RadBtnM.IsChecked == true)
            {
                sex = "Male";
            }
            else if (RadBtnF.IsChecked == true)
            {
                sex = "Female";
            }
            return sex;
        }

        private async void AddInfo_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                // checks if anything is null
                if (fName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("First Name not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (lName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Last Name not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (email.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Email Address not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (phone.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Phone Number not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (DetSex() == "")
                {
                    MessageDialog dialog = new MessageDialog("Gender not Selected", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (dob.Date.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Date of Birth not Selected", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {   // Inserts the data
                    conn.Insert(new PersonalInfo()
                    {
                        FirstName = fName.Text,
                        LastName = lName.Text,
                        Email = email.Text,
                        Phone = phone.Text,
                        DOB = FinalDate(),
                        Gender = DetSex().ToString()
                    });
                    Results();
                }

                ClearAll();

            }
            catch (Exception ex)
            {   // Exception to display when email i not valid or numbers not entered for phone
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You have entered invalid data", "Oops..!");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Account Name already exist, Try Different Name", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }

            }
        }

        
        private async void EditItem_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ShowConf = new MessageDialog("Do you wish to edit?", "Important");
            ShowConf.Commands.Add(new UICommand("Yes, Edit")
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
                pageHeader.PrimaryCommands.Remove(add);
                pageHeader.PrimaryCommands.Remove(edit);
                pageHeader.PrimaryCommands.Remove(delete);
                accept.Visibility = Visibility.Visible;

                DateTime birthDate = DateTime.Parse(((PersonalInfo)InfoList.SelectedItem).DOB);

                try
                {
                    fName.Text = ((PersonalInfo)InfoList.SelectedItem).FirstName;
                    lName.Text = ((PersonalInfo)InfoList.SelectedItem).LastName;
                    email.Text = ((PersonalInfo)InfoList.SelectedItem).Email;
                    phone.Text = ((PersonalInfo)InfoList.SelectedItem).Phone;
                    this.dob.Date = birthDate;
                    if (DetSex() == "Male")
                    {
                        RadBtnM.IsChecked = true;
                    }
                    else
                    {
                        RadBtnF.IsChecked = true;
                    }

                    Results();
                }
                catch (NullReferenceException)
                {
                    MessageDialog ClearDialog = new MessageDialog("Please select the item to Edit", "Oops..!");
                    await ClearDialog.ShowAsync();
                }
            }
            else
            {
                //
            }
        }

        // Displays the data when navigation between pages
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ShowConf = new MessageDialog("Do you wish to delete?", "Important");
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
                

                try
                {
                    string first = ((PersonalInfo)InfoList.SelectedItem).FirstName;
                    string last = ((PersonalInfo)InfoList.SelectedItem).LastName;
                    string birth = ((PersonalInfo)InfoList.SelectedItem).DOB;

                    var querydel = conn.Query<PersonalInfo>("DELETE FROM PersonalInfo WHERE FirstName = '" + first + 
                        "' and LastName = '" + last + "' and DOB = '" + birth + "'");
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

        private async void AcceptChange_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ShowConf = new MessageDialog("Do you wish to accept changes?", "Important");
            ShowConf.Commands.Add(new UICommand("Yes, Accept")
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


                try
                {
                    conn.Insert(new PersonalInfo()
                    {
                        FirstName = fName.Text,
                        LastName = lName.Text,
                        Email = email.Text,
                        Phone = phone.Text,
                        DOB = FinalDate(),
                        Gender = DetSex().ToString()
                    });
                    Results();
                }
                catch (NullReferenceException)
                {
                    MessageDialog ClearDialog = new MessageDialog("Please select the item to Edit", "Oops..!");
                    await ClearDialog.ShowAsync();
                }
            }
            else
            {
                //
            }
        }
    }
}