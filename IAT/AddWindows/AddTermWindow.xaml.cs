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
    /// Interaction logic for AddTermWindow.xaml
    /// </summary>
    public partial class AddTermWindow : Window
    {
        AdminWindow adminWindow;
        public AddTermWindow(AdminWindow adminWindow)
        {
            this.adminWindow = adminWindow;
            InitializeComponent();
        }

        bool validate_input()
        {
            if (valueTextBox.Text.Length < 1)
            {
                MessageBox.Show("Termin mora imati vrijednost!");
                return false;
            }

            return true;
        }

        void add()
        {
            if (validate_input() == false)
                return;

            Term term = new Term();

            term.value = valueTextBox.Text;
            term.group = adminWindow.selectedGroup;

            adminWindow.add_term(term);

            valueTextBox.Clear();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            add();
        }

        private void valueTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                add();
        }
    }
}
