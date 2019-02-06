using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace MassMediaEditor
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        public EditWindow()
        {
            InitializeComponent();
            
            //ToDo: Add logic to fill ddl, and then ability to save new field data to object.
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            
            //ToDo: Add logic to reenable the main window.
        }
    }
}
