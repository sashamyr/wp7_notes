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
using System.IO.IsolatedStorage;
using System.IO;

namespace SashaNote
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {

        }

        private void AppBar_Add_Click(object sender, EventArgs e)
        {

            NavigationService.Navigate(new Uri("/SashaNote;component/Add.xaml", UriKind.Relative));

            /*
             * This was created as an easy way to get a single file
             * into the IsolatedStorage for testing it.
            
            string fileName = "2010_10_06_13_43_01_Redmond_Washington-USA.txt";
            string fileContent = "This is just a test";

            var appStorage = IsolatedStorageFile.GetUserStoreForApplication();

            if (!appStorage.FileExists(fileName))
            {
                using (var file = appStorage.CreateFile(fileName))
                {
                    using (var writer = new StreamWriter(file))
                    {
                        writer.WriteLine(fileContent);
                    }
                }
            }

            bindList();
             */
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Determine the state of the application when last used
            // and restore the state. I'll simply read the app settings
            // 'state' property and navigate accordingly. Each page 
            // and settings the state once again

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            string state = "";
            if (settings.Contains("state"))
            {
                if (settings.TryGetValue<string>("state", out state))
                {
                    if (state == "add")
                    {
                        NavigationService.Navigate(new Uri("/SashaNote;component/Add.xaml", UriKind.Relative));
                    }
                    else if (state == "edit")
                    {
                        NavigationService.Navigate(new Uri("/SashaNote;component/ViewEdit.xaml", UriKind.Relative));
                    }
                }
            }
            
            bindList();
            
            // Old way creater for testing purposes
            //var appStorage = IsolatedStorageFile.GetUserStoreForApplication();

            //string[] fileList = appStorage.GetFileNames();

            //foreach (string file in fileList)
            //{
            //    bindList();
            //}
        }

        private void bindList()
        {
            var appStorage = IsolatedStorageFile.GetUserStoreForApplication();

            string[] fileList = appStorage.GetFileNames();

            List<note> notes = new List<note>();
             

            foreach (string file in fileList)
            {
                if (file != "__ApplicationSettings")
                {

                    // Retrieve the file
                    string fileName = file;

                    // Pluck out the date parts
                    string year = file.Substring(0, 4);
                    string month = file.Substring(5, 2);
                    string day = file.Substring(8, 2);
                    string hour = file.Substring(11, 2);
                    string minute = file.Substring(14, 2);
                    string second = file.Substring(17, 2);

                    // Create a new DateTime object
                    DateTime dateCreated = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), int.Parse(hour), int.Parse(minute), int.Parse(second), 0);

                    // Parse out the location
                    string location = file.Substring(20);
                    location = location.Replace("_", ", ");
                    location = location.Replace("-", " ");
                    location = location.Substring(0, location.Length - 4);

                    notes.Add(new note() { location = location, DateCreated = dateCreated.ToLongDateString(), FileName = fileName });
                }
            }
            noteListBox.ItemsSource = notes;
        }

        private void noteLocation_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton clickedLink = (HyperlinkButton)sender;

            string uri = String.Format("/SashaNote;component/ViewEdit.xaml?id={0}", clickedLink.Tag);

            NavigationService.Navigate(new Uri(uri, UriKind.Relative));
        }

        private void AppBar_Help_Click(object sender, EventArgs e)
        {
            helpCanvas.Visibility = System.Windows.Visibility.Visible;
        }

        private void helpCloseButton_Click(object sender, RoutedEventArgs e)
        {
            helpCanvas.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}