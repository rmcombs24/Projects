﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Division2_Toolkit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
                double UIdmg = Convert.ToDouble(txtUIDmg.Text);
                double TypeDmg = Convert.ToDouble(txtTypeDmg.Text);
                double AddlDmg = Convert.ToDouble(txtAddDmg.Text);
                double basedmg = UIdmg / (1 + AddlDmg + TypeDmg);

                lblCalculateRoll.Content = String.Format("Top Damage for \n{0} ", ddlModel.SelectedValue.ToString());
                lblCalculateRollVal.Content = String.Format("{0}", Math.Round(basedmg));

                lblCalculateRoll.Visibility = Visibility.Visible;
                lblCalculateRollVal.Visibility = Visibility.Visible;
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


    }
}
