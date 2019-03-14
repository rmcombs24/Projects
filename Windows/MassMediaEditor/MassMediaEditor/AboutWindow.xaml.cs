using System.Windows;

namespace MassMediaEditor
{
    /// <summary>
    /// Interaction logic for AboutWIndow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
