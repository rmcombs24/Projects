using System.Windows;

namespace MassMediaEditor
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            LoadConfigSettings();
         }

        private void LoadConfigSettings()
        {
            ddlMediaType.SelectedIndex = ((MainWindow)Application.Current.MainWindow).settings.MediaType;

            if (((MainWindow)Application.Current.MainWindow).settings.AutoSort)
            {
                rdoSortYes.IsChecked = true;
            }
            else
            {
                rdoSortNo.IsChecked = true;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Settings newSettings = new Settings
            {
                AutoSort = (rdoSortYes.IsChecked == true) ? true : false,
                MediaType = ddlMediaType.SelectedIndex
            };

            ((MainWindow)Application.Current.MainWindow).settings.WriteToSettingsConfig(newSettings);

            this.Close();
        }

        private void rdoSort_Checked(object sender, RoutedEventArgs e)
        {
            if (rdoSortYes.IsChecked == true)
            {
                rdoSortNo.IsChecked = false;
            }
            else if (rdoSortNo.IsChecked == true)
            {
                rdoSortYes.IsChecked = false;
            }
        }
    }
}
