using ColorPickerWPF;
using IAT.AddWindows;
using IAT.EditWindows;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using System.Xml.Linq;
using System.Xml.Serialization;

namespace IAT
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    /// 
    public static class Utilities
    {
        public static T @try<T>(Func<T> construct) where T : class
        {
            try
            {
                return construct();
            }
            catch
            {
                return default(T);
            }
        }

        public static bool try_show_message_if_fail(Action function, string message)
        {
            try
            {
                function();
                return true;
            }
            catch
            {
                MessageBox.Show(message);
                return false;
            }
        }
    }

public partial class AdminWindow : Window, INotifyPropertyChanged
    {

        public AdminWindow()
        {
            InitializeComponent();

            load_test_files();

            testList.ItemsSource = availableTests;
            DataContext = this;

            init_combo_boxes();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public ObservableCollection<Test> availableTests { get; set; } = new ObservableCollection<Test>();

        public Test selectedTest => (Test)testList.SelectedItem;
        public Block selectedBlock => (Block)blockList.SelectedItem;
        public Category selectedCategory => (Category)categoryList.SelectedItem;
        public Group selectedGroup => (Group)groupList.SelectedItem;
        public Term selectedTerm => (Term)termList.SelectedItem;

        public void flush()
        {
            var test = selectedTest;
            var serializer = new XmlSerializer(test.GetType());
            using (Stream outputStream = File.Create(test.test_file()))
                serializer.Serialize(outputStream, test);
        }

        void load_test_files()
        {
            string[] directories;

            try
            {
                directories = Directory.GetDirectories(Settings.testsFolder);
            }
            catch (DirectoryNotFoundException)
            {
                Directory.CreateDirectory(Settings.testsFolder.TrimEnd(new char[] {'/'}));
                directories = Directory.GetDirectories(Settings.testsFolder);
            }

            foreach (var directory in directories)
                availableTests.Add(Test.read_XML(directory + "/test.xml")); //make smarter 
        }

        void init_combo_boxes()
        {
            activeTestComboBox.SelectedIndex = availableTests.ToList().FindIndex(test => test.name == Settings.ActiveTest);
            practiceTestComboBox.SelectedItem = availableTests.ToList().Find(test => test.name == Settings.PracticeTest);
        }

        #region Selected
        public int selectedTestIndex = 0;
        public int selectedBlockIndex = 0;
        public int selectedCategoryIndex = 0;
        public int selectedGroupIndex = 0;
        public int selectedTermIndex = 0;
        #endregion

        private void sort_list(DataGrid list, string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(list.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }

        #region Adding
        void add_item<T>(ObservableCollection<T> list, T item, DataGrid listView)
        {
            try
            {
                list.Add(item);
                listView.SelectedIndex = listView.Items.Count - 1;
                flush();
            }
            catch
            {
                MessageBox.Show("Can't add item - No parent selected!");
            }
        }

        public void add_test(Test test)
        {
            availableTests.Add(test);
            test.flush();

            testList.SelectedIndex = testList.Items.Count - 1;
        }

        public void add_block(Block block) => add_item(selectedTest?.blocks, block, blockList);
        public void add_category(Category category) => add_item(selectedBlock?.categories, category, categoryList);
        public void add_group(Group group) => add_item(selectedCategory?.groups, group, groupList);
        public void add_term(Term term) => add_item(selectedGroup?.terms, term, termList);

        private void addTestButton_Click(object sender, RoutedEventArgs e) => new AddTestWindow(this).Show();
        private void addBlockButton_Click(object sender, RoutedEventArgs e) => new AddBlockWindow(this).Show();
        private void addCategoryButton_Click(object sender, RoutedEventArgs e) => new AddCategoryWindow(this).Show();
        private void addGroupButton_Click(object sender, RoutedEventArgs e) => new AddGroupWindow(this).Show();
        private void addTermButton_Click(object sender, RoutedEventArgs e) => new AddTermWindow(this).Show();

        private void addDescriptionBlock_Click(object sender, RoutedEventArgs e) => new AddDescriptionBlockWindow(this).Show();

        private void addTermListButton_Click(object sender, RoutedEventArgs e) => new AddTermListWindow(this).Show();
        #endregion

        #region Deleting

        bool confirm_delete()
        {
            return MessageBox.Show("Jeste li sigurni?", "Potvrda brisanja", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }
        void delete_test()
        {
            if (confirm_delete() == false)
                return;

            try
            {
                File.Delete(selectedTest.test_file());
            }
            catch
            {
                MessageBox.Show("Couldn't delete test file.");
            }

            try
            {
                availableTests.Remove(selectedTest);
            }
            catch
            {
                MessageBox.Show("Couldn't delete test entry");
            }
        }

        void delete_block()
        {
            if (confirm_delete() == false)
                return;

            //selected_test().blocks.Remove(selected_block());
            //selectedBlocks.ForEach(b => selectedTests[0].blocks.Remove(b));
            flush();
        }

        void delete_category()
        {
            if (confirm_delete() == false)
                return;

            //selected_block().categories.Remove(selected_category());
            //selectedCategories.ForEach(c => selectedBlocks[0].categories.Remove(c));
            flush();
        }

        void delete_group()
        {
            if (confirm_delete() == false)
                return;

            //selected_category().groups.Remove(selected_group());
            //selectedGroups.ForEach(g => selectedCategories[0].groups.Remove(g));
            flush();
        }

        void delete_term()
        {
            if (confirm_delete() == false)
                return;

            //selected_group().terms.Remove(selected_term());
            //selectedTerms.ForEach(t => selectedGroups[0].terms.Remove(t));
            flush();
        }        

        private void deleteTestButton_Click(object sender, RoutedEventArgs e) => delete_test();
        private void deleteBlockButton_Click(object sender, RoutedEventArgs e) => delete_block();
        private void deleteCategoryButton_Click(object sender, RoutedEventArgs e) => delete_category();
        private void deleteGroupButton_Click(object sender, RoutedEventArgs e) => delete_group();
        private void deleteTermButton_Click(object sender, RoutedEventArgs e) => delete_term();

        private void removeTestContextMenuItem_Click(object sender, RoutedEventArgs e) => delete_test();
        private void removeBlockContextMenuItem_Click(object sender, RoutedEventArgs e) => delete_block();
        private void removeCategoryContextMenuItem_Click(object sender, RoutedEventArgs e) => delete_category();
        private void removeGroupContextMenuItem_Click(object sender, RoutedEventArgs e) => delete_group();
        private void removeTermContextMenuItem_Click(object sender, RoutedEventArgs e) => delete_term();
        #endregion

        #region Editing
        void edit_test()
        {
            if(availableTests.Count > 0)
                new EditTestWindow(this).ShowDialog();
        }

        void edit_block()
        {
            if (selectedTest.blocks.Count > 0)
                new EditBlockWindow(this).ShowDialog();
        }

        void edit_category()
        {
            if(selectedBlock.categories.Count > 0)
                new EditCategoryWindow(this).ShowDialog();
        }

        void edit_group()
        {
            if(selectedCategory.groups.Count > 0)
                new EditGroupWindow(this).ShowDialog();
        }

        void edit_term()
        {
            if(selectedGroup.terms.Count > 0)
                new EditTermWindow(this).ShowDialog();
        }

        void flush_if_changed(DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
                flush();
        }

        private void testList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) => flush_if_changed(e);
        private void blockList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) => flush_if_changed(e);
        private void categoryList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) => flush_if_changed(e);
        private void groupList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) => flush_if_changed(e);
        private void termList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) => flush_if_changed(e);

        private void cellEditEnding(object sender, DataGridCellEditEndingEventArgs e) => flush_if_changed(e);

        private void editTestButton_Click(object sender, RoutedEventArgs e) => edit_test();
        private void editBlockButton_Click(object sender, RoutedEventArgs e) => edit_block();
        private void editCategoryButton_Click(object sender, RoutedEventArgs e) => edit_category();
        private void editGroupButton_Click(object sender, RoutedEventArgs e) => edit_group();
        private void editTermButton_Click(object sender, RoutedEventArgs e) => edit_term();

        private void editTestContextMenuItem_Click(object sender, RoutedEventArgs e) => edit_test();
        private void editBlockContextMenuItem_Click(object sender, RoutedEventArgs e) => edit_block();
        private void editCategoryContextMenuItem_Click(object sender, RoutedEventArgs e) => edit_category();
        private void editGroupContextMenuItem_Click(object sender, RoutedEventArgs e) => edit_group();
        private void editTermContextMenuItem_Click(object sender, RoutedEventArgs e) => edit_term();

        private void testList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => edit_test();
        private void blockList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => edit_block();
        private void categoryList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => edit_category();
        private void groupList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => edit_group();
        private void termList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => edit_term();
        #endregion

        #region Duplicating
        void duplicate_test()
        {
            var testCopy = (Test)selectedTest.Clone();
            testCopy.name = testCopy.name + "_copy";
            availableTests.Add(testCopy);

            testCopy.flush();
        }

        void duplicate_block()
        {
            var blockCopy = (Block)selectedBlock.Clone();
            blockCopy.name = blockCopy.name + "_copy";
            selectedTest.blocks.Add(blockCopy);

            flush();
        }

        void duplicate_category()
        {
            var categoryCopy = (Category)selectedCategory.Clone();
            categoryCopy.name = categoryCopy.name + "_copy";
            selectedBlock.categories.Add(categoryCopy);

            flush();
        }

        void duplicate_group()
        {
            var groupCopy = (Group)selectedGroup.Clone();
            groupCopy.name = groupCopy.name + "_copy";
            selectedCategory.groups.Add(groupCopy);

            flush();
        }

        void duplicate_term()
        {
            var termCopy = (Term)selectedTerm.Clone();
            termCopy.value = termCopy.value + "_copy";
            selectedGroup.terms.Add(termCopy);

            flush();
        }

        private void duplicateTestContextMenuItem_Click(object sender, RoutedEventArgs e) => duplicate_test();
        private void duplicateBlockContextMenuItem_Click(object sender, RoutedEventArgs e) => duplicate_block();
        private void duplicateCategoryContextMenuItem_Click(object sender, RoutedEventArgs e) => duplicate_category();
        private void duplicateGroupContextMenuItem_Click(object sender, RoutedEventArgs e) => duplicate_group();
        private void duplicateTermContextMenuItem_Click(object sender, RoutedEventArgs e) => duplicate_term();

        private void duplicateTestButton_Click(object sender, RoutedEventArgs e) => duplicate_test();
        private void duplicateBlockButton_Click(object sender, RoutedEventArgs e) => duplicate_block();
        private void duplicateCategoryButton_Click(object sender, RoutedEventArgs e) => duplicate_category();
        private void duplicateGroupButton_Click(object sender, RoutedEventArgs e) => duplicate_group();
        private void duplicateTermButton_Click(object sender, RoutedEventArgs e) => duplicate_term();

        void BlocksDuplicateCommand(object sender, ExecutedRoutedEventArgs e) => duplicate_block();
        void CategoriesDuplicateCommand(object sender, ExecutedRoutedEventArgs e) => duplicate_category();
        void GroupsDuplicateCommand(object sender, ExecutedRoutedEventArgs e) => duplicate_group();
        void TermsDuplicateCommand(object sender, ExecutedRoutedEventArgs e) => duplicate_term();
        #endregion

        #region CopyPasting
        List<Block> blockBuffer = new List<Block>();
        List<Category> categoryBuffer = new List<Category>();
        List<Group> groupBuffer = new List<Group>();
        List<Term> termBuffer = new List<Term>();

        void copy_blocks() => blockBuffer = blockList.SelectedItems.OfType<Block>().Select(b => (Block)b.Clone()).ToList();
        void copy_categories() => categoryBuffer = categoryList.SelectedItems.OfType<Category>().Select(c => (Category)c.Clone()).ToList();
        void copy_groups() => groupBuffer = groupList.SelectedItems.OfType<Group>().Select(g => (Group)g.Clone()).ToList();
        void copy_terms() => termBuffer = termList.SelectedItems.OfType<Term>().Select(t => (Term)t.Clone()).ToList();

        void paste_blocks() => blockBuffer.ForEach(block => selectedTest?.blocks?.Add(block));
        void paste_categories() => categoryBuffer.ForEach(category => selectedBlock?.categories?.Add(category));
        void paste_groups() => groupBuffer.ForEach(group => selectedCategory?.groups?.Add(group));
        void paste_terms() => termBuffer.ForEach(term => selectedGroup?.terms?.Add(term));

        private void copyBlockButton_Click(object sender, RoutedEventArgs e) => copy_blocks();
        private void copyCategoryButton_Click(object sender, RoutedEventArgs e) => copy_categories();
        private void copyGroupButton_Click(object sender, RoutedEventArgs e) => copy_groups();
        private void copyTermButton_Click(object sender, RoutedEventArgs e) => copy_terms();

        private void pasteBlockButton_Click(object sender, RoutedEventArgs e) => paste_blocks();
        private void pasteCategoryButton_Click(object sender, RoutedEventArgs e) => paste_categories();
        private void pasteGroupButton_Click(object sender, RoutedEventArgs e) => paste_groups();
        private void pasteTermButton_Click(object sender, RoutedEventArgs e) => paste_terms();

        void BlocksCopyCommand(object sender, ExecutedRoutedEventArgs e) => copy_blocks();
        void CategoriesCopyCommand(object sender, ExecutedRoutedEventArgs e) => copy_categories();
        void GroupsCopyCommand(object sender, ExecutedRoutedEventArgs e) => copy_groups();
        void TermsCopyCommand(object sender, ExecutedRoutedEventArgs e) => copy_terms();

        void BlocksPasteCommand(object sender, ExecutedRoutedEventArgs e) => paste_blocks();
        void CategoriesPasteCommand(object sender, ExecutedRoutedEventArgs e) => paste_categories();
        void GroupsPasteCommand(object sender, ExecutedRoutedEventArgs e) => paste_groups();
        void TermsPasteCommand(object sender, ExecutedRoutedEventArgs e) => paste_terms();

        #endregion

        #region KeyboardInput
        private void testList_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                delete_test();
        }

        private void blockList_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                delete_block();
        }

        private void categoryList_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                delete_category();
        }

        private void groupList_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                delete_group();
        }

        private void termList_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                delete_term();
        }
        #endregion

        #region ShowAll
        //void show_all_categories() => categoryList.ItemsSource = blockList.ItemsSource.Cast<Block>().SelectMany(b => b.categories);
        //void show_all_groups() => groupList.ItemsSource = categoryList.ItemsSource.Cast<Category>().SelectMany(c => c.groups);
        //void show_all_terms() => termList.ItemsSource = groupList.ItemsSource.Cast<Group>().SelectMany(g => g.terms);

        void show_all_categories() => categoryList.ItemsSource = selectedTest.blocks.SelectMany(b => b.categories);
        void show_all_groups() => groupList.ItemsSource = selectedBlock.categories.SelectMany(c => c.groups);
        void show_all_terms() => termList.ItemsSource = selectedCategory.groups.SelectMany(g => g.terms);

        private void showAllCategoriesButton_Click(object sender, RoutedEventArgs e) => show_all_categories();
        private void showAllGroupsButton_Click(object sender, RoutedEventArgs e) => show_all_groups();
        private void showAllTermsButton_Click(object sender, RoutedEventArgs e) => show_all_terms();
        #endregion

        private void calculateStatsButton_Click(object sender, RoutedEventArgs e)
        {
            TestStats.calculate(selectedTest.name);
            MessageBox.Show("Successfully calculated stats. Check the " + Serialization.testStatsFileName + " file.");
        }

        void calculate_from_existing_participant()
        {
            var dialog = new CommonOpenFileDialog();

            dialog.InitialDirectory = Directory.GetCurrentDirectory();
            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                TestRun.calculate_from_existing_data(dialog.FileName);

            MessageBox.Show("Successfully calculated stats. Check the " + Serialization.testStatsFileName + " file.");
        }

        private void calculateParticipantFromFileButton_Click(object sender, RoutedEventArgs e)
        {
            calculate_from_existing_participant();
        }
    }
}
