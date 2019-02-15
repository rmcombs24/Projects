using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
            
            DataGrid dg = ((MainWindow)Application.Current.MainWindow).dgInfoBox;            
            List<String> properties = new List<string>();

            //We're starting at base 2 for now because we're skipping the checkbox, and fileNames.
            //Filenames may be added under the prepend/appender program at a later date.
            for (int index = 2; index < dg.Columns.Count(); index++) 
            {
                if ((dg.Columns[index]).Header.ToString().Length > 0)
                {
                    properties.Add((dg.Columns[index]).Header.ToString());
                    lstFieldValuePair.Add(new KeyValuePair<string, string>((dg.Columns[index]).Header.ToString(), String.Empty));
                }
            }

            lblUpdate.Visibility = Visibility.Hidden;
            ddlFields.ItemsSource = properties;
        }

        private void WndEdit_Closed(object sender, EventArgs e)
        {
            UpdateSelectedFiles();
        }

        //Idea: Maybe in a future version, until save it pressed all values will be in a "draft" state.
        //EDIT: Okay so we kind of have this now, but maybe do it without hitting "Save" and leave save for actually saving and closing.
        private void ddlFields_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (KeyValuePair<string, string> kvp in lstFieldValuePair)
            {
                if (kvp.Key == ddlFields.SelectedValue.ToString() && !ddlFields.SelectedValue.ToString().Contains("Date"))
                {
                    CurrentValuePair = kvp;
                    txtFieldData.IsEnabled = true;
                    txtFieldData.Visibility = Visibility.Visible;
                    dpMediaEditor.IsEnabled = false;
                    dpMediaEditor.Visibility = Visibility.Hidden;

                    txtFieldData.Text = kvp.Value.ToString();
                    break;
                }
                else if (kvp.Key == ddlFields.SelectedValue.ToString() && ddlFields.SelectedValue.ToString().Contains("Date"))
                {
                    CurrentValuePair = kvp;
                    txtFieldData.IsEnabled = false;
                    txtFieldData.Visibility = Visibility.Hidden;
                    dpMediaEditor.IsEnabled = true;
                    dpMediaEditor.Visibility = Visibility.Visible;

                    dpMediaEditor.Text = kvp.Value.ToString();
                    break;
                }
            }

            lblUpdate.Visibility = Visibility.Hidden;
        }

        private void UpdateSelectedFiles()
        {
            List<KeyValuePair<String, String>> updatedFields = new List<KeyValuePair<string, string>>();

            foreach (KeyValuePair<string, string> kvp in lstFieldValuePair)
            {
                if (!String.IsNullOrEmpty(kvp.Value))
                {
                    updatedFields.Add(kvp);
                }
            }
            
            foreach (object oItem in ((MainWindow)Application.Current.MainWindow).dgInfoBox.Items)
            {
                if(((Media)oItem).isChecked == true)
                {
                    foreach (KeyValuePair<string, string> kvp in updatedFields)
                    {
                        //Since we're dealing with a mutable type of objects.
                        //If the object has a valid property field then we can update the file
                        //To have the new properties that were used in the edit window.

                        switch (kvp.Key)
                        {
                            case "Title":
                                ((Media)oItem).Title = kvp.Value;
                                break;

                            case "Subject":
                                ((Media)oItem).Subject = kvp.Value;
                                break;

                            case "Comments":
                                ((Media)oItem).Comments = kvp.Value;
                                break;

                            case "Rating":
                                ((Media)oItem).Rating = uint.Parse(kvp.Value);
                                break;

                            default:
                                break;
                        }

                        if (oItem is Picture)
                        {
                            switch (kvp.Key)
                            {
                                case "Author":
                                   ((Picture)oItem).Authors = kvp.Value;
                                    break;
                                case "Program Name":
                                    ((Picture)oItem).ProgramName = kvp.Value;
                                    break;
                                case "Copyright":
                                    ((Picture)oItem).Copyright= kvp.Value;
                                    break;
                                case "DateAcquired":
                                    //((Picture)oItem).DateAquired= kvp.Value;
                                    break;
                                case "DateTaken":
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if (oItem is Audio) { }
                        else if (oItem is Video) { }
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            CurrentValuePair = new KeyValuePair<String, String>(CurrentValuePair.Key, txtFieldData.Text);
            lstFieldValuePair[lstFieldValuePair.FindIndex(x => x.Key == CurrentValuePair.Key)] = CurrentValuePair;

            lblUpdate.Visibility = Visibility.Visible;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
