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
    public partial class ViewEdit : PhoneApplicationPage
    {

        private string fileName = "";

        public ViewEdit()
        {
            InitializeComponent();
        }

        private void AppBar_Back_Click(object sender, EventArgs e)
        {
            navigateBack();
        }

        private void AppBar_Edit_Click(object sender, EventArgs e)
        {
            if (displayTextBlock.Visibility == System.Windows.Visibility.Visible)
            {
                editTextBox.Text = displayTextBlock.Text;
                displayTextBlock.Visibility = System.Windows.Visibility.Collapsed;
                editTextBox.Visibility = System.Windows.Visibility.Visible;

            }
        }

        private void AppBar_Save_Click(object sender, EventArgs e)
        {
            if (editTextBox.Visibility == System.Windows.Visibility.Visible)
            {
                var appStorage = IsolatedStorageFile.GetUserStoreForApplication();

                using (var file = appStorage.OpenFile(fileName, FileMode.Create))
                {
                    using (StreamWriter sw = new StreamWriter(file))
                    {
                        sw.WriteLine(editTextBox.Text);
                    }
                }
                displayTextBlock.Text = editTextBox.Text;
                displayTextBlock.Visibility = System.Windows.Visibility.Visible;
                editTextBox.Visibility = System.Windows.Visibility.Collapsed;

            }
        }

        private void AppBar_Delete_click(object sender, EventArgs e)
        {
            confirmDialog.Visibility = System.Windows.Visibility.Visible;
        }

        private void navigateBack()
        {
            NavigationService.Navigate(new Uri("/SashaNote;component/MainPage.xaml", UriKind.Relative));
        }


        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            fileName = NavigationContext.QueryString["id"];

            var appStorage = IsolatedStorageFile.GetUserStoreForApplication();

            using (var file = appStorage.OpenFile(fileName, System.IO.FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    displayTextBlock.Text = sr.ReadToEnd();
                }

            }
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            var appStorage = IsolatedStorageFile.GetUserStoreForApplication();

            appStorage.DeleteFile(fileName);

            navigateBack();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            confirmDialog.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}