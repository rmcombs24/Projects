using System;
using System.Data;
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

            if (rdoPictures.IsChecked == true)
            {
                dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            }
            else if (rdoAudio.IsChecked == true) { }
            else if (rdoVideo.IsChecked == true) { }

            dlg.Multiselect = true;
            
            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                List<Picture> pictures = new List<Picture>();

                foreach (string fp in dlg.FileNames)
                {                    
                    Picture p = new Picture(fp);
                    pictures.Add(p);
                }

                GridView  gv = GenerateGridView();

                lstvInfoBox.View = gv;                
                lstvInfoBox.ItemsSource = pictures;
                lstvInfoBox.IsEnabled = true;

                btnEdit.IsEnabled = true;   
            }
        }


        //ToDo: This needs to either be mutable for types, or have one for each type
        private static GridView GenerateGridView()
        {
            Picture p = new Picture();
            GridView gv = new GridView();
            Dictionary<String, Binding>  headers =  p.GenerateBindings();
            Window window = Application.Current.MainWindow;
            DataTemplate s = (DataTemplate)window.FindResource("dtmplCheckbox");
            gv.Columns.Add(new GridViewColumn { Header = String.Empty, CellTemplate = s });

            for (int index = 0; index < headers.Count; index++)
            {
                GridViewColumn newCol = new GridViewColumn();
                newCol.Header = headers.Keys.ElementAt(index).ToString();
                newCol.Width = double.NaN; //Auto-size the width of the columns
                newCol.DisplayMemberBinding = headers.Values.ElementAt(index);
                gv.Columns.Add(newCol);
            }

            return gv;
        }


        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            EditWindow editWindow = new EditWindow();

            editWindow.Show();
        }
    }
}
