using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Division2Toolkit.Views.Weapons.Percentiles
{
    /// <summary>
    /// Interaction logic for TypeDamageWindow.xaml
    /// </summary>
    public partial class TypeDamageWindow : Window
    {
        public TypeDamageWindow()
        {
            InitializeComponent();
        }

        private void textbox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }


        private void wnd_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WeaponPercentile.dicTypeDamage["Assault Rifle"] = String.IsNullOrEmpty(txtARDmg.Text)       ? 0 : Convert.ToDouble(txtARDmg.Text);
            WeaponPercentile.dicTypeDamage["Pistol"]        = String.IsNullOrEmpty(txtPistolDmg.Text)   ? 0 : Convert.ToDouble(txtPistolDmg.Text);
            WeaponPercentile.dicTypeDamage["Rifle"]         = String.IsNullOrEmpty(txtRifleDmg.Text)    ? 0 : Convert.ToDouble(txtRifleDmg.Text);
            WeaponPercentile.dicTypeDamage["Shotgun"]       = String.IsNullOrEmpty(txtShotgunDmg.Text)  ? 0 : Convert.ToDouble(txtShotgunDmg.Text);
            WeaponPercentile.dicTypeDamage["SMG"]           = String.IsNullOrEmpty(txtSMGDmg.Text)      ? 0 : Convert.ToDouble(txtSMGDmg.Text);
            WeaponPercentile.dicTypeDamage["LMG"]           = String.IsNullOrEmpty(txtLMGDmg.Text)      ? 0 : Convert.ToDouble(txtLMGDmg.Text);
            WeaponPercentile.dicTypeDamage["MMR"]           = String.IsNullOrEmpty(txtMMRDmg.Text)      ? 0 : Convert.ToDouble(txtMMRDmg.Text);
        }
    }
}
