using System.Windows;

namespace Division2Toolkit.Views.Weapons.Shared
{
    /// <summary>
    /// Interaction logic for WeaponInfo.xaml
    /// </summary>
    public partial class WeaponInfo : Window
    {
        public WeaponInfo()
        {
            InitializeComponent();
            LoadValues(WeaponPercentile.selectedWeapon);
        }

        public void LoadValues(WeaponModel weaponModel)
        {
            lblWeaponName.Content = weaponModel.Model;

            lblCritMinMaxVal.Content = weaponModel.CritMinMax;
            lblDamageRangeVal.Content = weaponModel.DamageRangeStr;
            lblHSMultiplierVal.Content = weaponModel.HSMultiplier;
            lblMagSizeVal.Content = weaponModel.MagSize;
            lblOptimalRangeVal.Content = weaponModel.OptimalRange;
            lblRPMVal.Content = weaponModel.RPM;
            lblReloadSpeedVal.Content = weaponModel.ReloadSpeed;
            lblNormalizedDmgVal.Content = weaponModel.NormalizedDmg;
        }
    }
}
