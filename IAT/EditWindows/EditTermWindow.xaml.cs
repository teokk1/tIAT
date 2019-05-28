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
    /// Interaction logic for EditTermWindow.xaml
    /// </summary>
    public partial class EditTermWindow : Window
    {
        Term term;
        AdminWindow adminWindow;

        public EditTermWindow(AdminWindow adminWindow)
        {
            InitializeComponent();
            this.adminWindow = adminWindow;
            change_selected();
        }

        void change_selected()
        {
            term = adminWindow.selectedTerm;
            populate();
        }

        void populate()
        {
            valueTextBox.Text = term.value;
        }

        void flush()
        {
            adminWindow.flush();
        }

        void update()
        {
            term.value = valueTextBox.Text;

            flush();
            Close();
        }

        private void editButton_Click_1(object sender, RoutedEventArgs e)
        {
            update();
        }
    }
}
