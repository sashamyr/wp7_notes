using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Device.Location;
using System.IO.IsolatedStorage;
using System.Text;
using System.IO;

namespace SashaNote
{
    public partial class Add : PhoneApplicationPage
    {

        private IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        private string location = "";

        public Add()
        {
            InitializeComponent();

            try
            {
                GeoCoordinateWatcher myWatcher = new GeoCoordinateWatcher();

                var myPosition = myWatcher.Position;

                double latitude = 47.674;
                double longitude = -122.12;


                if (!myPosition.Location.IsUnknown)
                {
                    latitude = myPosition.Location.Latitude;
                    longitude = myPosition.Location.Longitude;
                }

                myTerraService.TerraServiceSoapClient client = new myTerraService.TerraServiceSoapClient();

                client.ConvertLonLatPtToNearestPlaceCompleted += new EventHandler<myTerraService.ConvertLonLatPtToNearestPlaceCompletedEventArgs>(client_ConvertLonLatPtToNearestPlaceCompleted);

                client.ConvertLonLatPtToNearestPlaceAsync(new myTerraService.LonLatPt() { Lat = latitude, Lon = longitude });
            }
            catch
            {
                // To be implemented
            }
        }

        void client_ConvertLonLatPtToNearestPlaceCompleted(object sender, myTerraService.ConvertLonLatPtToNearestPlaceCompletedEventArgs e)
        {
            location = e.Result;

            //throw new NotImplementedException();
        }

        private void AppBar_Cancel_click(object sender, EventArgs e)
        {
            navigateBack();
        }

        private void AppBar_Save_Click(object sender, EventArgs e)
        {
            // Save the new note
            if (location.Trim().Length == 0)
            {
                location = "unknown";
            }

            // Construct the name of the file
            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now.Year);
            sb.Append("_");
            sb.Append(String.Format("{0:00}", DateTime.Now.Month));
            sb.Append("_");
            sb.Append(String.Format("{0:00}", DateTime.Now.Day));
            sb.Append("_");
            sb.Append(String.Format("{0:00}", DateTime.Now.Hour));
            sb.Append("_");
            sb.Append(String.Format("{0:00}", DateTime.Now.Minute));
            sb.Append("_");
            sb.Append(String.Format("{0:00}", DateTime.Now.Second));
            sb.Append("_");

            location = location.Replace(" ", "-");
            location = location.Replace(", ", "_");
            sb.Append(location);
            sb.Append(".txt");

            // Now we have everything we need  to write the file to Isolated Storage
            var appStorage = IsolatedStorageFile.GetUserStoreForApplication();


            try
            {
                using (var fileStream = appStorage.OpenFile(sb.ToString(), System.IO.FileMode.Create))
                {
                    using (StreamWriter sw = new StreamWriter(fileStream))
                    {
                        sw.WriteLine(editTextBox.Text);
                    }
                }
            }
            catch
            {
                // To be implemented
            }
            

            // Finished, navigate back to the MainPage
            navigateBack();
        }

        private void navigateBack()
        {
            // Reset the application state used to ensure that the application
            // re-opens in the correct state

            settings["state"] = "";
            settings["value"] = "";

            NavigationService.Navigate(new Uri("/SashaNote;component/MainPage.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Firstly check to make sure we're note returning
            // from an interrupted session, redirected from
            // MainPage.xaml. We'll check IsolatedStorageSettings.

            

            string state = "";
            if (settings.Contains("state"))
            {
                if (settings.TryGetValue<string>("state", out state))
                {
                    if (state == "add")
                    {
                        string value = "";
                        if (settings.Contains("value"))
                        {
                            if (settings.TryGetValue<string>("value", out value))
                            {
                                editTextBox.Text = value;
                            }
                        }
                    }
                }
            }

            settings["state"] = "add";
            settings["value"] = editTextBox.Text;

            editTextBox.Focus();
            editTextBox.SelectionStart = editTextBox.Text.Length;
        }

        private void editTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            settings["value"] = editTextBox.Text;
        }
    }
}