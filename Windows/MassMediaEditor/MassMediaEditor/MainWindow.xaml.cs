using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

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
            Settings.LoadStartupConfig();
            LoadSettings();
        }

        private void GenerateGridView (DataGrid dg, List<Media> MediaObjects, MediaType mediaType)
        {
            if (dg.HasItems)
            {
                dg.ItemsSource = null;
                dg.Columns.Clear();
            }

            Type type = MediaObjects.GetType().GetGenericArguments()[0];
            Dictionary<String, Binding> headers = Media.GenerateBindings(mediaType);

            for (int index = 0; index < headers.Count; index++)
            {
                if (index == 0)
                {
                    CheckBox chkAll = new CheckBox() { Name = "chkSelectAll", IsChecked = false };
                    chkAll.Checked += chkAll_Checked;
                    chkAll.Unchecked += chkAll_Checked;

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
                        Binding = headers.Values.ElementAt(index)
                    };

                    //We need to be able select a field based on the editable fields. 1 is ALWAYS filename, which we aren't doing (Yet)
                    if (index != 1)
                    {
                        CheckBox chkHeader = new CheckBox()
                        {
                            Content = headers.Keys.ElementAt(index).ToString(),
                            Name = String.Format("chk{0}", headers.Keys.ElementAt(index).ToString().Replace(" ", String.Empty)),
                        };

                        dgCol.Header = chkHeader;
                    }
                    else
                    {
                        dgCol.Header = headers.Keys.ElementAt(index).ToString();
                    }

                    dg.Columns.Add(dgCol);
                }
            }

            dg.ItemsSource = MediaObjects;
            dg.Items.Refresh();
            dg.IsEnabled = true;
            dg.Visibility = Visibility = Visibility.Visible;
            dgInfoBox.Background = Brushes.White;   //"";   //"#FF4B4B4B"
        }

        private void LoadSettings()
        {
            if (Settings.DefaultMediaType() == MediaType.Audio)
            {
                rdoAudio.IsChecked = true;
            }
            else if (Settings.DefaultMediaType() == MediaType.Video)
            {
                rdoVideo.IsChecked = true;
            }
            else if (Settings.DefaultMediaType() == MediaType.Pictures)
            {
                rdoPictures.IsChecked = true;
            }
        }

        #region Public Events
        private void mnuAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }
        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
        private void mnuSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }
        private void mnuErrorLog_Click(object sender, RoutedEventArgs e)
        {
            ErrorLogWindow errorLogWindow = new ErrorLogWindow();
            errorLogWindow.ShowDialog();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            MediaType mediaType = new MediaType();

            if (rdoPictures.IsChecked == true)      { mediaType = MediaType.Pictures; dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png;"; }
            else if (rdoVideo.IsChecked == true)    { mediaType = MediaType.Video; dlg.Filter = "Video files (*.mkv, *.mpg, *.mpeg, *.mp4, *.wmv ) | *.mkv; *.mpg; *.mpeg; *.mp4; *.wmv;"; }
            else if (rdoAudio.IsChecked == true)    { mediaType = MediaType.Audio; dlg.Filter = "Audio files (*.mp3, *.wma) | *.mp3; *.wma;"; }

            dlg.Multiselect = true;

            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                List<Media> lstMediaObjects = new List<Media>();

                foreach (string fp in dlg.FileNames)
                {
                    if (rdoAudio.IsChecked == true) { lstMediaObjects.Add(new Audio(fp)); }
                    else if (rdoPictures.IsChecked == true) { lstMediaObjects.Add(new Picture(fp)); }
                    else if (rdoVideo.IsChecked == true) { lstMediaObjects.Add(new Video(fp)); }
                }

                GenerateGridView(dgInfoBox, lstMediaObjects, mediaType);
                btnClear.IsEnabled = true;
                btnCommit.IsEnabled = true;
                btnEdit.IsEnabled = true;
            }
        }
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            if (dgInfoBox.HasItems)
            {
                dgInfoBox.ItemsSource = null;
                dgInfoBox.Columns.Clear();
                dgInfoBox.Visibility = Visibility.Hidden;
                dgInfoBox.IsEnabled = false;

                btnClear.IsEnabled = false;
                btnCommit.IsEnabled = false;
                btnEdit.IsEnabled = false;
            }
        }
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            EditWindow editWindow = new EditWindow();

            //I had this setup as enabled/disabled with a listview, but a datagrid wants to be a pain, so this will work for now.
            if (dgInfoBox.ItemsSource.Cast<Media>().Where(x => x.isChecked == true).ToList().Count > 0)
            {
                editWindow.ShowDialog();
                dgInfoBox.Items.Refresh();
            }
            else { MessageBoxMgr.ItemsRequiredMessage(); }
        }
        private void btnCommit_Click(object sender, RoutedEventArgs e)
        {
            if (dgInfoBox.ItemsSource.Cast<Media>().Where(x => x.isChecked == true).ToList().Count > 0)
            {
                MessageBoxResult messageBoxCommit = MessageBoxMgr.CreateNewResult("Are you sure you want to save these changes? This cannot be undone.", "Edit Confirmation", MessageBoxButton.YesNo);

                if (messageBoxCommit == MessageBoxResult.Yes)
                {
                    bool completedWithoutErrors = true;
                    List<bool> listCompletion = new List<bool>();

                    BackgroundWorker worker = new BackgroundWorker();

                    worker.DoWork += (o, ea) =>
                    {
                        foreach (Media item in dgInfoBox.Items)
                        {
                            if (item.isChecked)
                            {
                                completedWithoutErrors = Media.WriteToShellFile(item);
                                listCompletion.Add(completedWithoutErrors);
                            }
                        }
                    };

                    worker.RunWorkerCompleted += (o, ea) =>
                    {
                        //work has completed. you can now interact with the UI
                        if (listCompletion.Contains(false)) { MessageBoxMgr.CompleteMessage(false); }
                        else { MessageBoxMgr.CompleteMessage(true); }

                        biIsWorking.IsBusy = false;
                    };

                    biIsWorking.IsBusy = true;
                    worker.RunWorkerAsync();
                }
            }
            else { MessageBoxMgr.ItemsRequiredMessage(); }
        }

        private void chkAll_Checked(object sender, RoutedEventArgs e)
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

            //Why do you have to commit twice?? That's so stupid. S.O. never lies though.
            dgInfoBox.CommitEdit();
            dgInfoBox.CommitEdit();
            dgInfoBox.Items.Refresh();
        }
        #endregion

    }
}
