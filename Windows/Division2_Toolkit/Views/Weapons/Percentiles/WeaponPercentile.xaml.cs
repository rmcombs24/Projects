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
        public WeaponPercentile()
        {
            InitializeComponent();

            ddlFamily.ItemsSource = WeaponModel.GetWeaponFamilies(weaponList);
        }

        Brush DivisionOrange = new SolidColorBrush(Color.FromArgb(100, 253, 107, 13));

        private Dictionary<string, double> dicTypeDamage = new Dictionary<string, double>
        {
            ["Assault Rifle"] = 0,
            ["Pistol"] = 0,
            ["Rifle"] = 0,
            ["Shotgun"] = 0,
            ["SMG"] = 0,
            ["LMG"] = 0,
            ["MMR"] = 0
        };

        private static List<WeaponModel> weaponList = WeaponModel.GetWeaponsList();

        public static WeaponModel selectedWeapon;

        private void chkTalents_Checking(object sender, RoutedEventArgs e)
        {
            CheckBox chkCurrent = (CheckBox)sender;

            if (chkCurrent.Name == "chkMeasured" && chkMeasured.IsChecked == true)
            {
                tglMeasured.IsEnabled = true;

                wrpTalentOptimist.IsEnabled = false;
                wrpTalentUnhinged.IsEnabled = false;
            }
            else if (chkCurrent.Name == "chkMeasured" && chkMeasured.IsChecked == false)
            {
                tglMeasured.IsEnabled = false;
                tglMeasured.isToggled = false;

                wrpTalentOptimist.IsEnabled = true;
                tglOptimist.IsEnabled = false;

                wrpTalentUnhinged.IsEnabled = true;
                tglUnhinged.IsEnabled = false;
            }

            if (chkCurrent.Name == "chkOptimist" && chkOptimist.IsChecked == true)
            {
                tglOptimist.IsEnabled = true;
                wrpTalentMeasured.IsEnabled = false;
                wrpTalentUnhinged.IsEnabled = false;
            }
            else if (chkCurrent.Name == "chkOptimist" && chkOptimist.IsChecked == false)
            {
                tglOptimist.IsEnabled = false;
                tglOptimist.isToggled = false;

                wrpTalentMeasured.IsEnabled = true;
                tglMeasured.IsEnabled = false;

                wrpTalentUnhinged.IsEnabled = true;
                tglUnhinged.IsEnabled = false;
            }

            if (chkCurrent.Name == "chkUnhinged" && chkUnhinged.IsChecked == true)
            {
                tglUnhinged.IsEnabled = true;

                wrpTalentOptimist.IsEnabled = false;
                wrpTalentMeasured.IsEnabled = false;
            }
            else if (chkCurrent.Name == "chkUnhinged" && chkUnhinged.IsChecked == false)
            {
                tglUnhinged.IsEnabled = false;
                tglUnhinged.isToggled = false;

                wrpTalentOptimist.IsEnabled = true;
                tglOptimist.IsEnabled = false;

                wrpTalentMeasured.IsEnabled = true;
                tglMeasured.IsEnabled = false;
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

                    btnCalculate.Visibility = Visibility.Hidden;
                    lblCalculateRoll.Visibility = Visibility.Hidden;
                    spWeaponRoll.Visibility = Visibility.Hidden;
                    lblUpdatedRng.Visibility = Visibility.Hidden;

                    break;
                case "ddlMake":
                    ddlModel.ItemsSource = (String.IsNullOrEmpty(selectedDDLValue)) ? null : WeaponModel.GetWeaponModelsByMake(weaponList, ddlFamily.SelectedValue.ToString(), selectedDDLValue);
                    ddlModel.IsEnabled = (String.IsNullOrEmpty(selectedDDLValue)) ? false : true;

                    btnCalculate.Visibility = Visibility.Hidden;
                    lblCalculateRoll.Visibility = Visibility.Hidden;
                    spWeaponRoll.Visibility = Visibility.Hidden;
                    lblUpdatedRng.Visibility = Visibility.Hidden;
                    break;
                case "ddlModel":

                    if (!String.IsNullOrEmpty(selectedDDLValue))
                    {
                        selectedWeapon = WeaponModel.GetWeaponByName(selectedDDLValue);
                        btnCalculate.Visibility = Visibility.Visible;
                        btnWeaponInfo.Visibility = Visibility.Visible;
                    }

                    spWeaponRoll.Visibility = Visibility.Hidden;
                    lblUpdatedRng.Visibility = Visibility.Hidden;
                    lblCalculateRoll.Visibility = Visibility.Hidden;

                    break;
                default:
                    break;
            }
        }        
        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            string typeDmg = getTypeDamage(ddlFamily.SelectedValue.ToString()).ToString();
            double AddlDmg = String.IsNullOrEmpty(txtAddDmg.Text) ? 0 : Convert.ToDouble(txtAddDmg.Text);
            double bonusDmg = 1 + ((Convert.ToDouble(typeDmg) + AddlDmg)/100);

            PercentileGridItem newItem = new PercentileGridItem()
            {
                ModelName = ddlModel.SelectedValue.ToString(),
                Percentage = lblWeaponRoll.Content.ToString(),
                UIDamage = String.IsNullOrEmpty(txtUIDmg.Text) ? "0" : txtUIDmg.Text,
                AllWeaponDamage = String.IsNullOrEmpty(txtAddDmg.Text) ? "0%" : txtAddDmg.Text + "%",
                TypeDamage = String.IsNullOrEmpty(typeDmg) ? "0%" : typeDmg + "%",
                dmgRange = String.Format("{0}/{1}", Math.Round(selectedWeapon.DamageRange[0] * ((bonusDmg < 0) ? 1 : bonusDmg), 0),Math.Round(selectedWeapon.DamageRange[1] * ((bonusDmg < 0) ? 1 : bonusDmg), 0))
            };

            lvPercentileComp.Items.Add(newItem);

            if (lvPercentileComp.Items.Count == 1)
            {
                btnCalculate.IsEnabled = true;
                btnRemove.IsEnabled = true;
                btnAdd.IsEnabled = true;

                lvPercentileComp.Visibility = Visibility.Visible;
                btnRemove.Visibility = Visibility.Visible;
                btnCalculate.Visibility = Visibility.Visible;
            }
        }
        private void btnCalculatePercentile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // UIdmg/(1+(typedmg+addldmg)/100) base damage
                // (basedmg-min) / (max-min) roll percentage
                // (113300/(1+(17.5+12)/100)-72650)/(88794-72650)
                // (UIdmg/(1+(AddlDmg+TypeDmg)/100)-int[0])/(int[1]-int[0])
                // (UIdmg / (1 + (AddlDmg + TypeDmg) / 100) - intArray[0]) / (intArray[1] - intArray[0])

                double UIdmg =  String.IsNullOrEmpty(txtUIDmg.Text) ? 0 : Convert.ToDouble(txtUIDmg.Text);
                double TypeDmg = getTypeDamage(ddlFamily.SelectedValue.ToString());
                double AddlDmg = String.IsNullOrEmpty(txtAddDmg.Text) ? 0 : Convert.ToDouble(txtAddDmg.Text);
                AddlDmg += getTalentDamage();

                double bonusDmg =  1 + ((AddlDmg + TypeDmg) / 100);
                double baseDmg = UIdmg / bonusDmg;


                int[] intArray = ((WeaponModel)weaponList.Where(x => x.Model == ddlModel.SelectedValue.ToString()).ToArray()[0]).DamageRange;

                double rollPercentage = (baseDmg - intArray[0]) / (intArray[1] - intArray[0]);

                lblCalculateRoll.Text = String.Format("{0} roll percentage", ddlModel.SelectedValue.ToString());
                lblCalculateRoll.Visibility = Visibility.Visible;

                spWeaponRoll.Visibility = Visibility.Visible;
                prgWeaponRoll.Value = Math.Round(rollPercentage * 100, 2);
                lblWeaponRoll.Content = String.Format("{0}%", prgWeaponRoll.Value);
                lblUpdatedRng.Content = String.Format("Damage Range:\n{0}/{1}", Math.Round(selectedWeapon.DamageRange[0] * ((bonusDmg < 0) ? 1 : bonusDmg), 0), Math.Round(selectedWeapon.DamageRange[1] * ((bonusDmg < 0) ? 1 : bonusDmg), 0));

                btnAdd.Visibility = Visibility.Visible;
                btnRemove.Visibility = Visibility.Visible;
                lblUpdatedRng.Visibility = Visibility.Visible;
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Please enter values before calculating", "Error", MessageBoxButton.OK);
            }
        }
        private void btnCloseSections_Click(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;

            if (currentButton.Name == "btnTypeClose")
            {
                updateTypes();
            }

            dpTypeDamage.Visibility = Visibility.Collapsed;
            dpTalents.Visibility = Visibility.Collapsed;

            btnTypeOpen.Visibility = Visibility.Visible;
            btnTalentsOpen.Visibility = Visibility.Visible;
        }
        private void btnOpenWeaponTalents_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).Visibility = Visibility.Collapsed;
            btnTypeOpen.Visibility = Visibility.Collapsed;
            dpTalents.Visibility = Visibility.Visible;
        }
        private void btnOpenWeaponInfo_Click(object sender, RoutedEventArgs e)
        {
            Weapons.Shared.WeaponInfo wiWindow = new Weapons.Shared.WeaponInfo();
            wiWindow.Show();
        }
        private void btnOpenWeaponTypes_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).Visibility = Visibility.Collapsed;
            btnTalentsOpen.Visibility = Visibility.Collapsed;

            dpTypeDamage.Visibility = Visibility.Visible;
        } 
        private void btnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (lvPercentileComp.HasItems)
            {
                if (lvPercentileComp.SelectedItem != null)
                {
                    lvPercentileComp.Items.RemoveAt(lvPercentileComp.SelectedIndex);

                    if (!lvPercentileComp.HasItems)
                    {
                        //lvPercentileComp.IsEnabled = false;
                        //btnDPSCalc_Clear.IsEnabled = false;
                        btnRemove.IsEnabled = false;
                        if (ddlModel.SelectedValue == null) { btnAdd.IsEnabled = false; }

                        //btnDPSCalc_Clear.Visibility = Visibility.Collapsed;
                        btnRemove.Visibility = Visibility.Collapsed;
                        btnCalculate.Visibility = Visibility.Collapsed;
                        lvPercentileComp.Visibility = Visibility.Collapsed;
                    }
                }

                lvPercentileComp.Items.Refresh();
            }
        }
        private void txtTypeDamage_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateTypes();
        }
        private void textbox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Classes.Helper_Classes.EventHandlers.IsTextAllowed(e.Text);
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

            if (chkMeasured.IsChecked == true)
            {
                if (tglMeasured.isToggled && tglMeasured.IsEnabled) { allWeaponDamage = 30; }
                else if (!tglMeasured.isToggled && tglMeasured.IsEnabled) { allWeaponDamage = -15; }
            }

            if (chkOptimist.IsChecked == true)
            {
                if (tglOptimist.isToggled && tglOptimist.IsEnabled) { allWeaponDamage = 30; }
                else if (tglOptimist.isToggled && tglOptimist.IsEnabled) { allWeaponDamage = 0; }
            }

            if (chkUnhinged.IsChecked == true)
            {
                if (tglUnhinged.isToggled && tglUnhinged.IsEnabled) { allWeaponDamage = 20; }
                else if (tglUnhinged.isToggled && tglUnhinged.IsEnabled) { allWeaponDamage = 0; }
            }

            if (chkUnhinged.IsChecked == false && chkOptimist.IsChecked == false && chkMeasured.IsChecked == false) { allWeaponDamage = 0; }

            return allWeaponDamage;
        }
        private void updateTypes()
        {
            dicTypeDamage["Assault Rifle"] = String.IsNullOrEmpty(txtARDmg.Text) ? 0 : Convert.ToDouble(txtARDmg.Text);
            dicTypeDamage["Pistol"] = String.IsNullOrEmpty(txtPistolDmg.Text) ? 0 : Convert.ToDouble(txtPistolDmg.Text);
            dicTypeDamage["Rifle"] = String.IsNullOrEmpty(txtRifleDmg.Text) ? 0 : Convert.ToDouble(txtRifleDmg.Text);
            dicTypeDamage["Shotgun"] = String.IsNullOrEmpty(txtShotgunDmg.Text) ? 0 : Convert.ToDouble(txtShotgunDmg.Text);
            dicTypeDamage["SMG"] = String.IsNullOrEmpty(txtSMGDmg.Text) ? 0 : Convert.ToDouble(txtSMGDmg.Text);
            dicTypeDamage["LMG"] = String.IsNullOrEmpty(txtLMGDmg.Text) ? 0 : Convert.ToDouble(txtLMGDmg.Text);
            dicTypeDamage["MMR"] = String.IsNullOrEmpty(txtMMRDmg.Text) ? 0 : Convert.ToDouble(txtMMRDmg.Text);
        }
        private struct PercentileGridItem
        {
            public string ModelName { get; set; }
            public string Percentage { get; set; }
            public string AllWeaponDamage { get; set; }
            public string TypeDamage { get; set; }
            public string UIDamage { get; set; }
            public string dmgRange { get; set; }
        }

    }
}