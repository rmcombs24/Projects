﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        //I'm going to redo these with reflection..soon(tm).
        private List<string> lstArrayFields = new List<string> { "Authors", "Tags", "Composer", "Contributing Artists", "Directors", "Producers", "Writers", "Genre" };
        private List<string> lstDateNumericFields = new List<string> { "Date Acquired", "Date Taken", "Media Created", "Year", "Rating" };

        private Dictionary<string, string> dicCurrentValues = new Dictionary<string, string>();
        private List<string> propertiesSortedBySection = new List<string>();
        private readonly DispatcherTimer timer = new DispatcherTimer();

        public EditWindow()
        {
            InitializeComponent();
            List<string> properties = new List<string>();
            
            //Field Select -- Start by looking at all the headers and see if ANY are checked, if yes, show ONLY those, otherwise show ALL
            List<DataGridColumn> lstCheckedCols = ((MainWindow)Application.Current.MainWindow).dgInfoBox.Columns.ToList().FindAll(
                    x => x is DataGridTextColumn).ToList().FindAll(
                        y => y.Header is CheckBox).ToList().FindAll(
                            z => ((CheckBox)z.Header).IsChecked == true);

            if (lstCheckedCols.Count > 0)
            {                
                for (int index = 0; index < lstCheckedCols.Count; index++)
                {
                    properties.Add(((CheckBox)lstCheckedCols[index].Header).Content.ToString());
                }
            }
            else
            {
                for (int index = 2; index < ((MainWindow) Application.Current.MainWindow).dgInfoBox.Columns.Count(); index++)
                {
                    if (((MainWindow)Application.Current.MainWindow).dgInfoBox.Columns[index].Header.ToString().Length > 0)
                    {
                        properties.Add(((CheckBox)((MainWindow) Application.Current.MainWindow).dgInfoBox.Columns[index].Header).Content.ToString());
                    }
                }
            }

            SortHeadersBySections(properties);
        }

        private void GenerateDataRow(string currentField)
        {
            RowDefinition autoRow = new RowDefinition { Height = new GridLength(40), Name = "rowTextValue" };
            grdEdit.RowDefinitions.Add(autoRow);

            Label autoLabel = new Label
            {
                Name = String.Format("lblAutoGenerated_{0}", string.Join("", currentField.Split(' '))),
                Content = String.Format("{0}:", currentField),
                Foreground = Brushes.White
            };

            grdEdit.Children.Add(autoLabel);
            Grid.SetRow(autoLabel, grdEdit.RowDefinitions.Count - 1);

            //Column 1
            if (lstDateNumericFields.Contains(currentField))
            {
                if (currentField == "Rating")
                {
                    Slider sldControl = new Slider()
                    {
                        Name = String.Format("sldrRating_{0}", grdEdit.RowDefinitions.Count - 1),
                        Minimum = 0,
                        Maximum = 99,
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

                    for (int year = DateTime.Now.Year; year >= 1900; year--)
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
                    autoRow.Name = "rowDateTimeValue";
                }

                autoRow.Name = "rowIntValue";
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

            //Column 2
            if (lstArrayFields.Contains(currentField))
            {
                //In here we need to assign the name values of the ValueTemplate so that we can differeniate based on the rows.
                DataTemplate dt = (DataTemplate)grdEdit.FindResource("tmplArray");
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
                autoRow.Name = "rowArrayValues";
            }
        }

        private void GenerateHeaderAndRows(MediaSection section, List<string> sectionProperties)
        {
            //Header 
            DataTemplate dt = (DataTemplate)grdEdit.FindResource("tmplHeader");
            StackPanel spOuter = (StackPanel)dt.LoadContent();
            RowDefinition autoRow = new RowDefinition { Height = new GridLength(45), Name= "rowSectionHeader" };

            ((Label)spOuter.Children[0]).Content = section;

            grdEdit.Children.Add(spOuter);
            grdEdit.RowDefinitions.Add(autoRow);

            Grid.SetRow(spOuter, grdEdit.RowDefinitions.Count - 1);

            //Rows
            foreach (string properties in sectionProperties)
            {
                propertiesSortedBySection.Add(properties);
                GenerateDataRow(properties);
            }
        }

        private void SortHeadersBySections(List<string> headers)
        {
            Dictionary<string, MediaSection> dicSortedSections = new Dictionary<string, MediaSection>();

            Media mediaType;

            if (((MainWindow)Application.Current.MainWindow).dgInfoBox.Items.CurrentItem is Audio) { mediaType = new Audio(); }
            else if (((MainWindow)Application.Current.MainWindow).dgInfoBox.Items.CurrentItem is Picture) { mediaType = new Picture(); }
            else { mediaType = new Video(); }

            //Headers has the current headers for whatever we're looking at, so we iterate through it, and put these into the sections they should be in
            foreach (string property in headers)
            {
                if (Media.GetMediaSection(mediaType, property) == MediaSection.Description)         { dicSortedSections.Add(property, MediaSection.Description); }
                else if (Media.GetMediaSection(mediaType, property) == MediaSection.Origin)         { dicSortedSections.Add(property, MediaSection.Origin); }
                else if (Media.GetMediaSection(mediaType, property) == MediaSection.Media)          { dicSortedSections.Add(property, MediaSection.Media); }
                else if (Media.GetMediaSection(mediaType, property) == MediaSection.Content)        { dicSortedSections.Add(property, MediaSection.Content); }
                else if (Media.GetMediaSection(mediaType, property) == MediaSection.Camera)         { dicSortedSections.Add(property, MediaSection.Camera); }
                else if (Media.GetMediaSection(mediaType, property) == MediaSection.AdvancedPhoto)  { dicSortedSections.Add(property, MediaSection.AdvancedPhoto); }
            }

            //Now we have it sorted. We need to filter based on the sections we have, and generate rows
            foreach (MediaSection currentSection in dicSortedSections.Values.Distinct())
            {
                if (dicSortedSections.Where(kvp => kvp.Value == currentSection).Count() > 0)
                {
                    GenerateHeaderAndRows(currentSection, dicSortedSections.Where(kvp => kvp.Value == currentSection).Select(kvp => kvp.Key).ToList());
                }
            }
        }

        private void UpdateSelectedFiles()
        {
            try
            {
                foreach (object oItem in ((MainWindow)Application.Current.MainWindow).dgInfoBox.Items)
                {
                    if (((Media)oItem).isChecked == true)
                    {
                        foreach (KeyValuePair<string, string> kvp in dicCurrentValues)
                        {
                            //Since we're dealing with a mutable type of objects.
                            //If the object has a valid property field then we can update the file
                            //To have the new properties that were used in the edit window.

                            switch (kvp.Key)
                            {
                                case "Title":
                                    ((Media)oItem).Title.Val = kvp.Value;
                                    break;
                                case "Comments":
                                    ((Media)oItem).Comments.Val = kvp.Value;
                                    break;
                                case "Rating":
                                    ((Media)oItem).Rating.Value = (Math.Round(double.Parse(kvp.Value)) == 0) ? null : (uint?)Math.Round(double.Parse(kvp.Value));
                                    break;
                                case "Tags":
                                    ((Media)oItem).Tags.Value = ParseArray(kvp.Value);
                                    ((Media)oItem).Tags.ArrayAsString = kvp.Value;
                                    break;
                                default:
                                    break;
                            }

                            if (oItem is Picture)
                            {
                                switch (kvp.Key)
                                {
                                    case "Subject":
                                        ((Picture)oItem).Subject.Val = kvp.Value;
                                        break;
                                    case "Author":
                                        ((Picture)oItem).Authors.Value = ParseArray(kvp.Value);
                                        ((Picture)oItem).Authors.ArrayAsString = kvp.Value;
                                        break;
                                    case "Program Name":
                                        ((Picture)oItem).ProgramName.Val = kvp.Value;
                                        break;
                                    case "Copyright":
                                        //((Picture)oItem).Copyright = kvp.Value;
                                        break;
                                    case "Date Acquired":
                                        ((Picture)oItem).DateAcquired.Value = ParseDate(kvp.Value);
                                        break;
                                    case "Date Taken":
                                        ((Picture)oItem).DateTaken.Value = ParseDate(kvp.Value);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else if (oItem is Audio)
                            {
                                switch (kvp.Key)
                                {
                                    case "Album":
                                        ((Audio)oItem).Album.Val = kvp.Value;
                                        break;
                                    case "Album Artist":
                                        ((Audio)oItem).AlbumArtist.Val = kvp.Value;
                                        break;
                                    case "BPM":
                                        ((Audio)oItem).BPM.Val = kvp.Value;
                                        break;
                                    case "Composer":
                                        ((Audio)oItem).Composer.Value = ParseArray(kvp.Value);
                                        ((Audio)oItem).Composer.ArrayAsString = kvp.Value;
                                        break;
                                    case "Subtitle":
                                        ((Audio)oItem).Subtitle.Val = kvp.Value;
                                        break;
                                    case "Publisher":
                                        ((Audio)oItem).Publisher.Val = kvp.Value;
                                        break;
                                    case "Genre":
                                        ((Audio)oItem).Genre.Value = ParseArray(kvp.Value);
                                        ((Audio)oItem).Genre.ArrayAsString = kvp.Value;
                                        break;
                                    case "Author URL":
                                        ((Audio)oItem).AuthorURL.Val = kvp.Value;
                                        break;
                                    case "Contributing Artists":
                                        ((Audio)oItem).ContributingArtists.Value = ParseArray(kvp.Value);
                                        ((Audio)oItem).ContributingArtists.ArrayAsString = kvp.Value;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else if (oItem is Video)
                            {
                                switch (kvp.Key)
                                {
                                    case "Media Created":
                                        ((Video)oItem).MediaCreated.Value = ParseDate(kvp.Value);
                                        break;
                                    case "Promotional URL":
                                        ((Video)oItem).PromotionalURL.Val = kvp.Value;
                                        break;
                                    case "Year":
                                        ((Video)oItem).Year.Value = (String.IsNullOrEmpty(kvp.Value)) ? (uint?)null : uint.Parse(kvp.Value);
                                        break;
                                    case "Directors":
                                        ((Video)oItem).Directors.Value = ParseArray(kvp.Value);
                                        ((Video)oItem).Directors.ArrayAsString = kvp.Value;
                                        break;
                                    case "Writers":
                                        ((Video)oItem).Writers.Value = ParseArray(kvp.Value);
                                        ((Video)oItem).Writers.ArrayAsString = kvp.Value;
                                        break;
                                    case "Producers":
                                        ((Video)oItem).Producers.Value = ParseArray(kvp.Value);
                                        ((Video)oItem).Producers.ArrayAsString = kvp.Value;
                                        break;
                                    case "Subtitle":
                                        ((Video)oItem).Subtitle.Val = kvp.Value;
                                        break;
                                    case "Publisher":
                                        ((Video)oItem).Publisher.Val = kvp.Value;
                                        break;
                                    case "Genre":
                                        ((Video)oItem).Genre.Value = ParseArray(kvp.Value);
                                        ((Video)oItem).Genre.ArrayAsString = kvp.Value;
                                        break;
                                    case "Author URL":
                                        ((Video)oItem).AuthorURL.Val = kvp.Value;
                                        break;
                                    case "Contributing Artists":
                                        ((Video)oItem).ContributingArtists.Value = ParseArray(kvp.Value);
                                        ((Video)oItem).ContributingArtists.ArrayAsString = kvp.Value;
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
                ErrorLog.WriteToLog(ex.Message, ex.StackTrace);
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        #region Helper Methods
        private string[] ParseArray(String itemArray)
        {
            string[] splitArray = (String.IsNullOrEmpty(itemArray)) ? Array.Empty<string>() : itemArray.Split(';');

            //Check the config to see if sorting is enabled.
            if (Settings.CanSort())
            {
                Array.Sort(splitArray, StringComparer.InvariantCulture);
            }

            return splitArray;
        }

        private DateTime? ParseDate(string dateString)
        {
            DateTime dateTimeOut;
            if (DateTime.TryParse(dateString, out dateTimeOut))
            {
                return dateTimeOut;
            }
            else
            {
                return null;
            }
        }
        
        #endregion

        #region Event Handlers
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> value = new List<string>();

                // Hit Save -> Get all the fields -> keep a tally of the "New" values.
                if (dicCurrentValues.Count > 0) { dicCurrentValues.Clear(); }

                //We start at one because we will ALWAYS have a section header
                for (int row = 1; row < grdEdit.RowDefinitions.Count; row++)
                {
                    int column = 0;
                    UIElement currentControl;
                    
                    //If it's a header, do the one below it instead
                    if (grdEdit.RowDefinitions[row].Name == "rowSectionHeader") { row++; }

                    column = (grdEdit.RowDefinitions[row].Name == "rowArrayValues") ? 2 : 1;                    
                    currentControl = grdEdit.Children.Cast<UIElement>().First(x => Grid.GetRow(x) == row && Grid.GetColumn(x) == column); //Default State.

                    if (currentControl is ComboBox) { value.Add((((ComboBox)currentControl).SelectedValue != null) ? ((ComboBox)currentControl).SelectedValue.ToString() : null); }
                    else if (currentControl is Slider) { value.Add(Math.Round(((Slider)currentControl).Value).ToString()); }
                    else if (currentControl is DatePicker) { value.Add(((DatePicker)currentControl).SelectedDate.HasValue ? ((DatePicker)currentControl).SelectedDate.Value.Date.ToShortDateString() : null); }
                    else if (currentControl is StackPanel)
                    {
                        if (((StackPanel)currentControl).Children.Count > 2 && ((StackPanel)currentControl).Name != "spHeader")
                        {
                            value.Add(((ListBox)((StackPanel)currentControl).Children[2]).HasItems ? String.Join(";", ((ListBox)((StackPanel)currentControl).Children[2]).Items.Cast<string>()) : String.Empty);
                        }
                    }
                    else { value.Add(((TextBox)currentControl).Text ); }
                }

                //Okay so, these headers are a real PITA so what we're doing is getting all the values first, since we can remove the headers there, then add them into the current value dictionary
                //I don't think anything should be affected programatically as the positions aren't dynamic once you hit the edit screen, the transfer process should be a 1:1 deal with no issues.
                if (propertiesSortedBySection.Count == value.Count)
                {
                    for (int index = 0; index < propertiesSortedBySection.Count; index++)
                    {
                        dicCurrentValues.Add(propertiesSortedBySection[index], value[index]);
                    }                    
                }

                lblUpdate.Visibility = Visibility.Visible;
                timer.Interval = TimeSpan.FromSeconds(3);
                timer.Tick += timer_Tick;
                timer.Start();
            }
            catch (Exception ex)
            {
                ErrorLog.WriteToLog(ex.Message, ex.StackTrace);
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
                ErrorLog.WriteToLog(ex.Message, ex.StackTrace);
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void sldEditor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UIElement lbl = grdEdit.Children.Cast<UIElement>().Where(x => Grid.GetRow(x) == Convert.ToInt32(((Slider)sender).Name.Split('_')[1]) && Grid.GetColumn(x) == 2).FirstOrDefault();

            ((Label)lbl).Content = String.Format("{0}/99", Math.Round(((Slider)sender).Value).ToString());
        }

        private void wndEdit_Closed(object sender, CancelEventArgs e)
        {
            MessageBoxResult result = MessageBoxMgr.CreateNewResult("You will lose any unsaved changes. Proceed?", "Warning", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                UpdateSelectedFiles();
            }
            else
            {
                e.Cancel = true;
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            lblUpdate.Visibility = Visibility.Hidden;
            timer.Stop();
        }
        #endregion
    }
}