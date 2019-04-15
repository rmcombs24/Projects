using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock.Layout;

namespace MassMediaEditor
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        List<string> lstArrayFields = new List<string> {"Author", "Tags", "Composers", "Artist", "Director", "Producer", "Writer", "Genre" };
        List<string> lstDateNumericFields = new List<string> { "Date Acquired", "Date Taken", "Year", "Rating" };

        private Dictionary<string, string> dicCurrentValues = new Dictionary<string, string>();
        private List<KeyValuePair<string, string>> lstFieldValuePair = new List<KeyValuePair<string, string>>();
        private List<string> properties = new List<string>();

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

        private void wndEdit_Closed(object sender, EventArgs e)
        {
            UpdateSelectedFiles();
        }

        /*
        private void ddlFields_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (KeyValuePair<string, string> kvp in lstFieldValuePair)
            {
                if (kvp.Key == ddlFields.SelectedValue.ToString() && !(ddlFields.SelectedValue.ToString().Contains("Created") || ddlFields.SelectedValue.ToString().Contains("Date") || ddlFields.SelectedValue.ToString().Contains("Year") || ddlFields.SelectedValue.ToString().Contains("Rating")))
                {
                    CurrentValuePair = kvp;
                    txtFieldData.IsEnabled = true;
                    dpMediaEditor.IsEnabled = false;
                    sldEditor.IsEnabled = false;

                    txtFieldData.Visibility = Visibility.Visible;
                    lblSliderVal.Visibility = Visibility.Hidden;
                    spSlider.Visibility = Visibility.Hidden;
                    dpMediaEditor.Visibility = Visibility.Hidden;

                    txtFieldData.Text = kvp.Value.ToString();
                    break;
                }
                else if (kvp.Key == ddlFields.SelectedValue.ToString() && (ddlFields.SelectedValue.ToString().Contains("Date") || ddlFields.SelectedValue.ToString().Contains("Created")))
                {
                    CurrentValuePair = kvp;
                    txtFieldData.IsEnabled = false;
                    sldEditor.IsEnabled = false;
                    dpMediaEditor.IsEnabled = true;

                    spSlider.Visibility = Visibility.Hidden;
                    lblSliderVal.Visibility = Visibility.Hidden;
                    txtFieldData.Visibility = Visibility.Hidden;
                    dpMediaEditor.Visibility = Visibility.Visible;

                    dpMediaEditor.SelectedDate = (String.IsNullOrEmpty(kvp.Value.ToString())) ? DateTime.Today : DateTime.Parse(kvp.Value.ToString());
                    break;
                }
                else if (kvp.Key == ddlFields.SelectedValue.ToString() && (ddlFields.SelectedValue.ToString().Contains("Rating") || ddlFields.SelectedValue.ToString().Contains("Year")) )
                {
                    CurrentValuePair = kvp;

                    double sliderVal = 0;

                    txtFieldData.IsEnabled = false;
                    dpMediaEditor.IsEnabled = false;
                    sldEditor.IsEnabled = true;

                    spSlider.Visibility = Visibility.Visible;
                    lblSliderVal.Visibility = Visibility.Visible;
                    txtFieldData.Visibility = Visibility.Hidden;
                    dpMediaEditor.Visibility = Visibility.Hidden;

                    if (ddlFields.SelectedValue.ToString().Contains("Year"))
                    {
                        sldEditor.Minimum = 1900;
                        sldEditor.Maximum = DateTime.Now.Year;
                    }
                    else
                    {
                        sldEditor.Minimum = 0;
                        sldEditor.Maximum = 99;
                    }

                    if (double.TryParse(kvp.Value, out sliderVal))
                    {
                        sldEditor.Value = sliderVal;
                        lblSliderVal.Content = "No Rating.";
                    }

                    break;
                }

                //else if () 
                //{
                //    IDEA: how fuckin cool would it be if you had a repeater for string array props that spits out a new field each time you press a button. 
                //    That way the user never has to worry about what is the seperator, and the developer can just do author.value = array. No splits or messes 
                //}

            }

            lblUpdate.Visibility = Visibility.Hidden;
        }
        */

        private void UpdateSelectedFiles()
        {
            try
            {
               Dictionary<string, string>  updatedFields = new Dictionary<string, string>();

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
                                    ((Media)oItem).Tags = kvp.Value.Split(';');
                                    break;
                                default:
                                    break;
                            }

                            if (oItem is Picture)
                            {
                                switch (kvp.Key)
                                {
                                    case "Author":
                                        ((Picture)oItem).Authors = kvp.Value.Split(';');
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
                                            ((Audio)oItem).Composers = kvp.Value.Split(';'); ;
                                            break;
                                        case "Genre":
                                            ((Audio)oItem).Genre = kvp.Value.Split(';'); ;
                                            break;
                                        case "Track Number":
                                            ((Audio)oItem).TrackNumber = uint.Parse(kvp.Value);
                                            break;
                                        case "Contributing Artists":
                                            ((Audio)oItem).ContributingArtists = kvp.Value.Split(';'); ;
                                            break;
                                        case "Copyright":
                                            ((Audio)oItem).Copyright = kvp.Value;
                                            break;
                                        case "Subtitle":
                                            ((Audio)oItem).Subtitle = kvp.Value;
                                            break;
                                        case "Publisher":
                                            ((Audio)oItem).Publisher = kvp.Value;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                else
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
                                            ((Video)oItem).Directors = kvp.Value.Split(';');
                                            break;
                                        case "Writers":
                                            ((Video)oItem).Writers = kvp.Value.Split(';');
                                            break;
                                        case "Producers":
                                            ((Video)oItem).Producers = kvp.Value.Split(';');
                                            break;
                                        case "Contributing Artists":
                                            ((Video)oItem).ContributingArtists = kvp.Value.Split(';');
                                            break;
                                        case "Genre":
                                            ((Video)oItem).Genre = kvp.Value.Split(';');
                                                break;
                                        case "Subtitle":
                                            ((Video)oItem).Subtitle = kvp.Value;
                                            break;
                                        case "Publisher":
                                            ((Video)oItem).Publisher = kvp.Value;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e) { new ErrorLog().WriteToLog(e.Message); }
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
                        Maximum = 100
                    };

                    grdEdit.Children.Add(sldControl);
                    Grid.SetRow(sldControl, grdEdit.RowDefinitions.Count - 1);
                    Grid.SetColumn(sldControl, 1);

                    sldControl.ValueChanged += sldEditor_ValueChanged;

                    Label sliderValue = new Label
                    {
                        Name = String.Format("lblAutoGenerated_{0}", grdEdit.RowDefinitions.Count - 1),
                        Content = String.Empty,
                        Foreground = Brushes.White
                    };

                    grdEdit.Children.Add(sliderValue);
                    Grid.SetRow(sliderValue, grdEdit.RowDefinitions.Count - 1);
                    Grid.SetColumn(sliderValue, 2);
                }
                else if (currentField == "Year")
                {
                    ComboBox Years = new ComboBox();

                    for (int year = 1900; year <= DateTime.Now.Year; year++)
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
                StackPanel innerSP = ((StackPanel)outerSP.Children[0]);

                //Outer Stack Panel
                outerSP.Name = String.Format("spParent_{0}", grdEdit.RowDefinitions.Count - 1);

                //ListBox
                ((ListBox)outerSP.Children[1]).Name = String.Format("lstbxValueContainer_{0}", grdEdit.RowDefinitions.Count - 1);

                //Buttons
                ((Button)innerSP.Children[0]).Name = String.Format("btnAddValue_{0}", grdEdit.RowDefinitions.Count - 1);
                ((Button)innerSP.Children[1]).Name = String.Format("btnRemoveValue_{0}", grdEdit.RowDefinitions.Count - 1);

                //Add to the grid and set the row.
                grdEdit.Children.Add(outerSP);
                Grid.SetRow(outerSP, grdEdit.RowDefinitions.Count - 1);
            }
        }
 
        private void sldEditor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UIElement lbl = grdEdit.Children.Cast<UIElement>().Where(x => Grid.GetRow(x) == Convert.ToInt32(((Slider)sender).Name.Split('_')[1]) && Grid.GetColumn(x) == 2).FirstOrDefault();

            ((Label)lbl).Content = String.Format("{0}/100", Math.Round(((Slider)sender).Value).ToString());
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string value = String.Empty;

                // Hit Save -> Get all the fields -> keep a tally of the "New" values.
                for (int row = 0; row < grdEdit.RowDefinitions.Count; row++)
                {
                    int column = (lstArrayFields.Contains(properties[row])) ? 3 : 1;
                    var currentControl = grdEdit.Children.Cast<UIElement>().First(x => Grid.GetRow(x) == row && Grid.GetColumn(x) == column);

                    if (currentControl is ComboBox)
                    {
                        value = ((ComboBox)currentControl).SelectedValue.ToString();
                    }
                    else if (currentControl is Slider)
                    {
                        value = Math.Round(((Slider)currentControl).Value).ToString();
                    }
                    else if (currentControl is DatePicker)
                    {
                        value = ((DatePicker)currentControl).SelectedDate.Value.Date.ToShortDateString();
                    }
                    else if (currentControl is StackPanel)
                    {
                        if (((StackPanel)currentControl).Children.Count > 0)
                        {
                            value = String.Join(";", ((ListBox)((StackPanel)currentControl).Children[1]).Items.Cast<string>());
                        }
                    }
                    else
                    {
                        value = ((TextBox)currentControl).Text;
                    }

                    dicCurrentValues.Add(properties[row], value);
                }

                /* lblUpdate.Visibility = Visibility.Visible; */
            }
            catch (Exception ex)
            {
                new ErrorLog().WriteToLog(ex.Message);
            }
        }

        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            try
            {
                if (btn != null)
                {
                    //Fuckin ew
                    ListBox lsbox = ((StackPanel)VisualTreeHelper.GetParent(((StackPanel)VisualTreeHelper.GetParent(btn)))).Children[1] as ListBox;
                    UIElement tbox = grdEdit.Children.Cast<UIElement>().Where(x => Grid.GetRow(x) == Convert.ToInt32(btn.Name.Split('_')[1]) && Grid.GetColumn(x) == 1).FirstOrDefault();

                    if (!String.IsNullOrEmpty(((TextBox)tbox).Text))
                    {
                        lsbox.IsEnabled = true;
                        lsbox.Visibility = Visibility.Visible;
                        lsbox.Items.Add(((TextBox)tbox).Text);

                        ((TextBox)tbox).Text = String.Empty;
                        ((StackPanel)VisualTreeHelper.GetParent(btn)).Children[1].IsEnabled = true;
                        ((StackPanel)VisualTreeHelper.GetParent(btn)).Children[1].Visibility = Visibility.Visible;
                    }
                }
            }

            catch (Exception ex)
            {
                new ErrorLog().WriteToLog(ex.Message);
            }
        }

        private void btnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn != null)
            {
                //Fuckin ew
                ListBox lsbox = ((StackPanel)VisualTreeHelper.GetParent(((StackPanel)VisualTreeHelper.GetParent(btn)))).Children[1] as ListBox;

                if (lsbox.SelectedIndex >= 0)
                {
                    lsbox.Items.Remove(lsbox.Items[lsbox.SelectedIndex]);
                    lsbox.Items.Refresh();
                }
                if (!lsbox.HasItems)
                {
                    btn.IsEnabled = false;
                    lsbox.IsEnabled = false;
                }
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
    }
}
