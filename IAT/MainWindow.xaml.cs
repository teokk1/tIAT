using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IAT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Settings.init();
            InitializeComponent();

            if (Settings.ShowPracticeButton == false)
            {
                practiceButton.IsEnabled = false;
                practiceButton.Opacity = 0;
            }
        }

        void begin()
        {
            if (idTextBox.Text.Length > 0)
            {
                var gameWindow = new GameWindow(idTextBox.Text);
                gameWindow.Show();
            }
            else
                MessageBox.Show("Please enter an ID");
        }

        void begin_practice()
        {
            var gameWindow = new GameWindow(idTextBox.Text);
            gameWindow.Show();
        }

        private void beginButton_Click(object sender, RoutedEventArgs e) => begin();
        private void practiceButton_Click(object sender, RoutedEventArgs e) => begin_practice();

        private void idTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                begin();
                e.Handled = true;
            }
        }

        private void open_administration()
        {
            idTextBox.Clear();
            var adminWindow = new AdminWindow();
            adminWindow.Show();
        }

        private void idTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (idTextBox.Text.StartsWith(Settings.AdminPassword))
                open_administration();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }


        bool chars_valid(string text)
        {
            return text.All(c => char.IsDigit(c) || char.IsLetter(c) || c == '_');
        }

        private static readonly Regex allowedRegex = new Regex("[a-zA-Z0-9_]");
        private void idTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!allowedRegex.IsMatch(e.Text) || e.Text.Contains(" "))
                e.Handled = true;
        }

        private void idTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
    }
}
