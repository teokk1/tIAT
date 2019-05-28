using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IAT
{
    public static class Extend
    {
        public static double StandardDeviation(this IEnumerable<double> values)
        {
            if (values.Count() < 1)
            {
                Console.WriteLine("No items for StandardDeviation. Returning 0 ");
                return 0;
            }

            double avg = values.Average();
            return Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));
        }
    }

    public static class Serialization
    {
        public const string dateFormat = "dd.MM.yyyy. HH:mm:ss";
        public const string fileDateFormat = "dd.MM.yyyy. HH:mm:ss";
        public const string fileSystemdateFormat = "yyyy_MM_dd_HH_mm_ss";

        public const string manualRecalculationFolder = "manual_recalculation/";

        public const string falseGuessesFile = "guesses.xml";
        public const string originalGuessesFile = "original_guesses.xml";
        public const string filteredGuessesFile = "filtered_guesses.xml";
        public const string adjustedGuessesFile = "adjusted_guesses.xml";

        public const string testsFolder = "Tests/";
        public const string participantsFolder = "participants/";

        public const string listsFolder = "lists/";
        public const string blocksFolder = "blocks/";

        public const string testStatsFileName = "test_stats.xml";
        public const string resultsFileName = "results.xml";
        public const string statsFileName = "stats.xml";
        public const string statsAdjustedFileName = "stats_adjusted.xml";
        public const string testTakenFileName = "test_taken.xml";
        public const string blockFile = "block.xml";
        public const string guessFile = "guesses.xml";
        public const string sdGroupsFileName = "sd_groups.xml";

        public static void serialize(Object o, string folderPath, string fileName)
        {
            var serializer = new XmlSerializer(o.GetType());
            using (Stream outputStream = File.Create(folderPath + fileName))
                serializer.Serialize(outputStream, o);
        }

        public static T deserialize<T>(T t, string fileName)
        {
            var serializer = new XmlSerializer(t.GetType());
            using (Stream inputStream = File.OpenRead(fileName))
                t = (T)serializer.Deserialize(inputStream);

            return t;
        }
    }

    [Serializable]
    public class Guess
    {
        [XmlElement(ElementName = "correct")]
        public bool correct { get; set; }

        [XmlElement(ElementName = "block")]
        public string block { get; set; } = "INVALID_BLOCK";

        [XmlElement(ElementName = "positivity")]
        public string positivity { get; set; } = "INVALID_POSITIVITY";

        [XmlElement(ElementName = "question_text")]
        public string question { get; set; } = "INVALID_QUESTION";
        
        [XmlElement(ElementName = "question_ordinal")]
        public int questionID { get; set; } = -1;

        [XmlElement(ElementName = "duration")]
        public double duration { get; set; } = -1;

        [XmlElement(ElementName = "timestamp")]
        public DateTime time { get; set; }

        [XmlElement(ElementName = "timeFormatted")]
        public string timeString { get; set; } = "INVALID_TIME";

        public Guess(bool correct, double duration, string block, string positivity, string question, int questionID)
        {
            this.correct = correct;
            this.duration = duration;
            this.block = block;
            this.positivity = positivity;
            this.question = question;
            this.questionID = questionID;

            this.time = DateTime.Now;
            timeString = time.ToString(Serialization.dateFormat);
        }

        public Guess()
        {

        }

        public Guess Clone()
        {
            Guess guess = new Guess();

            guess.correct = correct;

            guess.block = block;
            guess.positivity = positivity;

            guess.question = question;
            guess.questionID = questionID;

            guess.duration = duration;

            return guess;
        }

        [XmlIgnore] const string dateFormat = "dd.MM.yyyy. HH:mm:ss";
    }

    [Serializable]
    public class LatencyGroupStats
    {
        [XmlAttribute]
        public string subgroup { get; set; }

        [XmlElement(ElementName = "guessCount")]
        public int count { get; set; } = -1;

        [XmlElement(ElementName = "latencySum")]
        public double duration { get; set; } = -1;

        [XmlElement(ElementName = "mean")]
        public double mean { get; set; } = -1;

        [XmlElement(ElementName = "standardDeviation")]
        public double standardDeviation { get; set; } = -999;

        [XmlElement(ElementName = "minimum")]
        public double minimum { get; set; } = -1;

        [XmlElement(ElementName = "maximum")]
        public double maximum { get; set; } = -1;

        public void calculate(List<Guess> guesses)
        {
            count = guesses.Count;
            duration = guesses.Sum(g => g.duration);

            if(count > 0)
            {
                mean = duration / count;
                standardDeviation = guesses.Select(g => g.duration).StandardDeviation();
                minimum = guesses.Select(g => g.duration).DefaultIfEmpty().Min();
                maximum = guesses.Select(g => g.duration).DefaultIfEmpty().Max();
            }
        }

        LatencyGroupStats()
        {

        }

        public LatencyGroupStats(string subgroup)
        {
            this.subgroup = subgroup;
        }
    }

    public class RunLists
    {
        public List<Guess> correct = new List<Guess>();
        public List<Guess> incorrect = new List<Guess>();

        public List<Guess> positive = new List<Guess>();
        public List<Guess> negative = new List<Guess>();

        public List<Guess> positiveCorrect = new List<Guess>();
        public List<Guess> negativeCorrect = new List<Guess>();

        public List<Guess> positiveIncorrect = new List<Guess>();
        public List<Guess> negativeIncorrect = new List<Guess>();

        public void fill(List<Guess> guesses)
        {
            correct = guesses.FindAll(g => g.correct);
            incorrect = guesses.FindAll(g => g.correct == false);

            positive = guesses.FindAll(g => g.positivity == "Positive");
            negative = guesses.FindAll(g => g.positivity == "Negative");

            positiveCorrect = positive.FindAll(g => g.correct);
            negativeCorrect = negative.FindAll(g => g.correct);

            positiveIncorrect = positive.FindAll(g => g.correct == false);
            negativeIncorrect = negative.FindAll(g => g.correct == false);
        }

        public RunLists()
        {

        }

        public void serialize(string folderName)
        {
            string folder = folderName + Serialization.listsFolder;
            Directory.CreateDirectory(folder);

            Serialization.serialize(correct, folder, "correct.xml");
            Serialization.serialize(incorrect, folder, "incorrect.xml");

            Serialization.serialize(positive, folder, "positive.xml");
            Serialization.serialize(negative, folder, "negative.xml");

            Serialization.serialize(positiveCorrect, folder, "positiveCorrect.xml");
            Serialization.serialize(negativeCorrect, folder, "negativeCorrect.xml");

            Serialization.serialize(positiveIncorrect, folder, "positiveIncorrect.xml");
            Serialization.serialize(negativeIncorrect, folder, "negativeIncorrect.xml");
        }
    }

    public class RunStats
    {
        public LatencyGroupStats all = new LatencyGroupStats("All");

        public LatencyGroupStats correct = new LatencyGroupStats("Correct");
        public LatencyGroupStats incorrect = new LatencyGroupStats("Incorrect");

        public LatencyGroupStats positiveAll = new LatencyGroupStats("PositiveAll");
        public LatencyGroupStats negativeAll = new LatencyGroupStats("NegativeAll");

        public LatencyGroupStats positiveCorrect = new LatencyGroupStats("PositiveCorrect");
        public LatencyGroupStats negativeCorrect = new LatencyGroupStats("NegativeCorrect");

        public LatencyGroupStats positiveIncorrect = new LatencyGroupStats("PositiveIncorrect");
        public LatencyGroupStats negativeIncorrect = new LatencyGroupStats("NegativeIncorrect");

        public void calculate(List<Guess> originalList, RunLists lists)
        {
            all.calculate(originalList);

            correct.calculate(lists.correct);
            incorrect.calculate(lists.incorrect);

            positiveAll.calculate(lists.positive);
            negativeAll.calculate(lists.negative);

            positiveCorrect.calculate(lists.positiveCorrect);
            negativeCorrect.calculate(lists.negativeCorrect);

            positiveIncorrect.calculate(lists.positiveIncorrect);
            negativeIncorrect.calculate(lists.negativeIncorrect);
        }

        public RunStats()
        {

        }
    }

    public class RunPenalties
    {
        [XmlElement(ElementName = "penaltyAmount")]
        public double penaltyAmount { get; set; } = -1;

        [XmlElement(ElementName = "totalPenaltyDuration")]
        public double penaltyDuration { get; set; } = -1;

        [XmlElement(ElementName = "positivePenaltyDuration")]
        public double positivePenaltyDuration { get; set; } = -1;

        [XmlElement(ElementName = "negativePenaltyDuration")]
        public double negativePenaltyDuration { get; set; } = -1;

        public void calculate(RunStats stats, double penalty)
        {
            penaltyAmount = stats.correct.mean + penalty;
            penaltyDuration = penaltyAmount * stats.incorrect.count;

            positivePenaltyDuration = penaltyAmount * stats.positiveIncorrect.count;
            negativePenaltyDuration = penaltyAmount * stats.negativeIncorrect.count;
        }

        public RunPenalties()
        {

        }
    }

    public class BlockRun
    {
        #region OnCreation
        [XmlAttribute]
        public string blockName { get; set; }

        [XmlElement(ElementName = "startTime")]
        public DateTime startTime { get; set; }

        [XmlElement(ElementName = "startTimeFormatted")]
        public string startTimeString { get; set; }

        [XmlElement(ElementName = "congruence")]
        public string congruence { get; set; }

        [XmlElement(ElementName = "sdGroup")]
        public string sdGroup { get; set; }

        public RunStats stats = new RunStats();
        public RunPenalties penalties = new RunPenalties();
        #endregion

        #region DuringRuntime
        [XmlIgnore]
        public List<Guess> prefilteredGuesses { get; set; } = new List<Guess>(50);

        [XmlIgnore]
        public List<Guess> guesses { get; set; } = new List<Guess>();

        [XmlIgnore]
        public List<Guess> adjustedGuesses { get; set; } = new List<Guess>();
        #endregion

        [XmlIgnore]
        public RunLists lists = new RunLists();

        [XmlIgnore]
        public RunStats statsAdjusted = new RunStats();

        #region Creation
        public BlockRun(Block block)
        {
            blockName = block.name;
            congruence = block.congruence;
            sdGroup = block.sdGroup;

            startTime = DateTime.Now;
            startTimeString = startTime.ToString(Serialization.dateFormat);
        }

        public BlockRun()
        {

        }
        #endregion

        #region Runtime
        public void add_guess(bool correct, double duration, string positivity, string question, int questionId)
        {
            prefilteredGuesses.Add(new Guess(correct, duration, blockName, positivity, question, questionId));
        }

        #endregion

        #region Calculation
        void filter_times_over_max(double maxAllowedTime)
        {
            guesses = prefilteredGuesses.FindAll(g => g.duration < maxAllowedTime);
        }

        void calculate_adjusted()
        {
            adjustedGuesses = guesses.Select(g => g.Clone()).ToList();

            RunLists adjustedLists = new RunLists();
            adjustedLists.fill(adjustedGuesses);

            foreach(var guess in adjustedGuesses)
                if (guess.correct == false)
                    guess.duration = penalties.penaltyAmount;

            statsAdjusted.calculate(adjustedGuesses, adjustedLists);
        }

        public void calculate(double penalty, double maxAllowedTime)
        {
            filter_times_over_max(maxAllowedTime);
            lists.fill(guesses);
            stats.calculate(guesses, lists);
            penalties.calculate(stats, penalty);
            calculate_adjusted();
        }
        #endregion

        public void serialize(string folderName, int blockCount)
        {
            string folder = folderName + "block" + blockCount.ToString() + "/";

            Directory.CreateDirectory(folder);

            Serialization.serialize(this, folder, Serialization.blockFile);

            Serialization.serialize(prefilteredGuesses, folder, Serialization.originalGuessesFile);

            Serialization.serialize(guesses, folder, Serialization.filteredGuessesFile);
            Serialization.serialize(adjustedGuesses, folder, Serialization.adjustedGuessesFile);

            lists.serialize(folder);

            Serialization.serialize(stats, folder, Serialization.statsFileName);
            Serialization.serialize(statsAdjusted, folder, Serialization.statsAdjustedFileName);

            Serialization.serialize(penalties, folder, "penalties.xml");
        }
    }

    public class BlockRunGroup
    {
        [XmlIgnore]
        List<BlockRun> blockRuns = new List<BlockRun>();

        [XmlAttribute]
        public string name {get; set;} = "INVALID_NAME";   

        [XmlElement(ElementName = "BlockRuns")]
        public List<string> blockRunNames { get; set; } = new List<string>();
        
        [XmlElement(ElementName = "standardDeviationAdjusted")]
        public double standardDeviationAdjusted { get; set; } = -999;

        [XmlElement(ElementName = "standardDeviation")]
        public double standardDeviation {get; set;} = -999;

        [XmlElement(ElementName = "congruentMean")]
        public double congruentMean {get; set;} = -999;

        [XmlElement(ElementName = "incongruentMean")]
        public double incongruentMean {get; set;} = -999;

        [XmlElement(ElementName = "meanDifference")]
        public double meanDifference {get; set;} = -999;

        [XmlElement(ElementName = "result")]
        public double result { get; set; } = -999;


        void calculateSD()
        {
            standardDeviation = blockRuns.SelectMany(b => b.guesses).Select(guess => guess.duration).StandardDeviation();
            standardDeviationAdjusted = blockRuns.SelectMany(b => b.adjustedGuesses).Select(guess => guess.duration).StandardDeviation();
        }

        void calculateMeans()
        {
            congruentMean = blockRuns.FindAll(b => b.congruence == "Congruent").SelectMany(b => b.adjustedGuesses).Select(g => g.duration).DefaultIfEmpty(-999).Average();
            incongruentMean = blockRuns.FindAll(b => b.congruence == "Incongruent").SelectMany(b => b.adjustedGuesses).Select(g => g.duration).DefaultIfEmpty(-999).Average();
        }

        void calculate_mean_difference()
        {
            meanDifference = incongruentMean - congruentMean;
        }

        void calculate_result()
        {
            result = meanDifference / standardDeviation;
        }

        void calculate()
        {
            calculateSD();
            calculateMeans();
            calculate_mean_difference();
            calculate_result();
        }

        public BlockRunGroup()
        {

        }

        public BlockRunGroup(string name, List<BlockRun> allBlockRuns)
        {
            this.name = name;

            blockRuns = allBlockRuns.FindAll(b => b.sdGroup == name);
            blockRunNames = blockRuns.Select(b => b.blockName).ToList();

            calculate();
        }
    }

    public class BlockRunGroups
    {
        [XmlElement(ElementName = "group")]
        public List<BlockRunGroup> groups { get; set; } = new List<BlockRunGroup>();

        [XmlElement(ElementName = "FinalResult")]
        public double finalResult { get; set; } = -999;

        void calculate()
        {
            finalResult = groups.Average(g => g.result);
        }

        public BlockRunGroups()
        {

        }

        public BlockRunGroups(List<BlockRun> allBlockRuns)
        {
            List<string> groupNames = allBlockRuns.Select(b => b.sdGroup).Distinct().Where(sdGroup => sdGroup != null && sdGroup != "").ToList();

            foreach (var groupName in groupNames)
                groups.Add(new BlockRunGroup(groupName, allBlockRuns));

            calculate();
        }
    }

    [Serializable]
    [XmlRoot("TestRun")]
    public class TestRun
    {
        #region SeparateSerialize
        [XmlIgnore]
        Test testTaken;

        [XmlIgnore]
        List<BlockRun> blockRuns = new List<BlockRun>();

        [XmlIgnore]
        List<Guess> allGuesses = new List<Guess>();

        [XmlIgnore]
        RunLists lists = new RunLists();

        [XmlIgnore]
        RunStats stats = new RunStats();

        [XmlIgnore]
        BlockRunGroups blockRunGroups = new BlockRunGroups();
        #endregion

        #region OnCreation
        [XmlElement(ElementName = "takerId")]
        public string takerId { get; set; }

        [XmlElement(ElementName = "startTime")]
        public DateTime startTime { get; set; }

        [XmlElement(ElementName = "startTimeFormatted")]
        public string startTimeString { get; set; }

        [XmlElement(ElementName = "PC_NAME")]
        public string pcName { get; set; }
        #endregion

        RunPenalties penalties = new RunPenalties();

        //Validity
        [XmlElement(ElementName = "percentageLatenciesBelowMin")]
        public double percentageLatenciesBelowMin { get; set; } = -1;

        [XmlElement(ElementName = "percentageLatenciesAboveMax")]
        public double percentageLatenciesAboveMax { get; set; } = -1;

        [XmlElement(ElementName = "valid")]
        public bool valid { get; set; } = true;

        [XmlElement(ElementName = "passedQuestionMinCheck")]
        public bool passedQuestionMinCheck { get; set; } = false;

        [XmlElement(ElementName = "passedQuestionMaxCheck")]
        public bool passedQuestionMaxCheck { get; set; } = false;

        [XmlElement(ElementName = "passedTotalTimeMinCheck")]
        public bool passedTotalTimeMinCheck { get; set; } = false;

        [XmlElement(ElementName = "passedTotalTimeMaxCheck")]
        public bool passedTotalTimeMaxCheck { get; set; } = false;

        [XmlElement(ElementName = "finalResult")]
        public double finalResult { get; set; } = -999;

        #region Creation
        public TestRun(Test test, string takerId)
        {
            this.testTaken = test;
            this.takerId = takerId;

            startTime = DateTime.Now;
            startTimeString = startTime.ToString(Serialization.fileDateFormat);
            pcName = Environment.MachineName;

            blockRuns = new List<BlockRun>();
        }

        public TestRun()
        {

        }
        #endregion

        #region Runtime
        public void add_block(Block block)
        {
            if(block.categories.Count > 0 && block.isDescription == false || block.trialCount > 1)
                blockRuns.Add(new BlockRun(block));
        }
        public void add_guess(bool correct, double duration, string positivity, string question, int questionId)
        {

            blockRuns.Last().add_guess(correct, duration, positivity, question, questionId);
        }
        #endregion

        //Validity
        void determine_percentages(List<Guess> guesses, double minLatency, double maxLatency)
        {
            percentageLatenciesBelowMin = guesses.Select(g => g.duration).ToList().FindAll(d => d < minLatency).Count() / (double)stats.all.count;
            percentageLatenciesAboveMax = guesses.Select(g => g.duration).ToList().FindAll(d => d > maxLatency).Count() / (double)stats.all.count;
        }

        bool min_question_check()
        {
            return percentageLatenciesBelowMin < testTaken.allowedPercentageBelowMinQuestionTime / 100d;
        }

        bool max_question_check()
        {
            return percentageLatenciesAboveMax < testTaken.allowedPercentageAboveMaxQuestionTime / 100d;
        }

        bool total_time_min_check()
        {
            return stats.all.duration > testTaken.minTotalTime * 1000d;
        }

        bool total_time_max_check()
        {
            return stats.all.duration < testTaken.maxTotalTime * 1000d;
        }

        bool check_validity()
        {
            return passedQuestionMinCheck && passedQuestionMaxCheck && passedTotalTimeMinCheck && passedTotalTimeMaxCheck;
        }

        #region Calculation
        void perform_block_calculations() => blockRuns.ForEach(b => b.calculate(testTaken.mistakePenalty, testTaken.maxQuestionTimeBeforeIgnore));

        void validate()
        {
            passedQuestionMinCheck = min_question_check();
            passedQuestionMaxCheck = max_question_check();

            passedTotalTimeMinCheck = total_time_min_check();
            passedTotalTimeMaxCheck = total_time_max_check();

            valid = check_validity();
        }

        void calculate()
        {
            perform_block_calculations();

            allGuesses = blockRuns.SelectMany(b => b.guesses).ToList();

            lists.fill(allGuesses);
            stats.calculate(allGuesses, lists);
            penalties.calculate(stats, testTaken.mistakePenalty);
            determine_percentages(allGuesses, testTaken.minQuestionTime, testTaken.maxQuestionTime);
            blockRunGroups = new BlockRunGroups(blockRuns);

            finalResult = blockRunGroups.finalResult;

            validate();
        }
        #endregion

        #region Serialization
        void serialize_guesses(string resultFolder)
        {
            Serialization.serialize(allGuesses, resultFolder, Serialization.guessFile);
        }

        void serialize_blockRuns(string resultFolder)
        {
            var folder = resultFolder + Serialization.blocksFolder;
            for (int i = 0; i < blockRuns.Count; i++)
                blockRuns[i].serialize(folder, i);
        }

        void serialize_test(string resultFolder)
        {
            Serialization.serialize(testTaken, resultFolder, Serialization.testTakenFileName);
        }

        void serialize_run_lists(string resultFolder)
        {
            lists.serialize(resultFolder);
        }

        void serialize_stats(string resultFolder)
        {
            Serialization.serialize(stats, resultFolder, Serialization.statsFileName);
        }

        void serialize_sd_groups(string resultFolder)
        {
            Serialization.serialize(blockRunGroups, resultFolder, Serialization.sdGroupsFileName);
        }

        void serialize_other_elements(string resultFolder)
        {
            serialize_guesses(resultFolder);
            serialize_blockRuns(resultFolder);
            serialize_test(resultFolder);
            serialize_run_lists(resultFolder);
            serialize_stats(resultFolder);
            serialize_sd_groups(resultFolder);
        }

        void serialize_this(string resultFolder)
        {
            Directory.CreateDirectory(resultFolder);
            Serialization.serialize(this, resultFolder, Serialization.resultsFileName);
        }

        public void write_internal(string folderName)
        {
            var resultFolder = testTaken.test_folder() + Serialization.participantsFolder + folderName;

            serialize_this(resultFolder);
            serialize_other_elements(resultFolder);
        }

        string participant_folder()
        {
            return startTime.ToString(Serialization.fileSystemdateFormat) + "_" + takerId + "_" + pcName + "/";
        }

        public void write()
        {
            calculate();

            var folderName = (valid ? "" : "X_") + participant_folder();
            write_internal(folderName);
        }

        public void write_incomplete()
        {
            var folderName = ("NC_") + participant_folder();
            write_internal(folderName);
        }
        #endregion

        static List<BlockRun> read_block_runs(string participantFolder)
        {
            List<BlockRun> blockRuns = new List<BlockRun>();

            string folder = participantFolder + Serialization.blocksFolder;

            foreach (var directory in Directory.GetDirectories(folder))
            {
                var blockFile = directory + "/" + Serialization.blockFile;
                var blockRun = Serialization.deserialize(new BlockRun(), blockFile);

                var guessesFile = directory + "/" + Serialization.originalGuessesFile;
                blockRun.prefilteredGuesses = Serialization.deserialize(new List<Guess>(), guessesFile);

                blockRuns.Add(blockRun);
            }

            return blockRuns;
        }

        public static void calculate_from_existing_data(string participantFolder)
        {
            participantFolder = participantFolder + "/";
            var test = Serialization.deserialize(new Test(), participantFolder + Serialization.testTakenFileName);

            TestRun testRun = new TestRun(test, "MANUAL TEST RECALCULATION");
            testRun.blockRuns = read_block_runs(participantFolder);
            testRun.calculate();

            var resultFolder = Serialization.manualRecalculationFolder + test.test_folder();
            testRun.serialize_this(resultFolder);
            testRun.serialize_other_elements(resultFolder);
        }
    }
}