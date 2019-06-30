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
    public partial class GearPercentile : UserControl
    {
        Brush DivisionOrange = new SolidColorBrush(Color.FromArgb(100, 253, 107, 13));

        private void textbox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }


        public GearPercentile()
        {
            InitializeComponent();
            chkIsGearSet.IsChecked = false;

            List<Gear> lstGear = Gear.ReadGearXLSX("gear.xlsx");

            ddlGear.ItemsSource = lstGear;
            ddlGear.DisplayMemberPath = "gearType";
            ddlGear.SelectedValue = "gearAttributes";
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


        private void btnCalculatePercentile_Gear_Click(object sender, RoutedEventArgs e)
        {
            for (int wpIndex = 0; wpIndex < wpAttributeDesc.Children.Count; wpIndex++)
            {
                if (wpAttributeDesc.Children[wpIndex] is StackPanel)
                {
                    List<string> lstValueString = new List<string>();
                    double min = 0, max = 0, userVal = 0;

                    foreach (UIElement uieChild in ((StackPanel)wpAttributeDesc.Children[wpIndex]).Children)
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
                            userVal = String.IsNullOrEmpty(((TextBox)uieChild).Text) ? 0 : Convert.ToDouble(((TextBox)uieChild).Text);
                        }
                        else if (uieChild is StackPanel)
                        {
                            double rollPercentage = Math.Round((userVal - min) / (max - min) * 100, 2);

                            for (int spIndex = 0; spIndex < ((StackPanel)uieChild).Children.Count; spIndex++)
                            {
                                if (((StackPanel)uieChild).Children[spIndex] is Label)
                                {
                                    ((Label)((StackPanel)uieChild).Children[spIndex]).Content = String.Format("{0}%", rollPercentage.ToString());
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
                Margin = new Thickness(10, 0, 5, 0),
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
                Margin = new Thickness(0, 0, 0, 5),
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
                Margin = new Thickness(0, -2, 0, 5)
            };
            /*
            <StackPanel Orientation="Verticle" Height="30" Width="75" Margin="0,0,0,5" VerticleAlignment="Center" isEnabled="false">
                <Label Content="0%"  />
                <ProgressBar Height="5" Width="75" />
            </StackPanel>
            */
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
                for (int index = 0; index < wpAttributeDesc.Children.Count; index++)
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

    }
}
