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

            dlg.Multiselect = true;
            

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                lstvInfoBox.IsEnabled = true;
                List<Picture> pictures = new List<Picture>();

                foreach (string fp in dlg.FileNames)
                {                    
                    Picture p = new Picture(fp);
                    pictures.Add(p);
                }

                GridView  gv = GenerateGridView();

                lstvInfoBox.View = gv;
                lstvInfoBox.ItemsSource = pictures;
            }
        }


        //ToDo: This needs to either be mutable for types, or have one for each type
        private static GridView GenerateGridView()
        {
            Picture p = new Picture();
            GridView gv = new GridView();
            Dictionary<string,Binding>  headers =  p.GenerateBindings();

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
    }
}
