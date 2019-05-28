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
    /// Interaction logic for MessageWindow.xaml
    /// </summary>
    public partial class MessageWindow : Window
    {
        GameWindow gameWindow;
        MessageWindow(GameWindow gameWindow, string message)
        {
            this.gameWindow = gameWindow;
            InitializeComponent();

            Background = new SolidColorBrush(Settings.MainBackgroundColor);

            messageTextBox.Foreground = new SolidColorBrush(Settings.CenterLabelColor);
            messageTextBox.FontSize = Settings.TextFontSize;

            messageTextBox.Text = message;
        }

        void exit_message()
        {
            gameWindow.UnPause();
            DialogResult = true;
        }

        void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.Escape)
                exit_message();
        }

        void init_properties(double listsHeight)
        {
            Height = gameWindow.Height - listsHeight;
            Width = gameWindow.Width;

            Left = 0;
            Top = listsHeight;
        }

        public static void show(GameWindow gameWindow, string message, double topHeight = 0)
        {
            gameWindow.Pause();
            var window = new MessageWindow(gameWindow, message);

            window.init_properties(topHeight);
            window.ShowDialog();
        }
    }
}
