using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Division2Toolkit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {            
            InitializeComponent();
            Title = String.Format("The Division 2 Toolkit v{0}", Version.GetVersion());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            expMainMenu.IsExpanded = false;
        }
    }
}
