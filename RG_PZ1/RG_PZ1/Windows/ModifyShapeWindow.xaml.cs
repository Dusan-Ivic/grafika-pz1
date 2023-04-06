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
    /// Interaction logic for ModifyShapeWindow.xaml
    /// </summary>
    public partial class ModifyShapeWindow : Window
    {
        public string ShapeFillColor { get; set; }
        public int ShapeBorderThickness { get; set; }
        public string ShapeBorderColor { get; set; }
        public bool CanModify { get; set; } = false;

        public ModifyShapeWindow()
        {
            InitializeComponent();

            cbFillColors.ItemsSource = typeof(Brushes).GetProperties().Select(p => p.Name);
            cbFillColors.SelectedItem = cbFillColors.Items[0];

            cbBorderColors.ItemsSource = typeof(Brushes).GetProperties().Select(p => p.Name);
            cbBorderColors.SelectedItem = cbBorderColors.Items[0];
        }

        private void btnModifyShape_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;

            if (int.TryParse(tbBorderThickness.Text, out int borderThickness))
            {
                ShapeBorderThickness = borderThickness;
            }
            else
            {
                isValid = false;
            }

            if (isValid)
            {
                ShapeFillColor = cbFillColors.SelectedItem.ToString();
                ShapeBorderColor = cbBorderColors.SelectedItem.ToString();
                CanModify = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Check if inputs are valid!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
