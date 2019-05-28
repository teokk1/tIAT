using ColorPickerWPF;
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

namespace IAT
{
    /// <summary>
    /// Interaction logic for AddGroupWindow.xaml
    /// </summary>
    public partial class AddGroupWindow : Window
    {
        Color color = Color.FromRgb(255, 255, 255);
        AdminWindow adminWindow;

        public AddGroupWindow(AdminWindow adminWindow)
        {
            this.adminWindow = adminWindow;
            InitializeComponent();

            colorPicker.SelectedColor = Settings.DefaultListColor;
        }

        bool validate_input()
        {
            if (nameTextBox.Text.Length < 1)
            {
                MessageBox.Show("Grupa mora imati ime!");
                return false;
            }

            return true;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if(validate_input() == false)
                return;

            Group group = new Group();

            group.name = nameTextBox.Text;
            group.side = leftRadio.IsChecked == true ? "Left" : "Right";
            group.positivity = positiveRadio.IsChecked == true ? "Positive" : "Negative";
            group.hexColor = colorPicker.SelectedColor.Value.ToString();

            adminWindow.add_group(group);

            nameTextBox.Clear();
        }
    }
}
