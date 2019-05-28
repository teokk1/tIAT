using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace IAT
{
    public sealed class FGConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ListViewItem item = (ListViewItem)value;
            ListView listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
            int index = listView.ItemContainerGenerator.IndexFromContainer(item);

            var group = (Group)listView.Items[index];

            return group.ColorBrush;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public static class ShuffleExtension
    {
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while(n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }

    public static class StringExtensions
    {
        public static string Properize(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
    }

    public partial class GameWindow : Window
    {
        Test test;

        List<Term> terms = new List<Term>();

        ObservableCollection<Group> leftGroupList = new ObservableCollection<Group>();
        ObservableCollection<Group> rightGroupList = new ObservableCollection<Group>();

        string participantID;

        Term centerTerm;

        bool pause = false;
        bool practice = false;
        bool saved = false;

        int termsIndex = 0;
        int blocksIndex = 0;

        Key[] keysLeft = { Key.E };//, Key.Left };
        Key[] keysRight = { Key.I };//, Key.Right };

        TestRun testRun;

        DispatcherTimer wrongGuessCooldownTimer;
        Stopwatch guessStopWatch = new Stopwatch();

        Random random = new Random();

        public static string get_name(XmlNode element)
        {
            return element.Attributes["name"].Value;
        }

        public void Pause()
        {
            pause = true;
        }

        public void UnPause()
        {
            pause = false;
        }

        public GameWindow(string participantID, bool practice = false)
        {
            this.participantID = participantID;
            this.practice = practice;     
            
            InitializeComponent();

            Mouse.OverrideCursor = Cursors.None;

            Loaded += GameWindow_Loaded;            
        }

        private void GameWindow_Loaded(object sender, RoutedEventArgs e)
        {
            init_labels();
            init_colors();

            create_wrong_guess_cooldown_timer();
            read_test_file();
            start_test_run();

            start();
        }

        void show_message(string message, double topHeight = 0)
        {
            MessageWindow.show(this, message, topHeight);
        }

        void read_test_file()
        {
            if (practice)
            try
            {
                test = Test.read_XML(Test.test_file(Settings.PracticeTest));
            }
            catch
            {
                MessageBox.Show("Couldn't find practice test");
                Close();
            }
            else
            try
            {
                test = Test.read_XML(Test.test_file(Settings.ActiveTest));
            }
            catch
            {
                MessageBox.Show("Couldn't find test file. Please check the /Tests/" + Settings.ActiveTest + "/ folder for a text.xml file.");
                Close();
            }

            show_test_description();
        }

        void start_test_run()
        {
             if(practice == false)
                testRun = new TestRun(test, participantID);
        }

        void init_labels()
        {
            init_center_label();
            init_wrong_label();
            create_group_labels();
        }

        void init_center_label()
        {
            centerLabel.FontSize = Settings.CenterFontSize;
            centerLabel.Content = "";
        }

        void init_wrong_label()
        {
            wrongLabel.Visibility = Visibility.Hidden;
            wrongLabel.FontSize = Settings.WrongGuessFeedbackSize;
        }

        void init_colors()
        {
            Background = new SolidColorBrush(Settings.MainBackgroundColor);
            centerLabel.Foreground = new SolidColorBrush(Settings.CenterLabelColor);
        }

        void autoSizeColumns(ListView listView)
        {
            GridView gv = listView.View as GridView;
            if (gv != null)
                foreach (var c in gv.Columns)
                {
                    if (double.IsNaN(c.Width))
                        c.Width = c.ActualWidth;
                    c.Width = double.NaN;
                }
        }

        void update_group_labels(Block block)
        {
            leftGroupList.Clear();
            rightGroupList.Clear();

            foreach (var category in block.categories)
                foreach (var group in category.groups)
                    if (group.side == "Left")
                        leftGroupList.Add(group);
                    else
                        rightGroupList.Add(group);

            autoSizeColumns(leftGroups);
            autoSizeColumns(rightGroups);
        }

        public void create_group_labels()
        {
            leftGroups.FontSize = Settings.GroupFontSize;
            rightGroups.FontSize = Settings.GroupFontSize;

            leftGroups.ItemsSource = leftGroupList;
            rightGroups.ItemsSource = rightGroupList;
        }

        void write_test_data()
        {
            testRun.write();
            saved = true;
        }

        string iat_to_string(double iat)
        {
            var absoluteIat = Math.Abs(iat);

            if(iat > 0)
            {
                if (absoluteIat > test.strongTreshold)
                    return test.strongPositiveString;
                if (absoluteIat > test.moderateTreshold)
                    return test.moderatePositiveString;
                if (absoluteIat > test.slightTreshold)
                    return test.slightPositiveString;
            }

            if(iat < 0)
            {
                if (absoluteIat > test.strongTreshold)
                    return test.strongNegativeString;
                if (absoluteIat > test.moderateTreshold)
                    return test.moderateNegativeString;
                if (absoluteIat > test.slightTreshold)
                    return test.slightNegativeString;
            }

            return test.noBiasString;
        }

        void test_complete()
        {
            if (practice == false)
                write_test_data();

            show_message(test.completeMessage);

            if (Settings.ShowResultString)
                show_message(iat_to_string(testRun.finalResult));

            if(Settings.ShowIAT)
                show_message("Konačni rezultat: " + testRun.finalResult);

            Close();
        }

        #region BlockCOntrol
        void next_block() => play_block(test.blocks[blocksIndex]);
        void start() => next_block();

        private void change_center()
        {
            if (termsIndex >= test.blocks[blocksIndex].trialCount)
            {
                block_complete();
                return;
            }

            var previousTerm = centerTerm;

            if(terms.Count > 1)
                while(centerTerm == previousTerm)
                    centerTerm = terms[random.Next(terms.Count)];
            else
                centerTerm = terms[random.Next(terms.Count)];

            centerLabel.Content = centerTerm.value;            

            if(Settings.ColorCenterAsGroup)
                centerLabel.Foreground = centerTerm.group.ColorBrush;
            else
                centerLabel.Foreground = new SolidColorBrush(Settings.CenterLabelColor);

            guessStopWatch.Restart();

            termsIndex++;
        }

        void populate_terms(Block block)
        {
            termsIndex = 0;
            terms.Clear();

            foreach (var category in block.categories)
                foreach (var group in category.groups)
                    terms.AddRange(group.terms);
        }

        public void show_test_description()
        {
            var descriptions = test.description.Split(new string[] { Settings.PageEndString }, StringSplitOptions.None);

            var descriptionPageLabel = new Label();

            show_message(descriptions[0]);
        }

        double listsOffset()
        {
            var listsHeight = Math.Max(leftGroupList.Count * Settings.GroupFontSize, rightGroupList.Count * Settings.GroupFontSize);

             if (listsHeight > 0)
                return listsHeight + Height * 0.05;

            return 0;
        }

        public void play_block(Block block)
        {
            update_group_labels(block);
            show_message(block.description, listsOffset());

            populate_terms(block);

            change_center();

            if (practice == false)
                testRun.add_block(block);         
        }

        private void block_complete()
        {
            centerLabel.Content = "";

            blocksIndex++;

            if (blocksIndex == test.blocks.Count)
                test_complete();
            else
                next_block();
        }
        #endregion

        #region GuessChecking
        void create_wrong_guess_cooldown_timer()
        {
            wrongGuessCooldownTimer = new DispatcherTimer(DispatcherPriority.SystemIdle);
            wrongGuessCooldownTimer.Tick += new EventHandler(unpause_after_wrong_guess);
            wrongGuessCooldownTimer.Interval = TimeSpan.FromMilliseconds(Settings.WrongGuessCooldownTime);
        }

        private void correct_guess()
        {
            wrongLabel.Visibility = Visibility.Hidden;
            Console.WriteLine("Correct");
            change_center();
        }

        private void unpause_after_wrong_guess(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();
            change_center();
            UnPause();
        }

        void start_wrong_guess_cooldown()
        {
            wrongGuessCooldownTimer.Start();
        }

        private void wrong_guess()
        {
            centerLabel.Content = "X";
            centerLabel.Foreground = Brushes.Red;

            Pause();

            start_wrong_guess_cooldown();
        }

        void check_guess(bool leftPressed, bool rightPressed)
        {
            if (pause)
                return;

            guessStopWatch.Stop();

            bool isCorrect = leftPressed && item_in_list(leftGroupList) || rightPressed && item_in_list(rightGroupList);

            if (practice == false)
                testRun.add_guess(isCorrect, guessStopWatch.ElapsedMilliseconds, centerTerm.group.positivity, centerTerm.value, termsIndex);

            if (isCorrect)
                correct_guess();
            else
                wrong_guess();
        }

        bool item_in_list(ObservableCollection<Group> list)
        {
            return list.Any(g => g == centerTerm.group);
        }
        #endregion

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();

            if (pause)
            {
                e.Handled = true;
                return;
            }

            bool leftPressed = keysLeft.Contains(e.Key);
            bool rightPressed = keysRight.Contains(e.Key);

            if(leftPressed || rightPressed)
                check_guess(leftPressed, rightPressed);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (saved == false)
                testRun.write_incomplete();

            Mouse.OverrideCursor = null;
        }
    }
}
