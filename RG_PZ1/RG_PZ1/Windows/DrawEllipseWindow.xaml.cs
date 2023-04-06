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
    /// Interaction logic for DrawEllipseWindow.xaml
    /// </summary>
    public partial class DrawEllipseWindow : Window
    {
        public int EllipseRadiusX { get; set; }
        public int EllipseRadiusY { get; set; }
        public string EllipseFillColor { get; set; }
        public int EllipseBorderThickness { get; set; }
        public string EllipseBorderColor { get; set; }
        public string EllipseTextContent { get; set; }
        public string EllipseTextColor { get; set; }
        public bool CanDraw { get; set; } = false;

        public DrawEllipseWindow()
        {
            InitializeComponent();

            cbFillColors.ItemsSource = typeof(Brushes).GetProperties().Select(p => p.Name);
            cbFillColors.SelectedItem = cbFillColors.Items[0];

            cbBorderColors.ItemsSource = typeof(Brushes).GetProperties().Select(p => p.Name);
            cbBorderColors.SelectedItem = cbBorderColors.Items[0];

            cbTextColors.ItemsSource = typeof(Brushes).GetProperties().Select(p => p.Name);
            cbTextColors.SelectedItem = "Black";
        }

        private void btnDrawEllipse_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;

            if (int.TryParse(tbRadiusX.Text, out int radiusX))
            {
                EllipseRadiusX = radiusX;
            }
            else
            {
                isValid = false;
            }

            if (int.TryParse(tbRadiusY.Text, out int radiusY))
            {
                EllipseRadiusY = radiusY;
            }
            else
            {
                isValid = false;
            }

            if (int.TryParse(tbBorderThickness.Text, out int borderThickness))
            {
                EllipseBorderThickness = borderThickness;
            }
            else
            {
                isValid = false;
            }

            if (isValid)
            {
                EllipseTextContent = tbTextContent.Text;
                EllipseTextColor = cbTextColors.SelectedItem.ToString();
                EllipseFillColor = cbFillColors.SelectedItem.ToString();
                EllipseBorderColor = cbBorderColors.SelectedItem.ToString();
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
