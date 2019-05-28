using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;

namespace IAT
{
    public class Settings
    {
        [XmlElement(ElementName = "GroupFontSize")]
        public int groupFontSize = 10;
        public static int GroupFontSize { get { return instance.groupFontSize; } set { instance.groupFontSize = value; flush();  } }

        [XmlElement(ElementName = "CenterFontSize")]
        public int centerFontSize = 10;
        public static int CenterFontSize { get { return instance.centerFontSize; } set { instance.centerFontSize = value; flush(); } }

        [XmlElement(ElementName = "WrongGuessFeedbackSize")]
        public int wrongGuessFeedbackSize = 10;
        public static int WrongGuessFeedbackSize { get { return instance.wrongGuessFeedbackSize; } set { instance.wrongGuessFeedbackSize = value; } }

        [XmlElement(ElementName = "TextFontSize")]
        public int textFontSize = 30;
        public static int TextFontSize { get { return instance.textFontSize; } set { instance.textFontSize = value; } }

        [XmlElement(ElementName = "MenuFontSize")]
        public int menuFontSize = 16;
        public static int MenuFontSize { get { return instance.menuFontSize; } set { instance.menuFontSize = value; flush(); } }

        [XmlElement(ElementName = "AdminPassword")]
        public string adminPassword = "password";
        public static string AdminPassword { get { return instance.adminPassword; } set { instance.adminPassword = value; flush(); } }

        [XmlElement(ElementName = "ActiveTest")]
        public string activeTest = "test1/";
        public static string ActiveTest { get { return instance.activeTest; } set { instance.activeTest = value; flush(); } }

        [XmlElement(ElementName = "PracticeTest")]
        public string practiceTest = "pratice_test/";
        public static string PracticeTest { get { return instance.practiceTest; } set { instance.practiceTest = value; flush(); } }

        [XmlElement(ElementName = "DefaultMinQuestionTime")]
        public double defaultMinQuestionTime = 300;
        public static double DefaultMinQuestionTime { get { return instance.defaultMinQuestionTime; } set { instance.defaultMinQuestionTime = value; flush(); } }

        [XmlElement(ElementName = "DefaultAllowedPercentageBelowMinQuestionTime")]
        public double defaultAllowedPercentageBelowMinQuestionTime = 10;
        public static double DefaultAllowedPercentageBelowMinQuestionTime { get { return instance.defaultAllowedPercentageBelowMinQuestionTime; } set { instance.defaultAllowedPercentageBelowMinQuestionTime = value; flush(); } }

        [XmlElement(ElementName = "DefaultAllowedPercentageAboveMaxQuestionTime")]
        public double defaultAllowedPercentageAboveMaxQuestionTime = 10;
        public static double DefaultAllowedPercentageAboveMaxQuestionTime { get { return instance.defaultAllowedPercentageAboveMaxQuestionTime; } set { instance.defaultAllowedPercentageAboveMaxQuestionTime = value; flush(); } }

        [XmlElement(ElementName = "DefaultMaxQuestionTime")]
        public double defaultMaxQuestionTime = 4000;
        public static double DefaultMaxQuestionTime { get { return instance.defaultMaxQuestionTime; } set { instance.defaultMaxQuestionTime = value; flush(); } }

        [XmlElement(ElementName = "DefaultMaxQuestionTimeBeforeIgnore")]
        public double defaultMaxQuestionTimeBeforeIgnore = 10000;
        public static double DefaultMaxQuestionTimeBeforeIgnore { get { return instance.defaultMaxQuestionTimeBeforeIgnore; } set { instance.defaultMaxQuestionTimeBeforeIgnore = value; flush(); } }

        [XmlElement(ElementName = "DefaultMinTotalTime")]
        public double defaultMinTotalTime = 30;
        public static double DefaultMinTotalTime { get { return instance.defaultMinTotalTime; } set { instance.defaultMinTotalTime = value; flush(); } }

        [XmlElement(ElementName = "DefaultMaxTotalTime")]
        public double defaultMaxTotalTime = 300;
        public static double DefaultMaxTotalTime { get { return instance.defaultMaxTotalTime; } set { instance.defaultMaxTotalTime = value; flush(); } }

