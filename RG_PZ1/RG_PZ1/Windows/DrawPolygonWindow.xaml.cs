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
using System.Windows.Shapes;

namespace RG_PZ1.Windows
{
    /// <summary>
    /// Interaction logic for DrawPolygonWindow.xaml
    /// </summary>
    public partial class DrawPolygonWindow : Window
    {
        public string PolygonFillColor { get; set; }
        public int PolygonBorderThickness { get; set; }
        public string PolygonBorderColor { get; set; }
        public string PolygonTextContent { get; set; }
        public string PolygonTextColor { get; set; }
        public bool CanDraw { get; set; } = false;

        public DrawPolygonWindow()
        {
            InitializeComponent();

            cbFillColors.ItemsSource = typeof(Brushes).GetProperties().Select(p => p.Name);
            cbFillColors.SelectedItem = cbFillColors.Items[0];

            cbBorderColors.ItemsSource = typeof(Brushes).GetProperties().Select(p => p.Name);
            cbBorderColors.SelectedItem = cbBorderColors.Items[0];

            cbTextColors.ItemsSource = typeof(Brushes).GetProperties().Select(p => p.Name);
            cbTextColors.SelectedItem = "Black";
        }

        private void btnDrawPolygon_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;

            if (int.TryParse(tbBorderThickness.Text, out int borderThickness))
            {
                PolygonBorderThickness = borderThickness;
            }
            else
            {
                isValid = false;
            }

            if (isValid)
            {
                PolygonTextContent = tbTextContent.Text;
                PolygonTextColor = cbTextColors.SelectedItem.ToString();
                PolygonFillColor = cbFillColors.SelectedItem.ToString();
                PolygonBorderColor = cbBorderColors.SelectedItem.ToString();
                CanDraw = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Check if inputs are valid!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
