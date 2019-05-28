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
    /// Interaction logic for EditTestWindow.xaml
    /// </summary>
    public partial class EditTestWindow : Window
    {
        Test test;
        AdminWindow adminWindow;

        public EditTestWindow(AdminWindow adminWindow)
        {
            InitializeComponent();
            this.adminWindow = adminWindow;
            change_selected();
        }

        void change_selected()
        {
            test = adminWindow.selectedTest;
            populate();
        }

        void populate()
        {
            nameTextBox.Text = test.name;
            descriptionTextBox.Text = test.description;

            minQuestionTimeTextBox.Text = test.minQuestionTime.ToString();
            maxQuestionTimeTextBox.Text = test.maxQuestionTime.ToString();

            maxQuestionTimeBeforeIgnoreTextBox.Text = test.maxQuestionTimeBeforeIgnore.ToString();

            minTestTimeTextBox.Text = test.minTotalTime.ToString();
            maxTestTimeTextBox.Text = test.maxTotalTime.ToString();

            belowMinimumAllowedTextBox.Text = test.allowedPercentageBelowMinQuestionTime.ToString();
            aboveMaximumAllowedTextBox.Text = test.allowedPercentageAboveMaxQuestionTime.ToString();
        }

        void flush()
        {
            test.flush();
        }

        void handle_name_change()
        {
            //update name
            //delete folder
        }

        void update()
        {
            if (test.name != nameTextBox.Text)
                handle_name_change();
            
            test.description = descriptionTextBox.Text;

            test.minQuestionTime = AddTestWindow.parse_textbox(minQuestionTimeTextBox);
            test.maxQuestionTime = AddTestWindow.parse_textbox(maxQuestionTimeTextBox);

            test.maxQuestionTimeBeforeIgnore = AddTestWindow.parse_textbox(maxQuestionTimeBeforeIgnoreTextBox);

            test.minTotalTime = AddTestWindow.parse_textbox(minTestTimeTextBox);
            test.maxTotalTime = AddTestWindow.parse_textbox(maxTestTimeTextBox);

            test.allowedPercentageBelowMinQuestionTime = AddTestWindow.parse_textbox(belowMinimumAllowedTextBox);
            test.allowedPercentageAboveMaxQuestionTime = AddTestWindow.parse_textbox(aboveMaximumAllowedTextBox);

            flush();
            Close();
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            update();
        }
    }
}