        [XmlElement(ElementName = "DefaultMistakePenalty")]
        public double defaultMistakePenalty = 600;
        public static double DefaultMistakePenalty { get { return instance.defaultMistakePenalty; } set { instance.defaultMistakePenalty = value; flush(); } }

        [XmlElement(ElementName = "DefaultSlightTreshold")]
        public double defaultSlightTreshold = 3;
        public static double DefaultSlightTreshold { get { return instance.defaultSlightTreshold; } set { instance.defaultSlightTreshold = value; flush(); } }

        [XmlElement(ElementName = "DefaultModerateTreshold")]
        public double defaultModerateTreshold = 6;
        public static double DefaultModerateTreshold { get { return instance.defaultModerateTreshold; } set { instance.defaultModerateTreshold = value; flush(); } }

        [XmlElement(ElementName = "DefaultStrongTreshold")]
        public double defaultStrongTreshold = 10;
        public static double DefaultStrongTreshold { get { return instance.defaultStrongTreshold; } set { instance.defaultStrongTreshold = value; flush(); } }

        [XmlElement(ElementName = "TestCompleteMessage")]
        public string testCompleteMessage = "Test Complete!";
        public static string TestCompleteMessage { get { return instance.testCompleteMessage; } set { instance.testCompleteMessage = value; flush(); } }

        [XmlElement(ElementName = "MainBackgroundColor")]
        public string mainBackgroundColor = "#000000";
        public static Color MainBackgroundColor { get { return (Color)ColorConverter.ConvertFromString(instance.mainBackgroundColor); } set { instance.mainBackgroundColor = value.ToString(); flush(); } }

        [XmlElement(ElementName = "CenterLabelColor")]
        public string centerLabelColor = "#FFFFFF";
        public static Color CenterLabelColor { get { return (Color)ColorConverter.ConvertFromString(instance.centerLabelColor); } set { instance.centerLabelColor = value.ToString(); flush(); } }

        [XmlElement(ElementName = "DefaultListColor")]
        public string defaultListColor = "#FFFFFF";
        public static Color DefaultListColor { get { return (Color)ColorConverter.ConvertFromString(instance.defaultListColor); } set { instance.defaultListColor = value.ToString(); flush(); } }

        [XmlElement(ElementName = "PageEndString")]
        public string pageEndString = "PAGE_END";
        public static string PageEndString { get { return instance.pageEndString; } set { instance.pageEndString = value; flush(); } }

        [XmlElement(ElementName = "WrongGuessCooldownTime")]
        public double wrongGuessCooldownTime = 400;
        public static double WrongGuessCooldownTime { get { return instance.wrongGuessCooldownTime; } set { instance.wrongGuessCooldownTime = value; flush(); } }

        [XmlElement(ElementName = "ShowPracticeButton")]
        public bool showPracticeButton = true;
        public static bool ShowPracticeButton { get { return instance.showPracticeButton; } set { instance.showPracticeButton = value; flush(); } }

        [XmlElement(ElementName = "ShowIAT")]
        public bool showIAT = false;
        public static bool ShowIAT { get { return instance.showIAT; } set { instance.showIAT = value; flush(); } }

        [XmlElement(ElementName = "ShowResultString")]
        public bool showResultString = false;
        public static bool ShowResultString { get { return instance.showResultString; } set { instance.showResultString = value; flush(); } }

        [XmlElement(ElementName = "ColorCenterAsGroup")]
        public bool colorCenterAsGroup = true;
        public static bool ColorCenterAsGroup { get { return instance.colorCenterAsGroup; } set { instance.colorCenterAsGroup = value; flush(); } }

        public const string fileName = "settings.xml";
        public const string testsFolder = "Tests/";

        public Settings()
        {         
        }

        public static void init()
        {
            try
            {
                var serializer = new XmlSerializer(instance.GetType());
                using (Stream inputStream = File.OpenRead(fileName))
                    instance = (Settings)serializer.Deserialize(inputStream);
            }
            catch
            {
                MessageBox.Show("Couldn't load settings, loading defaults");
            }
        }

        void flush_internal()
        {
            var serializer = new XmlSerializer(this.GetType());
            using (Stream outputStream = File.Create(fileName))
                serializer.Serialize(outputStream, this);
        }

        public static void flush()
        {
            instance.flush_internal();
        }

        public static Settings instance = new Settings();
        public static Settings Instance { get { return instance; } }
    }
}