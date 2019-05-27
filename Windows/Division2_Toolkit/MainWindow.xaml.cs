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
        Brush DivisionOrange = new SolidColorBrush(Color.FromArgb(100, 253, 107, 13));

        public MainWindow()
        {            
            InitializeComponent();            
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

        private void btnCalculate_Click(object sender, RoutedEventArgs e)
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

        private void btn_OpenGearSection(object sender, RoutedEventArgs e)
        {
            expMainMenu.IsExpanded = false;
            grdWeaponCalc.IsEnabled = false;
            grdWeaponCalc.Visibility = Visibility.Collapsed;
            grdGearSection.Visibility = Visibility.Visible;
            grdGearSection.IsEnabled = true;

            List<Gear> lstGear = Gear.ReadGearXLSX("gear.xlsx");

            ddlGear.ItemsSource = lstGear;
            ddlGear.DisplayMemberPath = "gearType";
            ddlGear.SelectedValue = "gearAttributes";
        }

        private void btn_OpenWeaponCalc(object sender, RoutedEventArgs e)
        {
            expMainMenu.IsExpanded = false;
            grdGearSection.IsEnabled = false;
            grdGearSection.Visibility = Visibility.Collapsed;
            grdWeaponCalc.Visibility = Visibility.Visible;
            grdWeaponCalc.IsEnabled = true;

            ddlFamily.ItemsSource = WeaponModel.GetWeaponFamilies(weaponList);
        }

        private void ddl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedDDLValue = (((ComboBox)sender).SelectedValue != null) ? ((ComboBox)sender).SelectedValue.ToString() : string.Empty;

            switch (((ComboBox)sender).Name)
            {
                case "ddlFamily":
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
                        btnCalculate.Visibility = Visibility.Hidden;
                        sldWeaponRoll.Visibility = Visibility.Hidden;
                    }
                    break;
                default:
                    break;
            }
        }

        private void ddl_GearSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            wpChkAttributes.Children.Clear();
            wpAttributeDesc.Children.Clear();

            foreach (GearAttribute ga in ((Gear)((ComboBox)sender).SelectedValue).gearAttributes)
            {
                checkboxFactory(ga);
            }
        }

        private void stackPanel_CollapseEvent(object sender, RoutedEventArgs e)
        {
            if (expMainMenu.IsExpanded) { expMainMenu.Header = "Menu "; }
            else
            {
                expMainMenu.Header = String.Empty;
                sectCalculators.IsExpanded = false;
                sectGear.IsExpanded = false;
                sectWeapons.IsExpanded = false;
            }
        }

        private void textbox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void generatedChk_Checked(object sender, RoutedEventArgs e)
        {
            GearAttribute checkedAttribute = (GearAttribute)((CheckBox)sender).DataContext;
            StackPanel spDescription = new StackPanel()
            {
                Name = String.Format("spDescription_{0}", checkedAttribute.AttributeName.Replace(" ", String.Empty).Replace("/", String.Empty)),
                Orientation = Orientation.Horizontal
            };

            Label lblDescription = new Label()
            {
                Name = String.Format("lblDescription_{0}", checkedAttribute.AttributeName.Replace(" ", String.Empty).Replace("/", String.Empty)),
                Content = String.Format("{0}: {1}-{2}", checkedAttribute.AttributeName, checkedAttribute.minRoll, (chkIsGearSet.IsChecked == true) ? checkedAttribute.setMaxRoll : checkedAttribute.maxRoll),
                Foreground = DivisionOrange,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom
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
            };
            
            Slider slider = new Slider()
            {
                Minimum = checkedAttribute.minRoll,
                Maximum = (chkIsGearSet.IsChecked == true) ? checkedAttribute.setMaxRoll : checkedAttribute.maxRoll,
                Height= 15,
                Width = 40,
                TickFrequency = checkedAttribute.minRoll,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(20,0,5,0)
            };

            slider.Style = FindResource("Horizontal_Slider") as Style;
            spDescription.Children.Add(lblDescription);
            spDescription.Children.Add(txtAttributeInput);
            spDescription.Children.Add(slider);

            if (((CheckBox)sender).IsChecked == true)
            {
                wpAttributeDesc.Children.Add(spDescription);
                //wpPercentiles.Children.Add(slider);
                btnCalculatePercentile_Gear.IsEnabled = true;
                btnCalculatePercentile_Gear.Visibility = Visibility.Visible;
            }
            else if (((CheckBox)sender).IsChecked == false)
            {
                for(int index = 0; index < wpAttributeDesc.Children.Count; index++)
                { 
                    if (((StackPanel)wpAttributeDesc.Children[index]).Name == spDescription.Name)
                    {
                        wpAttributeDesc.Children.RemoveAt(index);
                        //wpPercentiles.Children.RemoveAt(index);
                    }
                }

                if (wpAttributeDesc.Children.Count == 0)
                {
                    btnCalculatePercentile_Gear.IsEnabled = false;
                    btnCalculatePercentile_Gear.Visibility = Visibility.Collapsed;
                }
            }
        }

        #endregion

        #region Helper Methods

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static List<WeaponModel> weaponList = WeaponModel.ReadCSV("gearsheet.csv");

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

            generatedChk.Checked += generatedChk_Checked;
            generatedChk.Unchecked += generatedChk_Checked;
            wpChkAttributes.Children.Add(generatedChk);
        }


        #endregion
    }
}
