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
    /// Interaction logic for EditBlockWindow.xaml
    /// </summary>
    public partial class EditBlockWindow : Window
    {
        Block block;
        AdminWindow adminWindow;

        public EditBlockWindow(AdminWindow adminWindow)
        {
            InitializeComponent();
            this.adminWindow = adminWindow;
            change_selected();
        }

        void change_selected()
        {
            block = adminWindow.selectedBlock;

            populate();
        }

        void populate()
        {
            nameTextBox.Text = block.name;
            //positionTextBox = block.position;
            idTextBox.Text = block.id.ToString();
            sdGroupTextBox.Text = block.sdGroup;
            trialCountTextBox.Text = block.trialCount.ToString();
            descriptionTextBox.Text = block.description;
        }

        void flush()
        {
            adminWindow.flush();
        }

        void update()
        {
            if (AddBlockWindow.validate_input(adminWindow, nameTextBox, trialCountTextBox, idTextBox, block) == false)
                return;

            AddBlockWindow.initialize(block, nameTextBox, idTextBox, sdGroupTextBox, descriptionTextBox, trialCountTextBox);

            flush();
            Close();
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            update();
        }
    }
}
