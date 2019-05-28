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
    /// Interaction logic for AddBlockWindow.xaml
    /// </summary>
    /// 
    public partial class AddBlockWindow : Window
    {
        AdminWindow adminWindow;
        public AddBlockWindow(AdminWindow adminWindow)
        {
            this.adminWindow = adminWindow;
            InitializeComponent();
        }

        public static bool validate_input(AdminWindow adminWindow, TextBox nameTextBox, TextBox trialCountTextBox, TextBox idTextBox, Block block)
        {
            if (nameTextBox.Text.Length < 1)
            {
                MessageBox.Show("Blok mora imati ime!");
                return false;
            }

            int trialCount;
            if(Int32.TryParse(trialCountTextBox.Text, out trialCount) == false)
            {
                MessageBox.Show("Neispravan broj triala!");
                return false;
            }

            int id;
            if(Int32.TryParse(idTextBox.Text, out id) == false)
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

        public static void initialize(Block block, TextBox nameTextBox, TextBox idTextBox, TextBox sdGroupTextBox, TextBox descriptionTextBox, TextBox trialCountTextBox)
        {
            block.name = nameTextBox.Text;
            block.description = descriptionTextBox.Text;
            block.sdGroup = sdGroupTextBox.Text;
            block.trialCount = Int32.Parse(trialCountTextBox.Text);
            block.id = Int32.Parse(idTextBox.Text);
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            Block block = new Block();

            if (validate_input(adminWindow, nameTextBox, trialCountTextBox, idTextBox, block) == false)
                return;

            initialize(block, nameTextBox, idTextBox, sdGroupTextBox, descriptionTextBox, trialCountTextBox);

            adminWindow.add_block(block);

            nameTextBox.Clear();
        }
    }
}
