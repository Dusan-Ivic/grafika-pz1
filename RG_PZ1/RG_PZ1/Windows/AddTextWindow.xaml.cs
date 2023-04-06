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
    /// Interaction logic for AddTextWindow.xaml
    /// </summary>
    public partial class AddTextWindow : Window
    {
        public string TextContent { get; set; }
        public int TextSize { get; set; }
        public string TextColor { get; set; }
        public bool CanDraw { get; set; } = false;

        public AddTextWindow()
        {
            InitializeComponent();

            cbTextColors.ItemsSource = typeof(Brushes).GetProperties().Select(p => p.Name);
            cbTextColors.SelectedItem = cbTextColors.Items[0];
        }

        private void btnAddText_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;

            if (tbTextContent.Text.Trim().Length > 0)
            {
                TextContent = tbTextContent.Text.Trim();
            }
            else
            {
                isValid = false;
            }

            if (int.TryParse(tbTextSize.Text, out int textSize))
            {
                TextSize = textSize;
            }
            else
            {
                isValid = false;
            }

            if (isValid)
            {
                TextColor = cbTextColors.SelectedItem.ToString();
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
