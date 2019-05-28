using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for AddTestWindow.xaml
    /// </summary>
    public partial class AddTestWindow : Window
    {
        AdminWindow adminWindow;
        public AddTestWindow(AdminWindow adminWindow)
        {
            this.adminWindow = adminWindow;
            InitializeComponent();

            populate_textboxes();
        }

        void populate_textboxes()
        {
            minQuestionTimeTextBox.Text = Settings.DefaultMinQuestionTime.ToString();
            maxQuestionTimeTextBox.Text = Settings.DefaultMaxQuestionTime.ToString();

            maxQuestionTimeBeforeIgnoreTextBox.Text = Settings.DefaultMaxQuestionTimeBeforeIgnore.ToString();

            minTestTimeTextBox.Text = Settings.DefaultMinTotalTime.ToString();
            maxTestTimeTextBox.Text = Settings.DefaultMaxTotalTime.ToString();

            belowMinimumAllowedTextBox.Text = Settings.DefaultAllowedPercentageBelowMinQuestionTime.ToString();
            aboveMaximumAllowedTextBox.Text = Settings.DefaultAllowedPercentageAboveMaxQuestionTime.ToString();
        }

        public static double parse_textbox(TextBox textbox)
        {
            try
            {
                return Double.Parse(textbox.Text);
            }
            catch
            {
                MessageBox.Show("Neispravan unos za: " + textbox.Name);
                return 0;
            }
        }

        bool validate_test(Test test)
        {
            if(test.name.Length < 1)
            {
                MessageBox.Show("Test mora imati ime!");
                return false;
            }

            //duplicate name check?

            bool questionTimesValid = test.minQuestionTime > 0 && test.maxQuestionTime > test.minQuestionTime;
            bool testTimesValid = test.minTotalTime > 0 && test.maxTotalTime > test.minTotalTime;

            return questionTimesValid && testTimesValid;
        }

        void add_test()
        {
            var test = new Test();
            test.name = testnameTextBox.Text;
            test.description = testDescriptionTextBox.Text;

            test.minQuestionTime = parse_textbox(minQuestionTimeTextBox);
            test.maxQuestionTime = parse_textbox(maxQuestionTimeTextBox);

            test.maxQuestionTimeBeforeIgnore = parse_textbox(maxQuestionTimeBeforeIgnoreTextBox);

            test.minTotalTime = parse_textbox(minTestTimeTextBox);
            test.maxTotalTime = parse_textbox(maxTestTimeTextBox);

            test.allowedPercentageBelowMinQuestionTime = parse_textbox(belowMinimumAllowedTextBox);
            test.allowedPercentageAboveMaxQuestionTime = parse_textbox(aboveMaximumAllowedTextBox);

            if (validate_test(test))
                adminWindow.add_test(test);

            testnameTextBox.Clear();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            add_test();
        }
    }
}
