using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Division2Toolkit.Views
{
    public partial class DPSCalculator : UserControl
    {
        private void textbox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }


        private static List<WeaponModel> weaponList = WeaponModel.ReadCSV("weapons.csv");

        public DPSCalculator()
        {
            InitializeComponent();
            ddlDPSCalc_Family.ItemsSource = WeaponModel.GetWeaponFamilies(weaponList);
        }

        #region Event Handlers

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
                MessageBox.Show("Please add an acceptable have value before adding to the compare.", "Error.", MessageBoxButton.OK);
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
                    currentItem.DPM = String.Format("{0:n0}", Math.Round((60.0 / (currentItem.MagSize / (currentItem.RPM / 60.0) + (currentItem.ReloadSpeed / 1000.0)) * currentItem.Damage * currentItem.MagSize)));
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

        private void ddlWeaponFamilyMakeModel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedDDLValue = (((ComboBox)sender).SelectedValue != null) ? ((ComboBox)sender).SelectedValue.ToString() : string.Empty;

            //I know I could have simplified this with a DataTemplate or using the same DDLS for everything but cest la vie
            //Maybe on a saturday I'll refactor.
            switch (((ComboBox)sender).Name)
            {
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
        #endregion
    }

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
}