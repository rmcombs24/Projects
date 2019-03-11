using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
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
        const string filePath = "C:\\Users\\Bob\\Desktop\\config.json";

        public List<object> selectedItems = new List<object>();

        public MainWindow()
        {
            InitializeComponent();
            LoadStartupConfig();
        }

        //ToDo: This needs to either be mutable for multiple types, or have one for each.
        private void GenerateGridView<T>(DataGrid dg, List<T> MediaObjects)
        {

            if (dg.HasItems)
            {
                dg.ItemsSource = null;
                dg.Columns.Clear();
            }

            Type type = MediaObjects.GetType().GetGenericArguments()[0];

            Media m = new Media();
            Dictionary<String, Binding> headers = m.GenerateBindings<T>(type);

            for (int index = 0; index < headers.Count; index++)
            {
                if (index == 0)
                {
                    CheckBox chkAll = new CheckBox() { Name = "chkSelectAll", IsChecked = false };
                    chkAll.Checked += ChkAll_Checked;
                    chkAll.Unchecked += ChkAll_Checked;

                    DataGridCheckBoxColumn dgChk = new DataGridCheckBoxColumn
                    {
                        Header = chkAll,
                        IsReadOnly = false,
                        Binding = headers.Values.ElementAt(index)
                    };

                    dg.Columns.Add(dgChk);
                }
                else
                {
                    DataGridTextColumn dgCol = new DataGridTextColumn()
                    {
                        IsReadOnly = true,
                        Header = headers.Keys.ElementAt(index).ToString(),
                        Binding = headers.Values.ElementAt(index)
                    };

                    dg.Columns.Add(dgCol);
                }
            }

            dg.ItemsSource = MediaObjects;
            dg.Items.Refresh();
            dg.IsEnabled = true;
        }

        #region Events

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            if (rdoPictures.IsChecked == true)      { dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png;"; }
            else if (rdoAudio.IsChecked == true)    { dlg.Filter = "Audio files (*.mp3, *.wma) | *.mp3; *.wma;"; }
            else if (rdoVideo.IsChecked == true)    { dlg.Filter = "Video files (*.mkv, *.mpg, *.mpeg, *.mp4, *.wmv ) | *.mkv; *.mpg; *.mpeg; *.mp4; *.wmv;"; }

            dlg.Multiselect = true;

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                //This feels wrong. I SHOULD be able to only call GenerateGridView once. 
                //I am completely forgetting something about the mutable types. 
                if (rdoAudio.IsChecked == true)
                {
                    List<Audio> audio = new List<Audio>();

                    foreach (string fp in dlg.FileNames)
                    {
                        Audio a = new Audio(fp);
                        audio.Add(a);
                    }

                    GenerateGridView(dgInfoBox, audio);
                }
                else if (rdoPictures.IsChecked == true)
                {
                    List<Picture> pictures = new List<Picture>();

                    foreach (string fp in dlg.FileNames)
                    {
                        Picture p = new Picture(fp);
                        pictures.Add(p);
                    }

                    GenerateGridView(dgInfoBox, pictures);
                }
                else if (rdoVideo.IsChecked == true)
                {
                    List<Video> videos = new List<Video>();

                    foreach (string fp in dlg.FileNames)
                    {
                        Video v = new Video(fp);
                        videos.Add(v);
                    }

                    GenerateGridView(dgInfoBox, videos);
                }

                btnClear.IsEnabled = true;
                btnCommit.IsEnabled = true;
                btnEdit.IsEnabled = true;
            }
        }

        private void ChkAll_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;

            if (cb.IsChecked == true)
            {
                foreach (Media item in dgInfoBox.Items)
                {
                    item.isChecked = true;
                }
            }
            else
            {
                foreach (Media item in dgInfoBox.Items)
                {
                    item.isChecked = false;
                }
            }

            //Why do you have to commit twice?? That's so stupid. SO never lies though.
            dgInfoBox.CommitEdit();
            dgInfoBox.CommitEdit();
            dgInfoBox.Items.Refresh();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            EditWindow editWindow = new EditWindow();

            //I had this setup as enabled/disabled with a listview, but a datagrid wants to be a pain, so this will work for now.
            if (dgInfoBox.ItemsSource.Cast<Media>().Where(x => x.isChecked == true).ToList().Count > 0)
            {
                editWindow.ShowDialog();
                dgInfoBox.Items.Refresh();
            }
            else { new MessageBoxMgr().ItemsRequiredMessage(); }
        }

        private void BtnCommit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxMgr mbMgr = new MessageBoxMgr();

            if (dgInfoBox.ItemsSource.Cast<Media>().Where(x => x.isChecked == true).ToList().Count > 0)
            {
                MessageBoxResult messageBoxCommit = mbMgr.CreateNewResult("Are you sure you want to save these changes? This cannot be undone.", "Edit Confirmation", MessageBoxButton.YesNo);

                if (messageBoxCommit == MessageBoxResult.Yes)
                {
                    Media mFile = new Media();
                    bool completedWithoutErrors = true;
                    List<bool> listCompletion = new List<bool>();

                    BackgroundWorker worker = new BackgroundWorker();

                    worker.DoWork += (o, ea) =>
                    {


                        foreach (Media item in dgInfoBox.Items)
                        {
                            if (item.isChecked)
                            {

                                completedWithoutErrors = mFile.WriteToShellFile(item);

                                listCompletion.Add(completedWithoutErrors);
                            }
                        }

                    };

                    worker.RunWorkerCompleted += (o, ea) =>
                    {
                        //work has completed. you can now interact with the UI

                        if (listCompletion.Contains(false))
                        {

                            mbMgr.CompleteMessage(false);

                        }
                        else
                        { mbMgr.CompleteMessage(true); }

                        biIsWorking.IsBusy = false;
                    };

                    biIsWorking.IsBusy = true;
                    worker.RunWorkerAsync();

                }
                else { /*Do nothing*/ }
            }
            else { mbMgr.ItemsRequiredMessage(); }
        }

        #endregion

        #region Settings

        public struct Settings
        {
            public string Media_Type;
            public string Theme;
        }

        private void LoadStartupConfig()
        {
            Settings settings;

            // deserialize JSON directly from a file
            using (StreamReader file = File.OpenText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                settings = (Settings)serializer.Deserialize(file, typeof(Settings));
            }

            ReadConfig(settings);
        }

        private void ReadConfig(Settings config)
        {
            //Gotta find a way to instantiate this config easily.
        }

        #endregion


        //Idefk
        private void Menu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftAlt || e.Key == Key.RightAlt)
                mnuTitlebar.Visibility = Visibility.Visible;
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            if (dgInfoBox.HasItems)
            {
                dgInfoBox.ItemsSource = null;
                dgInfoBox.Columns.Clear();

                btnClear.IsEnabled = false;
                btnCommit.IsEnabled = false;
                btnEdit.IsEnabled = false;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }
    }
}
