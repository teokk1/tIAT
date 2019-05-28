using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace IAT
{
    [Serializable]
    public class TestRuns
    {
        [XmlElement(ElementName = "Run")]
        public List<TestRun> list { get; set; } = new List<TestRun>();

        public void Add(TestRun run) => list.Add(run);
        public TestRuns()
        {

        }
    }

    [Serializable]
    public class IATScores
    {
        [XmlElement(ElementName = "Score")]
        public List<double> list { get; set; } = new List<double>();

        public void Add(double score) => list.Add(score);
        public IATScores()
        {

        }
    }

    [Serializable]
    [XmlRoot("TestStats")]
    public class TestStats
    {
        [XmlAttribute]
        public string name { get; set; } = "INVALID_NAME";

        [XmlElement(ElementName = "IAT_Scores")]
        public IATScores iatScores {get; set;} = new IATScores();

        [XmlElement(ElementName = "TestRuns")]
        public TestRuns testRuns { get; set; } = new TestRuns();

        TestStats()
        {

        }

        TestStats(string name)
        {
            this.name = name;
        }

        void read_data()
        {
            string folder = Serialization.testsFolder + name + "/" + Serialization.participantsFolder;
            List<string> directories = Directory.GetDirectories(folder).ToList().FindAll(f => f.StartsWith("X") == false && f.StartsWith("NC") == false).ToList();
            
            foreach(var directory in directories)
            {
                var filePath = directory + "/" + Serialization.resultsFileName;
                var testRun = Serialization.deserialize(new TestRun(), filePath);

                testRuns.Add(testRun);
                iatScores.Add(testRun.finalResult);
            }
        }

        public static void calculate(string name)
        {
            var stats = new TestStats(name);

            stats.read_data();

            Serialization.serialize(stats, Serialization.testsFolder + name + "/", Serialization.testStatsFileName);
        }
    }


    [Serializable]
    [XmlRoot("test")]
    public class Test : ICloneable
    {
        public static Test read_XML(string testName)
        {
            Test test = new Test();

            try
            {
                var serializer = new XmlSerializer(test.GetType());
                using (Stream inputStream = File.OpenRead(testName))
                    test = (Test)serializer.Deserialize(inputStream);

                test.link_terms_with_groups();
                test.blocks = new ObservableCollection<Block>(test.blocks.OrderBy(block => block.id));
                test.update_congruence();
            }
            catch
            {
                MessageBox.Show
                (
                    "Couldn't open test " + testName + " even though the folder exists." +
                    "Test folders don't get deleted when you delete a test so you don't lose the test data. If you don't want to see this message again, " +
                    "backup your test data and delete the /Tests/" + testName + "/ folder manually."
                );
            }

            return test;
        }

        void link_terms_with_groups()
        {
            foreach (var block in blocks)
                foreach (var category in block.categories)
                    foreach (var group in category.groups)
                        foreach (var term in group.terms)
                            term.group = group;
        }

        void update_congruence()
        {
            foreach (var block in blocks)
                block.determine_congruence();
        }

        public void flush()
        {
            update_congruence();

            Directory.CreateDirectory(test_folder(name));

            var serializer = new XmlSerializer(GetType());
            using (Stream outputStream = File.Create(test_file(name)))
                serializer.Serialize(outputStream, this);
        }

        public static void flush(Test test)
        {
            test.flush();
        }

        public static string test_file(string name)
        {
            return Settings.testsFolder + name + "/test.xml";
        }

        public static string test_folder(string name)
        {
            return Settings.testsFolder + name + "/";
        }

        public string test_file()
        {
            return Settings.testsFolder + name + "/test.xml";
        }

        public string test_folder()
        {
            return Settings.testsFolder + name + "/";
        }

        //Make all autoflush
        //public string Name { get { return name; } set { name = value; flush(); } }
        [XmlAttribute]
        public string name { get; set; } = "Invalid Name";

        //public double MinQuestionTime { get { return minQuestionTime; } set { minQuestionTime = value; flush(); } }
        [XmlAttribute]
        public double minQuestionTime { get; set; } = Settings.DefaultMinQuestionTime;

        [XmlAttribute]
        public double allowedPercentageBelowMinQuestionTime { get; set; } = Settings.DefaultAllowedPercentageBelowMinQuestionTime;

        [XmlAttribute]
        public double maxQuestionTime { get; set; } = Settings.DefaultMaxQuestionTime;

        [XmlAttribute]
        public double maxQuestionTimeBeforeIgnore { get; set; } = Settings.DefaultMaxQuestionTime;

        [XmlAttribute]
        public double allowedPercentageAboveMaxQuestionTime { get; set; } = Settings.DefaultAllowedPercentageAboveMaxQuestionTime;

        [XmlAttribute]
        public double minTotalTime { get; set; } = Settings.DefaultMinTotalTime;

        [XmlAttribute]
        public double maxTotalTime { get; set; } = Settings.DefaultMaxTotalTime;

        [XmlAttribute]
        public double mistakePenalty { get; set; } = Settings.DefaultMistakePenalty;

        #region Tresholds
        [XmlAttribute]
        public double slightTreshold { get; set; } = Settings.DefaultSlightTreshold;

        [XmlAttribute]
        public double moderateTreshold { get; set; } = Settings.DefaultModerateTreshold;

        [XmlAttribute]
        public double strongTreshold { get; set; } = Settings.DefaultStrongTreshold;
        #endregion

        [XmlElement(ElementName = "sdGroups")]
        public List<string> sdGroups { get; set; } = new List<string>();

        #region ResultStrings
        [XmlElement(ElementName = "noBiasString")]
        public string noBiasString { get; set; } = "Nemate nikakvu preferenciju";

        [XmlElement(ElementName = "slightNegativeString")]
        public string slightNegativeString { get; set; } = "Imate blago negativnu preferenciju";

        [XmlElement(ElementName = "moderateNegativeString")]
        public string moderateNegativeString { get; set; } = "Imate osrednje negativnu preferenciju";

        [XmlElement(ElementName = "strongNegativeString")]
        public string strongNegativeString { get; set; } = "Imate jako negativnu preferenciju";

        [XmlElement(ElementName = "slightPositiveString")]
        public string slightPositiveString { get; set; } = "Imate blago pozitivnu preferenciju";

        [XmlElement(ElementName = "moderatePositiveString")]
        public string moderatePositiveString { get; set; } = "Imate osrednje pozitivnu preferenciju";

        [XmlElement(ElementName = "strongPositiveString")]
        public string strongPositiveString { get; set; } = "Imate jako pozitivnu preferenciju";
        #endregion

        [XmlElement(ElementName = "description")]
        public string description { get; set; } = "Test Description Here";

        [XmlElement(ElementName = "testCompleteMessage")]
        public string completeMessage { get; set; } = "Test Complete!";

        [XmlElement(ElementName = "blocks")]
        public ObservableCollection<Block> blocks { get; set; } = new ObservableCollection<Block>();

        public Test()
        {

        }

        public override string ToString()
        {
            return name;
        }

        public Object Clone()
        {
            var test = new Test();

            test.name = name;
            test.description = description;

            test.minQuestionTime = minQuestionTime;
            test.maxQuestionTime = maxQuestionTime;

            test.maxQuestionTimeBeforeIgnore = maxQuestionTimeBeforeIgnore;

            test.minTotalTime = minTotalTime;
            test.maxTotalTime = maxTotalTime;

            test.mistakePenalty = mistakePenalty;

            test.slightTreshold = slightTreshold;
            test.moderateTreshold = moderateTreshold;
            test.strongTreshold = strongTreshold;

            test.sdGroups = sdGroups.ToList();

            test.slightNegativeString = slightNegativeString;
            test.slightPositiveString = slightPositiveString;

            test.moderateNegativeString = moderateNegativeString;
            test.moderatePositiveString = moderatePositiveString;

            test.strongNegativeString = strongNegativeString;
            test.strongPositiveString = strongPositiveString;

            test.completeMessage = completeMessage;

            test.blocks = new ObservableCollection<Block>(blocks.Select(objEntity => (Block)objEntity.Clone()).ToList());

            return test;
        }
    }

    public class Block : ICloneable
    {
        [XmlAttribute]
        public string name { get; set; } = "Invalid Name";

        [XmlAttribute]
        public int trialCount { get; set; } = 10;

        [XmlAttribute]
        public int id { get; set; } = -1;

        [XmlAttribute]
        public string sdGroup { get; set; } = "";

        [XmlAttribute]
        public string shuffleGroup { get; set; } = "";

        [XmlElement(ElementName = "isDescription")]
        public bool isDescription { get; set; } = false;

        [XmlElement(ElementName = "description")] 
        public string description { get; set; } = "Invalid Description";

        [XmlElement(ElementName = "congruence")]
        public string congruence { get; set; } = "No Congruence";

        [XmlElement(ElementName = "category")]
        public ObservableCollection<Category> categories { get; set; } = new ObservableCollection<Category>();

        bool is_incongruent(string side)
        {
            var groups = categories.ToList().SelectMany(category => category.groups).ToList().FindAll(group => group.side == side);

            if (groups.Count == 0)
                return false;

            return groups.Any(g => g.positivity != groups[0].positivity);
        }

        bool is_incongruent()
        {
            return is_incongruent("Left") || is_incongruent("Right");
        }

        public void determine_congruence()
        {
            congruence = is_incongruent() ? "Incongruent" : "Congruent";
        }

        public Block(string name, int id)
        {
            this.name = name;
            this.id = id;
        }

        public Block()
        {

        }

        public Object Clone()
        {
            var block = new Block();

            block.name = name;

            block.isDescription = isDescription;
            block.description = description;

            block.trialCount = trialCount;

            block.id = id;
            block.sdGroup = sdGroup;
            block.shuffleGroup = shuffleGroup;

            block.congruence = congruence;

            block.categories = new ObservableCollection<Category>(categories.Select(objEntity => (Category)objEntity.Clone()).ToList());

            return block;
        }
    }

    public class Category : ICloneable
    {
        [XmlAttribute]
        public string name { get; set; } = "Invalid Name";

        [XmlElement(ElementName = "group")]
        public ObservableCollection<Group> groups { get; set; } = new ObservableCollection<Group>();

        public Category(string name)
        {
            this.name = name;
        }

        public Category()
        {

        }

        public Object Clone()
        {
            var category = new Category();
            category.name = name;
            category.groups = new ObservableCollection<Group>(groups.Select(objEntity => (Group)objEntity.Clone()).ToList());

            return category;
        }
    }

    public class Group : ICloneable
    {
        [XmlAttribute]
        public string name { get; set; } = "Invalid Name";

        [XmlAttribute]
        public string side { get; set; } = "Invalid Side";

        [XmlAttribute]
        public string positivity { get; set; } = "Invalid Positivity";

        [XmlAttribute]
        public string hexColor { get; set; } = "#000000";

        [XmlIgnore]
        public Color Color { get { return (Color)ColorConverter.ConvertFromString(hexColor); } set { hexColor = value.ToString(); } }

        [XmlIgnore]
        public SolidColorBrush ColorBrush { get { return new SolidColorBrush(Color); } set { hexColor = value.Color.ToString(); } }        

        [XmlElement(ElementName = "term")]
        public ObservableCollection<Term> terms { get; set; }  = new ObservableCollection<Term>();

        public Group(string name, string side, string positivity)
        {
            this.name = name;
            this.side = side;
            this.positivity = positivity;

            hexColor = Settings.DefaultListColor.ToString();
        }

        public Group()
        {
            hexColor = Settings.DefaultListColor.ToString();
        }

        public Object Clone()
        {
            var group = new Group();
            group.name = name;
            group.side = side;
            group.positivity = positivity;

            group.hexColor = hexColor;
            group.terms = new ObservableCollection<Term>(terms.Select(objEntity => (Term)objEntity.Clone()).ToList());

            return group;
        }
    }

    public class Term
    {
        [XmlText]
        public string value { get; set; } = "Invalid Value";

        [XmlIgnore]
        public Group group { get; set; }

        [XmlAttribute]
        public bool isImage { get; set; } = false;

        public Term(string value, Group group)
        {
            this.value = value;
            this.group = group;
        }

        public Term()
        {

        }

        public Object Clone()
        {
            var term = new Term();
            term.value = value;
            term.group = group;
            term.isImage = isImage;
            return term;
        }
    }
}
