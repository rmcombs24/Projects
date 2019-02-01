using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace MassMediaEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Multiselect = true;
            

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                var fileContent = string.Empty;

                lstvInfoBox.IsEnabled = true;
                lstvInfoBox.Items.Clear();

                string filePath = dlg.FileName;
                var file = ShellFile.FromFilePath(filePath);

                List<String> ImageHeaders = new List<string>(new string[] {"File Name" ,"Title", "Subject", "Rating", "Tags", "Comments", "Authors", "Date Taken", "Program Name", "Date Aquired", "Copyright" });

                for (int index = 0; index < ImageHeaders.Count; index++)
                {
                    if (((GridView)lstvInfoBox.View).Columns.Count <= index)
                    {
                        GridViewColumn newCol = new GridViewColumn();
                        newCol.Width = double.NaN; //Auto-size the width of the columns

                        //Next
                        //newCol.DisplayMemberBinding

                        ((GridView)lstvInfoBox.View).Columns.Add(newCol);
                    }

                    ((GridView)lstvInfoBox.View).Columns[index].Header = ImageHeaders[index];
                }

                // Set the list view to the opened files
                lstvInfoBox.ItemsSource = dlg.SafeFileNames;

                
            }
        }

        private void RdoPictures_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RdoVideo_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RdoAudio_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
