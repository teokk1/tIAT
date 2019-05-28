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

namespace IAT.AddWindows
{
    /// <summary>
    /// Interaction logic for AddDescriptionBlockWindow.xaml
    /// </summary>
    public partial class AddDescriptionBlockWindow : Window
    {
        AdminWindow adminWindow;
        public AddDescriptionBlockWindow(AdminWindow adminWindow)
        {
            this.adminWindow = adminWindow;
            InitializeComponent();
        }

        public static bool validate_input(AdminWindow adminWindow, TextBox idTextBox, Block block)
        {
            int id;
            if (Int32.TryParse(idTextBox.Text, out id) == false)
            {
                MessageBox.Show("Neispravan broj unesen kao ID!");
                return false;
            }

            if (adminWindow.blockList.Items.Cast<Block>().Any(b => b.id == id && b != block))
            {
                MessageBox.Show("Unesen ID koji već postoji!");
                return false;
            }

            return true;
        }

        public static void initialize(Block block, TextBox nameTextBox, TextBox idTextBox, TextBox descriptionTextBox)
        {
            block.name = nameTextBox.Text;
            block.description = descriptionTextBox.Text;
            block.id = Int32.Parse(idTextBox.Text);
            block.trialCount = -1;
            block.isDescription = true;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            Block block = new Block();

            if (validate_input(adminWindow, idTextBox, block) == false)
                return;

            initialize(block, nameTextBox, idTextBox, descriptionTextBox);

            adminWindow.add_block(block);

            nameTextBox.Clear();
        }
    }
}
