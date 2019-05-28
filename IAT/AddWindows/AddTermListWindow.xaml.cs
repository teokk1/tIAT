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
    /// Interaction logic for AddTermListWindow.xaml
    /// </summary>
    public partial class AddTermListWindow : Window
    {
        AdminWindow adminWindow;
        public AddTermListWindow(AdminWindow adminWindow)
        {
            this.adminWindow = adminWindow;
            InitializeComponent();
        }

        bool validate_input(string[] list)
        {
            if(list.Count() < 1)
            {
                MessageBox.Show("Potreban je bar jedan delimiter znak!");
                return false;
            }

            if (listTextBox.Text.Length < 1)
            {
                MessageBox.Show("Potrebno je unijeti listu termina!");
                return false;
            }

            return true;
        }

        void add_delimiter(List<string> list, TextBox t)
        {
            if(t.Text.Length > 0)
                list.Add(t.Text);
        }

        string[] read_delimiters()
        {
            var list = new List<string>();

            add_delimiter(list, delimiterTextBox1);
            add_delimiter(list, delimiterTextBox2);
            add_delimiter(list, delimiterTextBox3);
            add_delimiter(list, delimiterTextBox4);
            add_delimiter(list, delimiterTextBox5);

            return list.ToArray();
        }

        void add()
        {
            var delimiters = read_delimiters();

            if (validate_input(delimiters) == false)
                return;

            foreach (var value in listTextBox.Text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries))
            {
                Term term = new Term();

                term.value = value.Trim().Properize();
                term.group = adminWindow.selectedGroup;

                adminWindow.add_term(term);
            }           

            listTextBox.Clear();
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
