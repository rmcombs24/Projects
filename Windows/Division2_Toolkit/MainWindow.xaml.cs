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

        private void LoadValues(WeaponModel selected)
        {
            foreach (UIElement ele in spModeVal_col_1.Children)
            {
                ((Label)ele).DataContext = selected;
            }

            foreach (UIElement ele in spModelVal_col_3.Children)
            {
                ((Label)ele).DataContext = selected;
            }
        }

        #region Event Handlers

        private void btnDPSCalc_AddWeapon_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtDPSCalc_GunDmg.Text) && !String.IsNullOrEmpty(txtDPSCalc_MagSize.Text) && !String.IsNullOrEmpty(txtDPSCalc_Reload.Text) && !String.IsNullOrEmpty(txtDPSCalc_RPM.Text))
            {
                DPSCalcItem newItem = new DPSCalcItem();
                newItem.ModelName = ddlDPSCalc_Model.SelectedValue.ToString();
                newItem.Damage = Convert.ToInt32(txtDPSCalc_GunDmg.Text);
                newItem.MagSize = Convert.ToInt32(txtDPSCalc_MagSize.Text);
                newItem.RPM = Convert.ToInt32(txtDPSCalc_RPM.Text);
                newItem.ReloadSpeed = Convert.ToInt32(txtDPSCalc_Reload.Text);
                newItem.DPM = String.Empty;
                newItem.DPS = String.Empty;

                lvDPSCalc_CompareView.Items.Add(newItem);

                if (lvDPSCalc_CompareView.Items.Count == 1)
                {
                    btnDPSCalc_Calculate.IsEnabled = true;
                    btnDPSCalc_Clear.IsEnabled = true;
                    btnDPSCalc_Remove.IsEnabled = true;
                    btnDPSCalc_Add.IsEnabled = true;
                    lvDPSCalc_CompareView.Visibility = Visibility.Visible;

                    btnDPSCalc_Clear.Visibility = Visibility.Visible;
                    btnDPSCalc_Remove.Visibility = Visibility.Visible;
                    btnDPSCalc_Calculate.Visibility = Visibility.Visible;
                }
            }
            else
            {
                MessageBox.Show("Please an acceptable have value before adding to the compare.", "Error.", MessageBoxButton.OK);
            }
        }

        private void btnDPSCalc_CalculateDamage_Click(object sender, RoutedEventArgs e)
        {
            if (!lvDPSCalc_CompareView.HasItems)
            {
                MessageBox.Show("Please add at least one item to the view before calculating", "Error", MessageBoxButton.OK);
            }
            else
            {
                List<DPSCalcItem> lstCalculatedItems = new List<DPSCalcItem>();

                for (int lvIndex = 0; lvIndex < lvDPSCalc_CompareView.Items.Count; lvIndex++)
                {
                    DPSCalcItem currentItem = (DPSCalcItem)lvDPSCalc_CompareView.Items[lvIndex];
                    currentItem.DPM = String.Format("{0:n0}", Math.Round((60 / (currentItem.MagSize / (currentItem.RPM / 60) + (currentItem.ReloadSpeed / 1000)) * currentItem.Damage * currentItem.MagSize)));
                    currentItem.DPS = String.Format("{0:n0}", Math.Round((currentItem.Damage * currentItem.RPM / 60.0)));
                    lstCalculatedItems.Add(currentItem);
                }

                lvDPSCalc_CompareView.Items.Clear();

                for (int lstIndex = 0; lstIndex < lstCalculatedItems.Count; lstIndex++)
                {
                    //Add the updated structs to the list
                    //lvItems.ItemsSource = lstCalculatedItems; //We Could do this, but then it won't allow you to add through the add button
                    lvDPSCalc_CompareView.Items.Add(lstCalculatedItems[lstIndex]);
                }

                lvDPSCalc_CompareView.Items.Refresh();
                lvDPSCalc_CompareView.Items.Refresh();
            }
        }

        private void btnDPSCalc_ClearList_Click(object sender, RoutedEventArgs e)
        {
            if (lvDPSCalc_CompareView.HasItems)
            {
                lvDPSCalc_CompareView.Items.Clear();
                lvDPSCalc_CompareView.Items.Refresh();

                btnDPSCalc_Calculate.IsEnabled = false;
                btnDPSCalc_Clear.IsEnabled = false;
                btnDPSCalc_Remove.IsEnabled = false;
                btnDPSCalc_Add.IsEnabled = true;
                lvDPSCalc_CompareView.Visibility = Visibility.Collapsed;

                btnDPSCalc_Clear.Visibility = Visibility.Collapsed;
                btnDPSCalc_Remove.Visibility = Visibility.Collapsed;
                btnDPSCalc_Calculate.Visibility = Visibility.Collapsed;
            }
        }

        private void btnDPSCalc_RemoveWeapon_Click(object sender, RoutedEventArgs e)
        {
            if (lvDPSCalc_CompareView.HasItems)
            {
                if (lvDPSCalc_CompareView.SelectedItem != null)
                {
                    lvDPSCalc_CompareView.Items.RemoveAt(lvDPSCalc_CompareView.SelectedIndex);

                    if (!lvDPSCalc_CompareView.HasItems)
                    {
                        btnDPSCalc_Calculate.IsEnabled = false;
                        btnDPSCalc_Clear.IsEnabled = false;
                        btnDPSCalc_Remove.IsEnabled = false;
                        if (ddlDPSCalc_Model.SelectedValue == null) { btnDPSCalc_Add.IsEnabled = false; }

                        btnDPSCalc_Clear.Visibility = Visibility.Collapsed;
                        btnDPSCalc_Remove.Visibility = Visibility.Collapsed;
                        btnDPSCalc_Calculate.Visibility = Visibility.Collapsed;
                        lvDPSCalc_CompareView.Visibility = Visibility.Collapsed;
                    }
                }

                lvDPSCalc_CompareView.Items.Refresh();
            }
        }

        private void btnCalculatePercentile_Weapon_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // UIdmg/(1+(typedmg+addldmg)/100) base damage
                // (basedmg-min) / (max-min) roll percentage
                // (113300/(1+(17.5+12)/100)-72650)/(88794-72650)
                // (UIdmg/(1+(AddlDmg+TypeDmg)/100)-int[0])/(int[1]-int[0])
                // (UIdmg / (1 + (AddlDmg + TypeDmg) / 100) - intArray[0]) / (intArray[1] - intArray[0])

                double UIdmg = Convert.ToDouble(txtUIDmg.Text);
                double TypeDmg = Convert.ToDouble(txtTypeDmg.Text);
                double AddlDmg = Convert.ToDouble(txtAddDmg.Text);
                double basedmg = (UIdmg / (1 + (AddlDmg + TypeDmg) / 100));

                int[] intArray = Array.ConvertAll(lblDamageRangeVal.Content.ToString().Split('/'), el => Convert.ToInt32(el));
                double rollPercentage = (basedmg - intArray[0]) / (intArray[1] - intArray[0]);

                lblCalculateRoll.Content = String.Format("Top Damage for \n{0} ", ddlModel.SelectedValue.ToString());
                lblCalculateRoll.Visibility = Visibility.Visible;

                sldWeaponRoll.Visibility = Visibility.Visible;
                sldWeaponRoll.Value = Math.Round(rollPercentage * 100, 2);
                sldWeaponRoll.IsEnabled = false;

            }
            catch (FormatException ex)
            {
                MessageBox.Show("Please enter values before calculating", "Error", MessageBoxButton.OK);
            }
        }

        private void btnCalculatePercentile_Gear_Click(object sender, RoutedEventArgs e)
        {
            for (int wpIndex = 0; wpIndex < wpAttributeDesc.Children.Count; wpIndex++)
            { 
                if (wpAttributeDesc.Children[wpIndex] is StackPanel)
                {
                    List<string> lstValueString = new List<string>();
                    double min = 0, max = 0, userVal = 0;

                    foreach (UIElement uieChild in ((StackPanel) wpAttributeDesc.Children[wpIndex]).Children)
                    {
                        if (uieChild is Label)
                        {
                            lstValueString = ((Label)uieChild).Content.ToString().Split(':').ToList();
                            lstValueString = lstValueString[1].Split('-').ToList();
                            min = Convert.ToDouble(lstValueString[0]);
                            max = Convert.ToDouble(lstValueString[1]);
                        }
                        else if (uieChild is TextBox)
                        {
                            userVal = Convert.ToDouble(((TextBox)uieChild).Text);
                        }
                        else if (uieChild is StackPanel)
                        {
                            double rollPercentage = Math.Round((userVal - min) / (max - min) * 100, 2);

                            for (int spIndex = 0; spIndex < ((StackPanel)uieChild).Children.Count; spIndex++)
                            {
                                if (((StackPanel)uieChild).Children[spIndex] is Label)
                                {
                                    ((Label)((StackPanel)uieChild).Children[spIndex]).Content =String.Format("{0}%", rollPercentage.ToString());
                                }
                                else if (((StackPanel)uieChild).Children[spIndex] is ProgressBar)
                                {
                                    ((ProgressBar)((StackPanel)uieChild).Children[spIndex]).Value = rollPercentage;
                                }
                            }

                            ((StackPanel)uieChild).Visibility = Visibility.Visible;
                        }
                    }
                }
            }
        }

        private void btnClearPercentile_Gear_Click(object sender, RoutedEventArgs e)
        {
            wpAttributeDesc.Children.Clear();

            foreach (UIElement uie in wpChkAttributes.Children)
            {
                if (uie is CheckBox)
                {
                    ((CheckBox)uie).IsChecked = false;
                }
            }
        }

        private void btnOpenSection_GearPercentile_Click(object sender, RoutedEventArgs e)
        {
            expMainMenu.IsExpanded = false;
            grdSectionWeapons_Percentile.IsEnabled = false;
            grdSectionWeapons_Percentile.Visibility = Visibility.Collapsed;
            grdSectionWeapons_DPS.Visibility = Visibility.Collapsed;
            grdSectionWeapons_DPS.IsEnabled = false;

            grdSectionGear.Visibility = Visibility.Visible;
            grdSectionGear.IsEnabled = true;
            chkIsGearSet.IsChecked = false;
            
            List<Gear> lstGear = Gear.ReadGearXLSX("gear.xlsx");

            ddlGear.ItemsSource = lstGear;
            ddlGear.DisplayMemberPath = "gearType";
            ddlGear.SelectedValue = "gearAttributes";
        }

        private void btnOpenSection_WeaponDPSCalculator_Click(object sender, RoutedEventArgs e)
        {
            expMainMenu.IsExpanded = false;
            grdSectionGear.IsEnabled = false;
            grdSectionGear.Visibility = Visibility.Collapsed;
            grdSectionWeapons_Percentile.Visibility = Visibility.Collapsed;
            grdSectionWeapons_Percentile.IsEnabled = false;

            grdSectionWeapons.Visibility = Visibility.Visible;
            grdSectionWeapons_DPS.IsEnabled = true;
            grdSectionWeapons_DPS.Visibility = Visibility.Visible;
            ddlDPSCalc_Family.ItemsSource = WeaponModel.GetWeaponFamilies(weaponList);
        }

        private void btnOpenSection_WeaponPercentile_Click(object sender, RoutedEventArgs e)
        {
            expMainMenu.IsExpanded = false;
            grdSectionGear.IsEnabled = false;
            grdSectionGear.Visibility = Visibility.Collapsed;
            grdSectionWeapons_DPS.IsEnabled = false;
            grdSectionWeapons_DPS.Visibility = Visibility.Collapsed;

            grdSectionWeapons.Visibility = Visibility.Visible;
            grdSectionWeapons_Percentile.Visibility = Visibility.Visible;
            grdSectionWeapons_Percentile.IsEnabled = true;
            ddlFamily.ItemsSource = WeaponModel.GetWeaponFamilies(weaponList);
        }

        private void ddlWeaponFamilyMakeModel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedDDLValue = (((ComboBox)sender).SelectedValue != null) ? ((ComboBox)sender).SelectedValue.ToString() : string.Empty;

            //I know I could have simplified this with a DataTemplate or using the same DDLS for everything but cest la vie
            //Maybe on a saturday I'll refactor.
            switch (((ComboBox)sender).Name)
            {
                case "ddlFamily" :
                    ddlMake.ItemsSource = WeaponModel.GetWeaponMakesByFamily(weaponList, selectedDDLValue);
                    ddlMake.IsEnabled = true;

                    spModelInfo_col_0.Visibility = Visibility.Hidden;
                    spModelInfo_col_2.Visibility = Visibility.Hidden;
                    spModelVal_col_3.Visibility = Visibility.Hidden;
                    spModeVal_col_1.Visibility = Visibility.Hidden;
                    btnCalculate.Visibility = Visibility.Hidden;
                    lblCalculateRoll.Visibility = Visibility.Hidden;
                    lblCalculateRollVal.Visibility = Visibility.Hidden;
                    sldWeaponRoll.Visibility = Visibility.Hidden;

                    break;
                case "ddlMake":
                    ddlModel.ItemsSource = (String.IsNullOrEmpty(selectedDDLValue)) ? null : WeaponModel.GetWeaponModelsByMake(weaponList, ddlFamily.SelectedValue.ToString(), selectedDDLValue);
                    ddlModel.IsEnabled = (String.IsNullOrEmpty(selectedDDLValue)) ? false : true;

                    spModelInfo_col_0.Visibility = Visibility.Hidden;
                    spModelInfo_col_2.Visibility = Visibility.Hidden;
                    spModelVal_col_3.Visibility = Visibility.Hidden;
                    spModeVal_col_1.Visibility = Visibility.Hidden;
                    btnCalculate.Visibility = Visibility.Hidden;
                    lblCalculateRoll.Visibility = Visibility.Hidden;
                    lblCalculateRollVal.Visibility = Visibility.Hidden;
                    sldWeaponRoll.Visibility = Visibility.Hidden;

                    break;
                case "ddlModel":

                    if (!String.IsNullOrEmpty(selectedDDLValue))
                    {
                        LoadValues((WeaponModel)weaponList.Where(x => x.Model == selectedDDLValue).ToArray()[0]);
                        spModelInfo_col_0.Visibility = Visibility.Visible;
                        spModelInfo_col_2.Visibility = Visibility.Visible;
                        spModelVal_col_3.Visibility = Visibility.Visible;
                        spModeVal_col_1.Visibility = Visibility.Visible;
                        btnCalculate.Visibility = Visibility.Visible;
                        sldWeaponRoll.Visibility = Visibility.Hidden;
                    }

                    break;
                case "ddlDPSCalc_Family":
                    ddlDPSCalc_Make.ItemsSource = WeaponModel.GetWeaponMakesByFamily(weaponList, selectedDDLValue);
                    ddlDPSCalc_Make.IsEnabled = true;
                    if (!lvDPSCalc_CompareView.HasItems)
                    {
                        wpDPSCalc_Controls.Visibility = Visibility.Collapsed;
                    }
                    btnDPSCalc_Add.IsEnabled = false;

                    chkDPSCalc_CustomMagSize.IsChecked = false;
                    chkDPSCalc_CustomReload.IsChecked = false;
                    chkDPSCalc_CustomRPM.IsChecked = false;
                    chkDPSCalc_CustomMagSize.IsEnabled = false;
                    chkDPSCalc_CustomReload.IsEnabled = false;
                    chkDPSCalc_CustomRPM.IsEnabled = false;
                    txtDPSCalc_GunDmg.IsEnabled = false;
                    txtDPSCalc_MagSize.IsEnabled = false;
                    txtDPSCalc_Reload.IsEnabled = false;
                    txtDPSCalc_RPM.IsEnabled = false;

                    break;
                case "ddlDPSCalc_Make":
                    ddlDPSCalc_Model.ItemsSource = (String.IsNullOrEmpty(selectedDDLValue)) ? null : WeaponModel.GetWeaponModelsByMake(weaponList, ddlDPSCalc_Family.SelectedValue.ToString(), selectedDDLValue);
                    ddlDPSCalc_Model.IsEnabled = (String.IsNullOrEmpty(selectedDDLValue)) ? false : true;
                    if (!lvDPSCalc_CompareView.HasItems)
                    {
                        wpDPSCalc_Controls.Visibility = Visibility.Collapsed;
                    }
                    btnDPSCalc_Add.IsEnabled = false;

                    chkDPSCalc_CustomMagSize.IsChecked = false;
                    chkDPSCalc_CustomReload.IsChecked = false;
                    chkDPSCalc_CustomRPM.IsChecked = false;
                    chkDPSCalc_CustomMagSize.IsEnabled = false;
                    chkDPSCalc_CustomReload.IsEnabled = false;
                    chkDPSCalc_CustomRPM.IsEnabled = false;
                    txtDPSCalc_GunDmg.IsEnabled = false;
                    txtDPSCalc_MagSize.IsEnabled = false;
                    txtDPSCalc_Reload.IsEnabled = false;
                    txtDPSCalc_RPM.IsEnabled = false;

                    break;
                case "ddlDPSCalc_Model":
                    
                    if (!String.IsNullOrEmpty(selectedDDLValue))
                    {
                        WeaponModel selectedModel = (WeaponModel)weaponList.Where(x => x.Model == selectedDDLValue).ToArray()[0];
                        txtDPSCalc_MagSize.Text = selectedModel.MagSize.ToString();
                        txtDPSCalc_RPM.Text = selectedModel.RPM.ToString();
                        txtDPSCalc_Reload.Text = selectedModel.ReloadSpeed.ToString();
                        btnDPSCalc_Add.IsEnabled = true;

                        chkDPSCalc_CustomMagSize.IsEnabled = true;
                        chkDPSCalc_CustomReload.IsEnabled = true;
                        chkDPSCalc_CustomRPM.IsEnabled = true;
                        txtDPSCalc_GunDmg.IsEnabled = true;
                        txtDPSCalc_MagSize.IsEnabled = false;
                        txtDPSCalc_Reload.IsEnabled = false;
                        txtDPSCalc_RPM.IsEnabled = false;

                        if (!lvDPSCalc_CompareView.HasItems)
                        {
                            wpDPSCalc_Controls.Visibility = Visibility.Visible;
                            btnDPSCalc_Clear.Visibility = Visibility.Collapsed;
                            btnDPSCalc_Remove.Visibility = Visibility.Collapsed;
                            btnDPSCalc_Calculate.Visibility = Visibility.Collapsed;
                        }
                    }

                    break;
                default:
                    break;
            }
        }

        private void ddlGearPercentile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            wpChkAttributes.Children.Clear();
            wpAttributeDesc.Children.Clear();
            btnCalculatePercentile_Gear.Visibility = Visibility.Collapsed;
            btnClearPercentile_Gear.Visibility = Visibility.Collapsed;

            if (((ComboBox)sender).SelectedValue != null)
            {
                foreach (GearAttribute ga in ((Gear)((ComboBox)sender).SelectedValue).gearAttributes)
                {
                    checkboxFactory(ga);
                }
            }
        }

        private void stackPanel_CollapseEvent(object sender, RoutedEventArgs e)
        {
            if (expMainMenu.IsExpanded) { expMainMenu.Header = "Main Menu"; expMainMenu.FontFamily = new FontFamily("Montserrat Medium"); }
            else
            {
                expMainMenu.Header = String.Empty;
                sectResources.IsExpanded = false;
                sectGear.IsExpanded = false;
                sectWeapons.IsExpanded = false;
            }
        }

        private void textbox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void chkGenerated_Checked(object sender, RoutedEventArgs e)
        {
            GearAttribute checkedAttribute = (GearAttribute)((CheckBox)sender).DataContext;
            StackPanel spDescription = new StackPanel()
            {
                Name = String.Format("spDescription_{0}", checkedAttribute.AttributeName.Replace(" ", String.Empty).Replace("/", String.Empty)),
                Orientation = Orientation.Horizontal
            };

            Label lblDescription = new Label()
            {//Montserrat SemiBold
                Name = String.Format("lblDescription_{0}", checkedAttribute.AttributeName.Replace(" ", String.Empty).Replace("/", String.Empty)),
                Content = String.Format("{0}: {1}-{2}", checkedAttribute.AttributeName, checkedAttribute.minRoll, (chkIsGearSet.IsChecked == true) ? checkedAttribute.setMaxRoll : checkedAttribute.maxRoll),
                Foreground = DivisionOrange,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                FontFamily = new FontFamily("Montserrat SemiBold")
        };

            TextBox txtAttributeInput = new TextBox()
            {
                Name = String.Format("txtAttributeInput_{0}", checkedAttribute.AttributeName.Replace(" ", String.Empty).Replace("/", String.Empty)),
                Text = String.Empty,
                Height = 18,
                Width = 75,
                Margin = new Thickness(10,0,5,0),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                FontFamily = new FontFamily("Montserrat")
    };

            txtAttributeInput.PreviewTextInput += textbox_PreviewTextInput;

            StackPanel spPercentile = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                Height = 30,
                Width = 75,
                Margin = new Thickness(0,0,0,5),
                VerticalAlignment = VerticalAlignment.Center,
                IsEnabled = false,
                Visibility = Visibility.Collapsed
            };

            Label lblPercentileVal = new Label()
            {
                Content = "0%",
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                FontSize = 10,
                FontFamily = new FontFamily("Montserrat")
            };

            ProgressBar pbPercentile = new ProgressBar()
            {
                Height = 5,
                Width = 75,
                Background = Brushes.Transparent,
                Foreground = Brushes.White,
                Margin = new Thickness(0,-2,0,5)
            };

            spPercentile.Children.Add(lblPercentileVal);
            spPercentile.Children.Add(pbPercentile);

            spDescription.Children.Add(lblDescription);
            spDescription.Children.Add(txtAttributeInput);
            spDescription.Children.Add(spPercentile);

            if (((CheckBox)sender).IsChecked == true)
            {
                wpAttributeDesc.Children.Add(spDescription);
                btnCalculatePercentile_Gear.IsEnabled = true;
                btnCalculatePercentile_Gear.Visibility = Visibility.Visible;
                btnClearPercentile_Gear.IsEnabled = true;
                btnClearPercentile_Gear.Visibility = Visibility.Visible;
            }
            else if (((CheckBox)sender).IsChecked == false)
            {
                for(int index = 0; index < wpAttributeDesc.Children.Count; index++)
                { 
                    if (((StackPanel)wpAttributeDesc.Children[index]).Name == spDescription.Name)
                    {
                        wpAttributeDesc.Children.RemoveAt(index);
                    }
                }

                if (wpAttributeDesc.Children.Count == 0)
                {
                    btnClearPercentile_Gear.IsEnabled = false;
                    btnCalculatePercentile_Gear.IsEnabled = false;
                    btnClearPercentile_Gear.Visibility = Visibility.Collapsed;
                    btnCalculatePercentile_Gear.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void chkDPSCalc_CustomValue(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked == true)
            {
                if (((CheckBox)sender).Name == "chkDPSCalc_CustomMagSize")
                {
                    txtDPSCalc_MagSize.IsEnabled = true;
                }
                else if (((CheckBox)sender).Name == "chkDPSCalc_CustomReload")
                {
                    txtDPSCalc_Reload.IsEnabled = true;
                }
                else if (((CheckBox)sender).Name == "chkDPSCalc_CustomRPM")
                {
                    txtDPSCalc_RPM.IsEnabled = true;
                }
            }
            else
            {
                if (((CheckBox)sender).Name == "chkDPSCalc_CustomMagSize")
                {
                    txtDPSCalc_MagSize.IsEnabled = false;
                }
                else if (((CheckBox)sender).Name == "chkDPSCalc_CustomReload")
                {
                    txtDPSCalc_Reload.IsEnabled = false;
                }
                else if (((CheckBox)sender).Name == "chkDPSCalc_CustomRPM")
                {
                    txtDPSCalc_RPM.IsEnabled = false;
                }
            }
        }

        #endregion

        #region Helper Methods

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static List<WeaponModel> weaponList = WeaponModel.ReadCSV("weapons.csv");

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void checkboxFactory(GearAttribute currentAttribute)
        {
            CheckBox generatedChk = new CheckBox()
            {
                Foreground = DivisionOrange,
                Name = String.Format("chkAttribute_{0}", currentAttribute.AttributeName.Replace(" ", String.Empty)).Replace("/", ""),
                Content = currentAttribute.AttributeName,
                DataContext = currentAttribute
            };

            generatedChk.Checked += chkGenerated_Checked;
            generatedChk.Unchecked += chkGenerated_Checked;
            wpChkAttributes.Children.Add(generatedChk);
        }

        Brush DivisionOrange = new SolidColorBrush(Color.FromArgb(100, 253, 107, 13));
        struct DPSCalcItem
        {
            public string ModelName { get; set; }
            public int Damage { get; set; }
            public int MagSize { get; set; }
            public int RPM { get; set; }
            public string DPM { get; set; }
            public string DPS { get; set; }
            public double ReloadSpeed { get; set; }

        }

        #endregion

        private void btnOpenSection_ResourcesExotics_Click(object sender, RoutedEventArgs e)
        {
            grdSectionGear.IsEnabled = false;
            grdSectionGear.Visibility = Visibility.Collapsed;

            grdSectionWeapons.IsEnabled = false;
            grdSectionWeapons.Visibility = Visibility.Collapsed;

            grdSectionResources.IsEnabled = true;
            grdSectionResources.Visibility = Visibility.Visible;
            grdSectionResources_Exotics.IsEnabled = true;
            grdSectionResources_Exotics.Visibility = Visibility.Visible;

            //Get weapon exotics from weaponlist
            //Load exotic talents
            ddlResourcesExotics_ExoticList.ItemsSource = Exotic.ReadExoticXLSX("ExoticTalents.xlsx");
            ddlResourcesExotics_ExoticList.DisplayMemberPath = "Name";
        }
    }
}
