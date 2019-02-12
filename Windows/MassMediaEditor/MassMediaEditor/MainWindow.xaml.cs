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

        public List<object> selectedItems = new List<object>();

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

                GenerateGridView(dgInfoBox, pictures);
            }
        }


        //ToDo: This needs to either be mutable for multiple types, or have one for each.
        private static void GenerateGridView(DataGrid dg, List<Picture> pictures)
        {
            Picture p = new Picture();
            Dictionary <String, Binding>  headers =  p.GenerateBindings();
            
            for (int index = 0; index < headers.Count; index++)
            {
                if (index == 0)
                {
                    CheckBox chkAll = new CheckBox()
                    {
                        Name = "chkSelectAll"
                    };

                    chkAll.Checked += ChkAll_Checked;

                    DataGridCheckBoxColumn dgChk = new DataGridCheckBoxColumn
                    {
                        Header = chkAll
                    };

                    dgChk.IsReadOnly = false;
                    dgChk.Binding = headers.Values.ElementAt(index);
                    dg.Columns.Add(dgChk);
                }
                else
                {
                    DataGridTextColumn dgCol = new DataGridTextColumn();
                    dgCol.IsReadOnly = true;
                    dgCol.Header = headers.Keys.ElementAt(index).ToString();
                    dgCol.Binding = headers.Values.ElementAt(index);
                    dg.Columns.Add(dgCol);
                }
            }

            dg.ItemsSource = pictures;
            dg.IsEnabled = true;
        }

        private static void ChkAll_Checked(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            EditWindow editWindow = new EditWindow();

            editWindow.ShowDialog();
            dgInfoBox.Items.Refresh();
        }

        private void checkBox_Checking(object sender, RoutedEventArgs e)
        {
            var chkBox = (CheckBox) e.OriginalSource;
            var dataSource = (object) chkBox.DataContext; //Mutable

            if (chkBox.Name == "chkSelectAll" && chkBox.IsChecked == true)
            {
                foreach (object item in dgInfoBox.Items)
                {

                    ((Media)item).isChecked = true;
                    dgInfoBox.Items.Refresh();
                }
            }
            else if (chkBox.IsChecked == true && !selectedItems.Contains(dataSource))
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

        private void BtnCommit_Click(object sender, RoutedEventArgs e)
        {
            Media mFile = new Media();

            foreach (object item in selectedItems)
            {
                mFile.WriteToShellFile(item);
            }
        }

        private void DataGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            // Lookup for the source to be DataGridCell
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                // Starts the Edit on the row;
                DataGrid grd = (DataGrid)sender;
                grd.BeginEdit(e);
            }
        }
    }
}
