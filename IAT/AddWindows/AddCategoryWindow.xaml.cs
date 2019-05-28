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
    /// Interaction logic for AddCategoryWindow.xaml
    /// </summary>
    public partial class AddCategoryWindow : Window
    {
        AdminWindow adminWindow;
        public AddCategoryWindow(AdminWindow adminWindow)
        {
            this.adminWindow = adminWindow;
            InitializeComponent();
        }

        bool validate_input()
        {
            if (nameTextBox.Text.Length < 1)
            {
                MessageBox.Show("Kategorija mora imati ime!");
                return false;
            }

            return true;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (validate_input() == false)
                return;

            Category category = new Category();

            category.name = nameTextBox.Text;

            adminWindow.add_category(category);

            nameTextBox.Clear();
        }
    }
}
