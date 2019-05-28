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

namespace IAT.EditWindows
{
    /// <summary>
    /// Interaction logic for EditCategoryWindow.xaml
    /// </summary>
    public partial class EditCategoryWindow : Window
    {
        Category category;
        AdminWindow adminWindow;

        public EditCategoryWindow(AdminWindow adminWindow)
        {
            InitializeComponent();
            this.adminWindow = adminWindow;
            change_selected();
        }

        void change_selected()
        {
            category = adminWindow.selectedCategory;
            populate();
        }

        void populate()
        {
            nameTextBox.Text = category.name;
            //positionTextBox = category.position;
        }

        void flush()
        {
            adminWindow.flush();
        }

        void update()
        {
            category.name = nameTextBox.Text;
            //category.position = positionTextBox.psoition;

            flush();
            Close();
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            update();
        }
    }
}
