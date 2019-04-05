using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

            List<String> properties = new List<string>();

            //We're starting at base 2 for now because we're skipping the checkbox, and fileNames.
            //Filenames may be added under the prepend/appender program at a later date.
            for (int index = 2; index < ((MainWindow)Application.Current.MainWindow).dgInfoBox.Columns.Count(); index++)
            {
                if (((MainWindow)Application.Current.MainWindow).dgInfoBox.Columns[index].Header.ToString().Length > 0)
                {
                    properties.Add(((MainWindow)Application.Current.MainWindow).dgInfoBox.Columns[index].Header.ToString());
                    lstFieldValuePair.Add(new KeyValuePair<string, string>(((MainWindow)Application.Current.MainWindow).dgInfoBox.Columns[index].Header.ToString(), String.Empty));

                    GenerateDataFields(((MainWindow)Application.Current.MainWindow).dgInfoBox, index);
                }
            }

            /*
            DataTemplate dt = (DataTemplate)grdEdit.FindResource("tmplControls");
            StackPanel spControls = ((StackPanel)dt.LoadContent());
            RowDefinition autoRow = new RowDefinition();
            autoRow.Height = new GridLength(35);



            grdEdit.RowDefinitions.Add(autoRow);
            grdEdit.Children.Add(spControls);
            Grid.SetRow(spControls, grdEdit.RowDefinitions.Count + 1);
            Grid.SetColumn(spControls, 0);
            */
        }

        private void WndEdit_Closed(object sender, EventArgs e)
        {
            UpdateSelectedFiles();
        }

        //Idea: Maybe in a future version, until save it pressed all values will be in a "draft" state.
        //EDIT: Okay so we kind of have this now, but maybe do it without hitting "Save" and leave save for actually saving and closing

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
            List<KeyValuePair<String, String>> updatedFields = new List<KeyValuePair<string, string>>();
            try
            {
                foreach (KeyValuePair<string, string> kvp in lstFieldValuePair)
                {
                    if (!String.IsNullOrEmpty(kvp.Value))
                    {
                        updatedFields.Add(kvp);
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
                                    ((Media)oItem).Tags = kvp.Value;
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
                                /* public String Creator */

                                switch (kvp.Key)
                                {

                                    default:
                                        break;
                                }

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
                                            ((Audio)oItem).Composers = kvp.Value;
                                            break;
                                        case "Genre":
                                            ((Audio)oItem).Genre = kvp.Value;
                                            break;
                                        case "Track Number":
                                            ((Audio)oItem).TrackNumber = uint.Parse(kvp.Value);
                                            break;
                                        case "Contributing Artists":
                                            ((Audio)oItem).ContributingArtists = kvp.Value;
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
                                            ((Video)oItem).Directors = kvp.Value;
                                            break;
                                        case "Writers":
                                            ((Video)oItem).Writers = kvp.Value;
                                            break;
                                        case "Producers":
                                            ((Video)oItem).Producers = kvp.Value;
                                            break;
                                        case "Contributing Artists":
                                            ((Video)oItem).ContributingArtists = kvp.Value;
                                            break;
                                        case "Copyright":
                                            ((Video)oItem).Copyright = kvp.Value;
                                            break;
                                        case "Genre":
                                            ((Video)oItem).Genre = kvp.Value;
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
                            else if (oItem is Video)
                            {

                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLog().WriteToLog(e.Message);
            }
        }

        /*
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ddlFields.SelectedValue.ToString().Contains("Date"))
                {
                    CurrentValuePair = new KeyValuePair<String, String>(CurrentValuePair.Key, dpMediaEditor.ToString());
                }
                else if (ddlFields.SelectedValue.ToString().Contains("Rating"))
                {
                    CurrentValuePair = new KeyValuePair<String, String>(CurrentValuePair.Key, sldEditor.Value.ToString());
                }
                else
                {
                    CurrentValuePair = new KeyValuePair<String, String>(CurrentValuePair.Key, txtFieldData.Text);
                }

                lstFieldValuePair[lstFieldValuePair.FindIndex(x => x.Key == CurrentValuePair.Key)] = CurrentValuePair;
                lblUpdate.Visibility = Visibility.Visible;
            }
            catch(Exception ex)
            {
                new ErrorLog().WriteToLog(ex.Message);
            }
        }
        */

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        
        private void SldEditor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            var childs = grdEdit.Children.Cast<UIElement>();
            UIElement lbl = childs.Where(x => Grid.GetRow(x) == Convert.ToInt32(((Slider)sender).Name.Split('_')[1]) && Grid.GetColumn(x) == 2).FirstOrDefault();

            ((Label)lbl).Content = String.Format("{0}/100", Math.Round(((Slider)sender).Value).ToString());
        }

        private void GenerateDataFields(DataGrid dg, int row)
        {
            List<string> lstArrayFields = new List<string> { "Tags", "Composers", "Artist", "Director", "Producer", "Writer", "Genre" };
            List<string> lstDateNumericFields = new List<string> { "Date Aquired", "Date Taken", "Year", "Rating"};

            RowDefinition autoRow = new RowDefinition();
            autoRow.Height = new GridLength(40);
            grdEdit.RowDefinitions.Add(autoRow);

            Label autoLabel = new Label
            {
                Name = String.Format("lblAutoGenerated_{0}", row),
                Content = String.Format("{0}:", dg.Columns[row].Header.ToString()),
                Foreground = Brushes.White
            };

            grdEdit.Children.Add(autoLabel);
            Grid.SetRow(autoLabel, grdEdit.RowDefinitions.Count - 1);


            if (lstDateNumericFields.Contains(dg.Columns[row].Header.ToString()))
            {

                if (dg.Columns[row].Header.ToString() == "Rating")
                {
                    
                    DataTemplate dt = (DataTemplate)grdEdit.FindResource("tmplSlider");
                    StackPanel spControls = ((StackPanel)dt.LoadContent());
                                        
                    grdEdit.Children.Add(spControls);
                    Grid.SetRow(spControls, grdEdit.RowDefinitions.Count -1);
                    Grid.SetColumn(spControls, 1);

                    ((Slider)spControls.Children[0]).ValueChanged += SldEditor_ValueChanged;
                    ((Slider)spControls.Children[0]).Name = String.Format("sldrRating_{0}", grdEdit.RowDefinitions.Count -1);

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
                else if (dg.Columns[row].Header.ToString() == "Year")
                {
                    ComboBox Years = new ComboBox();

                    for (int year = 1900; year <= DateTime.Now.Year; year++)
                    {
                        Years.Items.Add(year);
                    }
                }
            }
            else
            {
                TextBox autoTextBox = new TextBox
                {
                    Name = String.Format("txtAutoGenerated_{0}", row),
                    Width = 240,
                    Height = 25
                };

                grdEdit.Children.Add(autoTextBox);
                Grid.SetColumn(autoTextBox, 1);
                Grid.SetRow(autoTextBox, grdEdit.RowDefinitions.Count - 1);
            }


            if (lstArrayFields.Contains(dg.Columns[row].Header.ToString()))
            {
                //Stack Panel. List Box, Stack Panel, Button Button

                //In here we need to assign the name values of the ArrayValueTemplate so that we can differeniate based on the rows

                DataTemplate dt = (DataTemplate)grdEdit.FindResource("cellTemplate");
                StackPanel outerSP = ((StackPanel)dt.LoadContent());
                StackPanel innerSP = ((StackPanel)outerSP.Children[0]);

                //Outer Stack Panel
                outerSP.Name = String.Format("spParent_{0}", grdEdit.RowDefinitions.Count - 1);

                //ListBox
                ((ListBox) outerSP.Children[1]).Name = String.Format("lstbxValueContainer_{0}", grdEdit.RowDefinitions.Count - 1);

                //Buttons
                ((Button) innerSP.Children[0]).Name = String.Format("btnAddValue_{0}", grdEdit.RowDefinitions.Count - 1);
                ((Button) innerSP.Children[1]).Name = String.Format("btnRemoveValue_{0}", grdEdit.RowDefinitions.Count - 1);

                //Add to the grid and set the row.
                grdEdit.Children.Add(outerSP);
                Grid.SetRow(outerSP, grdEdit.RowDefinitions.Count - 1);
            }
        }

        private void BtnAddItem_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn != null)
            {
                //Fuckin ew
                ListBox lsbox = ((StackPanel)VisualTreeHelper.GetParent(((StackPanel)VisualTreeHelper.GetParent(btn)))).Children[1] as ListBox;
                
                var childs = grdEdit.Children.Cast<UIElement>();
                UIElement tbox = childs.Where(x => Grid.GetRow(x) == Convert.ToInt32(btn.Name.Split('_')[1]) && Grid.GetColumn(x) == 1).FirstOrDefault();

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
            else
            {
                // A nasty error occurred...
            }
        }

        private void BtnRemoveItem_Click(object sender, RoutedEventArgs e)
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
    }
}
