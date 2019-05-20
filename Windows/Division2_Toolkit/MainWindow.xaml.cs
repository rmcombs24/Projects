using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Division2_WeaponDmg_Calc
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
            LoadDDLs();
        }

        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            // GunDmgInUI / (1 + WeapDmg + TypeDmg) = BaseDmg  
        }

        private void LoadDDLs()
        {   
            ddlFamily.ItemsSource = WeaponModel.GetWeaponFamilies(weaponList);
            //ddlModel.ItemsSource = WeaponModel.GetWeaponModels(weaponList);
        }

        private void ddl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {          
            string selectedDDLValue = (((ComboBox)sender).SelectedValue != null) ? ((ComboBox)sender).SelectedValue.ToString() : string.Empty;

            switch (((ComboBox)sender).Name)
            {
                case "ddlFamily":
                    ddlMake.ItemsSource = WeaponModel.GetWeaponMakesByFamily(weaponList, selectedDDLValue);
                    ddlMake.IsEnabled = true;
                    break;
                case "ddlMake":
                    ddlModel.ItemsSource =  (String.IsNullOrEmpty(selectedDDLValue)) ?  null : WeaponModel.GetWeaponModelsByMake(weaponList, selectedDDLValue);
                    ddlModel.IsEnabled = (String.IsNullOrEmpty(selectedDDLValue)) ? false : true;
                    break;
                case "ddlModel":

                    dgSelected.ItemsSource = weaponList.Where(x => x.Model == selectedDDLValue);
                    dgSelected.Visibility = Visibility.Visible;
                    //string selectedWeapon = weaponList.Where(x => x.Model == selectedDDLValue).Select(o => o.Model).ToArray()[0];
                    break;
                default:
                    break;
            }
        }
    }
}
