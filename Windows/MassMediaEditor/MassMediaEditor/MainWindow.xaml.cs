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
using System.Windows.Controls.Primitives;

namespace MassMediaEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<object> selectedItems = new List<object>();

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

                GenerateGridView(lvInfoBox, pictures);
            }
        }


        //ToDo: This needs to either be mutable for multiple types, or have one for each.
        private  void GenerateGridView(ListView lv, List<Picture> pictures)
        {
            Picture p = new Picture();
            GridView gv = new GridView();
            Dictionary <String, Binding>  headers =  p.GenerateBindings();
            DataTemplate dt = (DataTemplate)lv.FindResource("tmplCheckbox");
            DependencyObject depObj = dt.LoadContent();

            ((CheckBox)depObj).SetBinding(ToggleButton.IsCheckedProperty, headers.Values.ElementAt(0));

            gv.Columns.Add(
                new GridViewColumn { Header = String.Empty, CellTemplate = dt });

            for (int index = 1; index < headers.Count; index++)
            {
                GridViewColumn newCol = new GridViewColumn();
                newCol.Header = headers.Keys.ElementAt(index).ToString();
                newCol.Width = double.NaN; //Auto-size the width of the columns
                newCol.DisplayMemberBinding = headers.Values.ElementAt(index);
                gv.Columns.Add(newCol);
            }

            lv.View = gv;
            lv.ItemsSource = pictures;
            lv.IsEnabled = true;
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

        private void checkBox_Checking(object sender, RoutedEventArgs e)
        {
            var chkBox = (CheckBox) e.OriginalSource;
            var dataSource = (Picture) chkBox.DataContext;

            if (chkBox.IsChecked == true && !selectedItems.Contains(dataSource))
            {
                selectedItems.Add(dataSource);
            }
            else if ((chkBox.IsChecked == false && selectedItems.Contains(dataSource)))
            {
                selectedItems.Remove(dataSource);
            }

            //If no files are selected turn off the edit button, otherwise keep it on.
            btnEdit.IsEnabled = selectedItems.Count > 0 ? btnEdit.IsEnabled = true : btnEdit.IsEnabled = false;            
        }
    }
}
