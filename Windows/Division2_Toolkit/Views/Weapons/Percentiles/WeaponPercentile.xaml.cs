using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Division2Toolkit.Views
{
    /// <summary>
    /// Interaction logic for WeaponPercentile.xaml
    /// </summary>
    public partial class WeaponPercentile : UserControl
    {
        Brush DivisionOrange = new SolidColorBrush(Color.FromArgb(100, 253, 107, 13));

        private void textbox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        public static Dictionary<string, double> dicTypeDamage = new Dictionary<string, double>
        {
            ["Assault Rifle"] = 0,
            ["Pistol"] = 0,
            ["Rifle"] = 0,
            ["Shotgun"] = 0,
            ["SMG"] = 0,
            ["LMG"] = 0,
            ["MMR"] = 0
        };

        private static List<WeaponModel> weaponList = WeaponModel.ReadCSV("weapons.csv");


        public WeaponPercentile()
        {
            InitializeComponent();

            ddlFamily.ItemsSource = WeaponModel.GetWeaponFamilies(weaponList);
            CheckBox chkTalents = new CheckBox()
            {
                Name = "chkTalents",
                Content = "Weapon Talents",
                FlowDirection = FlowDirection.RightToLeft,
                Foreground = DivisionOrange
            };

            grpTalents.Header = chkTalents;

            chkTalents.Checked += chkTalents_Checking;
            chkTalents.Unchecked += chkTalents_Checking;
        }

        private void chkTalents_Checking(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked == true) { spTglTalents.IsEnabled = true; }
            else { spTglTalents.IsEnabled = false; }
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


        private void ddlWeaponFamilyMakeModel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedDDLValue = (((ComboBox)sender).SelectedValue != null) ? ((ComboBox)sender).SelectedValue.ToString() : string.Empty;

            //I know I could have simplified this with a DataTemplate or using the same DDLS for everything but cest la vie
            //Maybe on a saturday I'll refactor.
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
                    spWeaponRoll.Visibility = Visibility.Hidden;

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
                    spWeaponRoll.Visibility = Visibility.Hidden;

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
                        spWeaponRoll.Visibility = Visibility.Hidden;
                    }

                    break;
                default:
                    break;
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
                double TypeDmg = getTypeDamage(ddlFamily.SelectedValue.ToString());
                double AddlDmg = Convert.ToDouble(txtAddDmg.Text) + getTalentDamage();
                double basedmg = (UIdmg / (1 + (AddlDmg + TypeDmg) / 100));

                int[] intArray = Array.ConvertAll(lblDamageRangeVal.Content.ToString().Split('/'), el => Convert.ToInt32(el));
                double rollPercentage = (basedmg - intArray[0]) / (intArray[1] - intArray[0]);

                lblCalculateRoll.Content = String.Format("Top Damage for \n{0} ", ddlModel.SelectedValue.ToString());
                lblCalculateRoll.Visibility = Visibility.Visible;

                spWeaponRoll.Visibility = Visibility.Visible;
                prgWeaponRoll.Value = Math.Round(rollPercentage * 100, 2);
                lblWeaponRoll.Content = String.Format("{0}%", prgWeaponRoll.Value);
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Please enter values before calculating", "Error", MessageBoxButton.OK);
            }
        }

        private double getTypeDamage(string WeaponFamily)
        {
            double TypeDamage;

            switch (WeaponFamily)
            {
                case "AR":
                    TypeDamage = dicTypeDamage["Assault Rifle"];
                    break;
                case "Pistol":
                    TypeDamage = dicTypeDamage["Pistol"];
                    break;
                case "Shotgun":
                    TypeDamage = dicTypeDamage["Shotgun"];
                    break;
                case "Rifle":
                    TypeDamage = dicTypeDamage["Rifle"];
                    break;
                case "LMG":
                    TypeDamage = dicTypeDamage["LMG"];
                    break;
                case "SMG":
                    TypeDamage = dicTypeDamage["SMG"];
                    break;
                case "MMR":
                    TypeDamage = dicTypeDamage["MMR"];
                    break;
                default:
                    TypeDamage = 0;
                    break;
            }

            return TypeDamage;
        }

        private double getTalentDamage()
        {
            double allWeaponDamage = 0;
            if (spTglTalents.IsEnabled)
            {
                if (tglMeasured.isToggled && tglMeasured.IsEnabled) { allWeaponDamage = 30; }
                else if (!tglMeasured.isToggled && tglMeasured.IsEnabled) { allWeaponDamage = -15; }

                if (tglOptimist.isToggled && tglOptimist.IsEnabled) { allWeaponDamage = 30; }
                else if (tglOptimist.isToggled && tglOptimist.IsEnabled) { allWeaponDamage = 0; }

                if (tglUnhinged.isToggled && tglUnhinged.IsEnabled) { allWeaponDamage = 20; }
                else if (tglUnhinged.isToggled && tglUnhinged.IsEnabled) { allWeaponDamage = 0; }
            }
            else { allWeaponDamage = 0; }

            return allWeaponDamage;
        }

        private void btnOpenWeaponTypes_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Weapons.Percentiles.TypeDamageWindow();

            window.ShowDialog();
        }

        private void tglMeasured_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (tglMeasured.isToggled)
            {
                tglUnhinged.IsEnabled = false;
                tglUnhinged.isToggled = false;

                tglOptimist.IsEnabled = false;
                tglOptimist.isToggled = false;
            }
            else if (!tglMeasured.isToggled)
            {
                tglUnhinged.IsEnabled = true;
                tglOptimist.IsEnabled = true;
            }
        }

        private void tglOptimist_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (tglOptimist.isToggled)
            {
                tglMeasured.IsEnabled = false;
                tglMeasured.isToggled = false;

                tglUnhinged.IsEnabled = false;
                tglUnhinged.isToggled = false;
            }
            else if (!tglOptimist.isToggled)
            {
                tglMeasured.IsEnabled = true;
                tglUnhinged.IsEnabled = true;
            }
        }

        private void tglUnhinged_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (tglUnhinged.isToggled)
            {
                tglMeasured.IsEnabled = false;
                tglMeasured.isToggled = false;

                tglOptimist.IsEnabled = false;
                tglOptimist.isToggled = false;
            }
            else if (!tglUnhinged.isToggled)
            {
                tglMeasured.IsEnabled = true;
                tglOptimist.IsEnabled = true;
            }
        }
    }
}
