using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Division2_Toolkit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Brush DivisionOrange = new SolidColorBrush(Color.FromArgb(100, 253, 107, 13));

        static List<WeaponModel> weaponList = WeaponModel.ReadCSV("gearsheet.csv");

        public MainWindow()
        {            
            InitializeComponent();
            ddlFamily.ItemsSource = WeaponModel.GetWeaponFamilies(weaponList);
        }

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
                lblCalculateRollVal.Content = String.Format("{0}\nRoll percentage: {1}%", Math.Round(basedmg), rollPercentage * 100);
                lblCalculateRoll.Visibility = Visibility.Visible;
                lblCalculateRollVal.Visibility = Visibility.Visible;

                    if (rollPercentage >= .90)
                    gradient.Color = Brushes.Green.Color;
                    else if (rollPercentage >= .80)
                    gradient.Color = Brushes.LawnGreen.Color;
                    else if (rollPercentage >= .70)
                    gradient.Color = Brushes.GreenYellow.Color;
                    else if (rollPercentage >= .60)
                    gradient.Color = Brushes.YellowGreen.Color;
                    else  if (rollPercentage >= .50)
                    gradient.Color = Brushes.Yellow.Color;
                    else if (rollPercentage >= .40)
                    gradient.Color = Brushes.OrangeRed.Color;
                    else if (rollPercentage <= .20)
                    gradient.Color = Brushes.Red.Color;
                    else
                    gradient.Color = Color.FromRgb(253, 107, 13);
            }
            catch(FormatException ex)
            {
                MessageBox.Show("Please enter values before calculating", "Error", MessageBoxButton.OK);
            }
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

                    break;
                case "ddlMake":
                    ddlModel.ItemsSource =  (String.IsNullOrEmpty(selectedDDLValue)) ?  null : WeaponModel.GetWeaponModelsByMake(weaponList, ddlFamily.SelectedValue.ToString() ,selectedDDLValue);
                    ddlModel.IsEnabled = (String.IsNullOrEmpty(selectedDDLValue)) ? false : true;

                    spModelInfo_col_0.Visibility = Visibility.Hidden;
                    spModelInfo_col_2.Visibility = Visibility.Hidden;
                    spModelVal_col_3.Visibility = Visibility.Hidden;
                    spModeVal_col_1.Visibility = Visibility.Hidden;
                    btnCalculate.Visibility = Visibility.Hidden;
                    lblCalculateRoll.Visibility = Visibility.Hidden;
                    lblCalculateRollVal.Visibility = Visibility.Hidden;
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
                        rectRollBar.Visibility = Visibility.Visible;
                        gradient.Color = Color.FromRgb(253, 107, 13);
                    }
                    break;
                default:
                    break;
            }
        }

        private void PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
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

        private void StackPanel_CollapseEvent(object sender, RoutedEventArgs e)
        {
            if (expMainMenu.IsExpanded) { expMainMenu.Header = "Main Menu"; }
            else { expMainMenu.Header = String.Empty; }
        }

        private void btn_OpenGearSection(object sender, RoutedEventArgs e)
        {
            expMainMenu.IsExpanded = false;
            grdWeaponCalc.IsEnabled = false;
            grdWeaponCalc.Visibility = Visibility.Hidden;
            
            List<Gear> lstGear = Gear.ReadGearXLSX("gear.xlsx");
            
            ddlGear.ItemsSource = lstGear;
            ddlGear.DisplayMemberPath = "gearType";
            ddlGear.SelectedValue = "gearAttributes";

            //string selectedGear
        }

        private void ddl_GearSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            wpAttributes.Children.Clear();

            foreach (GearAttribute ga in ((Gear)((ComboBox)sender).SelectedValue).gearAttributes)
            {
                checkboxFactory(ga);
            }
        }

        private void checkboxFactory(GearAttribute currentAttribute)
        {
            CheckBox generatedChk = new CheckBox()
            {
                Foreground = DivisionOrange,
                Name = String.Format("chkAttribute_{0}",  currentAttribute.AttributeName.Replace(" ", String.Empty)).Replace("/", ""),
                Content = currentAttribute.AttributeName,
                DataContext = currentAttribute
            };
            
            generatedChk.Checked += generatedChk_Checked;
            wpAttributes.Children.Add(generatedChk);
        }

        private void generatedChk_Checked(object sender, RoutedEventArgs e)
        {
            //((CheckBox)sender).DataContext;
        }
    }
}
