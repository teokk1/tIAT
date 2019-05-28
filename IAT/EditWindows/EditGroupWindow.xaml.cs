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

namespace IAT.EditWindows
{
    /// <summary>
    /// Interaction logic for EditGroupWindow.xaml
    /// </summary>
    public partial class EditGroupWindow : Window
    {
        Color color;
        Group group;

        AdminWindow adminWindow;
        public EditGroupWindow(AdminWindow adminWindow)
        {
            InitializeComponent();
            this.adminWindow = adminWindow;
            change_selected();
        }

        void change_selected()
        {
            group = adminWindow.selectedGroup;

            populate();
        }

        void update_side_radio()
        {
            if (group.side == "Left")
                leftRadio.IsChecked = true;
            else
                rightRadio.IsChecked = true;
        }

        void update_positivity_radio()
        {
            if (group.positivity == "Negative")
                negativeRadio.IsChecked = true;
            else
                positiveRadio.IsChecked = true;
        }

        void populate()
        {
            nameTextBox.Text = group.name;
            update_side_radio();
            update_positivity_radio();
            colorPicker.SelectedColor = group.Color;
        }

        void flush()
        {
            adminWindow.flush();
        }

        void update()
        {
            group.name = nameTextBox.Text;
            group.side = leftRadio.IsChecked == true ? "Left" : "Right";
            group.positivity = negativeRadio.IsChecked == true ? "Negative" : "Positive";
            group.ColorBrush = new SolidColorBrush(colorPicker.SelectedColor.Value);

            flush();
            Close();
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            update();
        }
    }
}
