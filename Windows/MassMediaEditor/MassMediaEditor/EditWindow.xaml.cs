using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private List<KeyValuePair<String, String>> lstFieldValuePair = new List<KeyValuePair<String, String>>();
        private KeyValuePair<String, String> CurrentValuePair;

        public EditWindow()
        {
            InitializeComponent();

            GridView gv = (GridView)((MainWindow)Application.Current.MainWindow).lstvInfoBox.View;

            List<String> properties = new List<string>();

            foreach (GridViewColumn colProp in gv.Columns)
            {
                if (colProp.Header.ToString().Length > 0)
                {
                    properties.Add(colProp.Header.ToString());
                    lstFieldValuePair.Add(new KeyValuePair<string, string>(colProp.Header.ToString(), String.Empty));
                }
            }
            ddlFields.ItemsSource = properties;
        }

        private void WndEdit_Closed(object sender, EventArgs e)
        {
            //We're done with the edit window, give control back to the main window.
            ((MainWindow)Application.Current.MainWindow).IsEnabled = true;
        }

        //Idea: Maybe in a future version, until save it pressed all values will be in a "draft" state.
        private void ddlFields_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (KeyValuePair<string, string> kvp in lstFieldValuePair)
            {
                if (kvp.Key == ddlFields.SelectedValue.ToString())
                {
                    CurrentValuePair = kvp;
                    txtFieldData.Text = kvp.Value.ToString();
                    break;
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            CurrentValuePair = new KeyValuePair<String, String>(CurrentValuePair.Key, txtFieldData.Text);
            lstFieldValuePair[lstFieldValuePair.FindIndex(x => x.Key == CurrentValuePair.Key)] = CurrentValuePair;
        }
    }
}
