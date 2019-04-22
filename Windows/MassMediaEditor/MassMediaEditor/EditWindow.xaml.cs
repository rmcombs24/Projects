using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace MassMediaEditor
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        List<string> lstArrayFields = new List<string> {"Author", "Tags", "Composers", "Contributing Artists", "Directors", "Producers", "Writers", "Genre" };
        List<string> lstDateNumericFields = new List<string> { "Date Acquired", "Date Taken", "Media Created", "Year", "Rating" };

        private Dictionary<string, string> dicCurrentValues = new Dictionary<string, string>();
        private List<KeyValuePair<string, string>> lstFieldValuePair = new List<KeyValuePair<string, string>>();
        private List<string> properties = new List<string>();
        DispatcherTimer timer = new DispatcherTimer();

        public EditWindow()
        {
            InitializeComponent();
  
            //We're starting at base 2 for now because we're skipping the checkbox, and fileNames.
            //Filenames may be added under the prepend/appender program at a later date.
            for (int index = 2; index < ((MainWindow)Application.Current.MainWindow).dgInfoBox.Columns.Count(); index++)
            {
                if (((MainWindow)Application.Current.MainWindow).dgInfoBox.Columns[index].Header.ToString().Length > 0)
                {
                    properties.Add(((MainWindow)Application.Current.MainWindow).dgInfoBox.Columns[index].Header.ToString());
                    lstFieldValuePair.Add(new KeyValuePair<string, string>(((MainWindow)Application.Current.MainWindow).dgInfoBox.Columns[index].Header.ToString(), String.Empty));
                    GenerateDataRow(properties[index -2]);
                }
            }
        }

        private void UpdateSelectedFiles()
        {
            try
            {
                Dictionary<string, string> updatedFields = new Dictionary<string, string>();

                foreach (KeyValuePair<string, string> entry in dicCurrentValues)
                {
                    if (!String.IsNullOrEmpty(entry.Value))
                    {
                        updatedFields.Add(entry.Key, entry.Value);
                    }
                }

                foreach (object oItem in ((MainWindow)Application.Current.MainWindow).dgInfoBox.Items)
                {
                    if (((Media)oItem).isChecked == true)
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
                                    ((Media)oItem).Rating = (uint)Math.Round(double.Parse(kvp.Value));
                                    break;
                                case "Tags":
                                    ((Media)oItem).Tags = ArraySort(kvp.Value);
                                    break;
                                default:
                                    break;
                            }

                            if (oItem is Picture)
                            {
                                switch (kvp.Key)
                                {
                                    case "Author":
                                        ((Picture)oItem).Authors = ArraySort(kvp.Value);
                                        break;
                                    case "Program Name":
                                        ((Picture)oItem).ProgramName = kvp.Value;
                                        break;
                                    case "Copyright":
                                        ((Picture)oItem).Copyright = kvp.Value;
                                        break;
                                    case "Date Acquired":
                                        ((Picture)oItem).DateAcquired = DateTime.Parse(kvp.Value);
                                        break;
                                    case "Date Taken":
                                        ((Picture)oItem).DateTaken = DateTime.Parse(kvp.Value);
                                        break;

                                    default:
                                        break;
                                }
                            }
                            else if (oItem is Audio || oItem is Video)
                            {
                                if (oItem is Audio)
                                {
                                    switch (kvp.Key)
                                    {
                                        case "Album":
                                            ((Audio)oItem).Album = kvp.Value;
                                            break;
                                        case "Album Artist":
                                            ((Audio)oItem).AlbumArtist = kvp.Value;
                                            break;
                                        case "BPM":
                                            ((Audio)oItem).BPM = kvp.Value;
                                            break;
                                        case "Composers":
                                            ((Audio)oItem).Composers = ArraySort(kvp.Value);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                else if (oItem is Video)
                                {
                                    switch (kvp.Key)
                                    {
                                        case "Author URL":
                                            ((Video)oItem).AuthorURL = kvp.Value;
                                            break;
                                        case "Promotional URL":
                                            ((Video)oItem).PromoURL = kvp.Value;
                                            break;
                                        case "Year":
                                            ((Video)oItem).Year = uint.Parse(kvp.Value);
                                            break;
                                        case "Directors":
                                            ((Video)oItem).Directors = ArraySort(kvp.Value);
                                            break;
                                        case "Writers":
                                            ((Video)oItem).Writers = ArraySort(kvp.Value);
                                            break;
                                        case "Producers":
                                            ((Video)oItem).Producers = ArraySort(kvp.Value);
                                            break;
                                        default:
                                            break;
                                    }
                                }

                                switch (kvp.Key)
                                {
                                    case "Subtitle":
                                        ((Media)oItem).Subtitle = kvp.Value;
                                        break;
                                    case "Publisher":
                                        ((Media)oItem).Publisher = kvp.Value;
                                        break;
                                    case "Genre":
                                        ((Media)oItem).Genre = ArraySort(kvp.Value);
                                        break;
                                    case "Contributing Artists":
                                        ((Media)oItem).ContributingArtists = ArraySort(kvp.Value);
                                        break;
                                    default:
                                        break;
                                }
                                
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteToLog(ex.Message);
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        private string[] ArraySort(String itemArray)
        {
            string[] splitArray = itemArray.Split(';');

            //Check the config to see if sorting is enabled.
            if (((MainWindow)Application.Current.MainWindow).settings.AutoSort)
            {
                Array.Sort(splitArray, StringComparer.InvariantCulture);
            }

            return splitArray;
        }

        private void GenerateDataRow(string currentField)
        {
            RowDefinition autoRow = new RowDefinition();
            autoRow.Height = new GridLength(40);
            grdEdit.RowDefinitions.Add(autoRow);

            Label autoLabel = new Label
            {
                Name = String.Format("lblAutoGenerated_{0}", string.Join("", currentField.Split(' '))),
                Content = String.Format("{0}:", currentField),
                Foreground = Brushes.White
            };

            grdEdit.Children.Add(autoLabel);
            Grid.SetRow(autoLabel, grdEdit.RowDefinitions.Count - 1);

            if (lstDateNumericFields.Contains(currentField))
            {
                if (currentField == "Rating")
                {
                    Slider sldControl = new Slider()
                    {
                        Name = String.Format("sldrRating_{0}", grdEdit.RowDefinitions.Count - 1),
                        Minimum = 0,
                        Maximum = 100,
                        Width = 245,
                        Height = 25
                    };

                    grdEdit.Children.Add(sldControl);
                    Grid.SetRow(sldControl, grdEdit.RowDefinitions.Count - 1);
                    Grid.SetColumn(sldControl, 1);

                    sldControl.ValueChanged += sldEditor_ValueChanged;

                    Label sliderValue = new Label
                    {
                        Name = String.Format("lblAutoGenerated_{0}", grdEdit.RowDefinitions.Count - 1),
                        Content = String.Empty,
                        Foreground = Brushes.White,
                        Margin = new Thickness(0, 5, 0, 0)
                };

                    grdEdit.Children.Add(sliderValue);
                    Grid.SetRow(sliderValue, grdEdit.RowDefinitions.Count - 1);
                    Grid.SetColumn(sliderValue, 2);
                }
                else if (currentField == "Year")
                {
                    ComboBox Years = new ComboBox();
                    Years.Height = 25;
                    Years.Width = 240;
                    Years.Items.Add(String.Empty);

                    for (int year = DateTime.Now.Year; year >= 1900 ; year--)
                    {
                        Years.Items.Add(year);
                    }
                    
                    grdEdit.Children.Add(Years);
                    Grid.SetRow(Years, grdEdit.RowDefinitions.Count - 1);
                    Grid.SetColumn(Years, 1);
                }
                else
                {
                    DatePicker dp = new DatePicker()
                    {
                        Name = String.Format("dpAutoGenerated_{0}", grdEdit.RowDefinitions.Count - 1),
                        Height = 25
                    };

                    grdEdit.Children.Add(dp);
                    Grid.SetRow(dp, grdEdit.RowDefinitions.Count - 1);
                    Grid.SetColumn(dp, 1);
                }
            }
            else
            {
                TextBox autoTextBox = new TextBox
                {
                    Name = String.Format("txtAutoGenerated_{0}", string.Join("", currentField.Split(' '))),
                    Width = 240,
                    Height = 25
                };

                grdEdit.Children.Add(autoTextBox);
                Grid.SetColumn(autoTextBox, 1);
                Grid.SetRow(autoTextBox, grdEdit.RowDefinitions.Count - 1);
            }

            if (lstArrayFields.Contains(currentField))
            {
                //Stack Panel. List Box, Stack Panel, Button Button

                //In here we need to assign the name values of the ArrayValueTemplate so that we can differeniate based on the rows.
                DataTemplate dt = (DataTemplate)grdEdit.FindResource("cellTemplate");
                StackPanel outerSP = ((StackPanel)dt.LoadContent());

                //Outer Stack Panel
                outerSP.Name = String.Format("spParent_{0}", grdEdit.RowDefinitions.Count - 1);

                //ListBox
                ((ListBox)outerSP.Children[2]).Name = String.Format("lstbxValueContainer_{0}", grdEdit.RowDefinitions.Count - 1);

                //Buttons
                ((Button)outerSP.Children[0]).Name = String.Format("btnAddValue_{0}", grdEdit.RowDefinitions.Count - 1);
                ((Button)outerSP.Children[1]).Name = String.Format("btnRemoveValue_{0}", grdEdit.RowDefinitions.Count - 1);

                //Add to the grid and set the row.
                grdEdit.Children.Add(outerSP);
                Grid.SetRow(outerSP, grdEdit.RowDefinitions.Count - 1);
            }
        }
 
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string value = String.Empty;

                // Hit Save -> Get all the fields -> keep a tally of the "New" values.
                if (dicCurrentValues.Count > 0) { dicCurrentValues.Clear(); }

                for (int row = 0; row < grdEdit.RowDefinitions.Count; row++)
                {
                    int column = (lstArrayFields.Contains(properties[row])) ? 2 : 1;
                    var currentControl = grdEdit.Children.Cast<UIElement>().First(x => Grid.GetRow(x) == row && Grid.GetColumn(x) == column);

                    if (currentControl is ComboBox)
                    {
                        value = (((ComboBox)currentControl).SelectedValue != null) ? ((ComboBox)currentControl).SelectedValue.ToString() : "1900" ;
                    }
                    else if (currentControl is Slider)
                    {
                        value = Math.Round(((Slider)currentControl).Value).ToString();
                    }
                    else if (currentControl is DatePicker)
                    {
                        value = (((DatePicker)currentControl).SelectedDate.HasValue) ? ((DatePicker)currentControl).SelectedDate.Value.Date.ToShortDateString() : DateTime.MinValue.ToShortDateString();
                    }
                    else if (currentControl is StackPanel)
                    {
                        if (((StackPanel)currentControl).Children.Count > 0)
                        {
                            value = (((ListBox)((StackPanel)currentControl).Children[2]).HasItems) ? String.Join(";", ((ListBox)((StackPanel)currentControl).Children[2]).Items.Cast<string>()) : String.Empty;
                        }
                    }
                    else
                    {
                        value = ((TextBox)currentControl).Text;
                    }

                    dicCurrentValues.Add(properties[row], value);
                }

                lblUpdate.Visibility = Visibility.Visible;
                timer.Interval = TimeSpan.FromSeconds(3);
                timer.Tick += timer_Tick;
                timer.Start();
                
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteToLog(ex.Message);
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        private void btnAddRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;

                if (btn != null)
                {
                    //Fuckin ew
                    ListBox lsbox = ((StackPanel)VisualTreeHelper.GetParent(btn)).Children[2] as ListBox;
                    String[] strNameRow = btn.Name.Split('_');
                    
                    //Add
                    if (strNameRow[0].Contains("Add"))
                    {
                        UIElement tbox = grdEdit.Children.Cast<UIElement>().Where(x => Grid.GetRow(x) == Convert.ToInt32(strNameRow[1]) && Grid.GetColumn(x) == 1).FirstOrDefault();

                        if (!String.IsNullOrEmpty(((TextBox)tbox).Text))
                        {
                            lsbox.IsEnabled = true;
                            lsbox.Visibility = Visibility.Visible;
                            lsbox.Items.Add(((TextBox)tbox).Text.Trim());

                            ((TextBox)tbox).Text = String.Empty;
                            ((StackPanel)VisualTreeHelper.GetParent(btn)).Children[1].IsEnabled = true;
                            ((StackPanel)VisualTreeHelper.GetParent(btn)).Children[1].Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        //Remove
                        if (lsbox.SelectedIndex >= 0)
                        {
                            lsbox.Items.Remove(lsbox.Items[lsbox.SelectedIndex]);
                            lsbox.Items.Refresh();
                        }
                        if (!lsbox.HasItems)
                        {
                            btn.IsEnabled = false;
                            btn.Visibility = Visibility.Hidden;
                            lsbox.IsEnabled = false;
                            lsbox.Visibility = Visibility.Hidden;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteToLog(ex.Message);
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
        }
        
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxMgr mbMgr = new MessageBoxMgr();
            MessageBoxResult result = mbMgr.CreateNewResult("You will lose any unsaved changes. Proceed?", "Warning", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }

        }

        private void sldEditor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UIElement lbl = grdEdit.Children.Cast<UIElement>().Where(x => Grid.GetRow(x) == Convert.ToInt32(((Slider)sender).Name.Split('_')[1]) && Grid.GetColumn(x) == 2).FirstOrDefault();

            ((Label)lbl).Content = String.Format("{0}/100", Math.Round(((Slider)sender).Value).ToString());
        }

        private void wndEdit_Closed(object sender, EventArgs e)
        {
            UpdateSelectedFiles();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            lblUpdate.Visibility = Visibility.Hidden;
            timer.Stop();
        }
    }
}