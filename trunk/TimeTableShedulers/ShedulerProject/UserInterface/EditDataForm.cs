using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ShedulerProject.Core;

namespace ShedulerProject.UserInterface
{
    public partial class EditDataForm : Form
    {
        public enum Tab
        {
            Groups,
            Subjects,
            Rooms,
            Lecturers,
            Events
        }

        TimeTable data;

        public EditDataForm(TimeTable data)
        {
            InitializeComponent();
            this.data = data;
            FillComponents();
        }

        public void FillGrid<T>(DataGridView dataGrid, IEnumerable<T> source, Dictionary<string, Func<T, object>> fillRules)
        {
            foreach (T elem in source)
            {
                int colId = dataGrid.Rows.Add();
                DataGridViewRow newRow = dataGrid.Rows[colId];
                foreach (var rule in fillRules)
                {
                    newRow.Cells[rule.Key].Value = rule.Value(elem);
                }
            }
            dataGrid.AutoResizeColumns();

            //// Configure the details DataGridView so that its columns automatically 
            //// adjust their widths when the data changes.
            //detailsDataGridView.AutoSizeColumnsMode =
            //    DataGridViewAutoSizeColumnsMode.AllCells;
        }

        private void FillComponents()
        {
            // groups
            var groupsRules = new Dictionary<string, Func<Group, object>>()
            {
                {"colGroupId", g => g.Id},
                {"colGroupName", g => g.Name},
                {"colGroupCourse", g => g.Course}
            };
            FillGrid(gridGroups, data.Groups, groupsRules);

            //// groups
            //var groupsRules = new Dictionary<string, Func<Group, object>>()
            //{
            //    {"colGroupId", g => g.Id},
            //    {"colGroupName", g => g.Name},
            //    {"colGroupCourse", g => g.Course}
            //};
            //FillGrid(gridGroups, data.Groups, groupsRules);

            //// groups
            //var groupsRules = new Dictionary<string, Func<Group, object>>()
            //{
            //    {"colGroupId", g => g.Id},
            //    {"colGroupName", g => g.Name},
            //    {"colGroupCourse", g => g.Course}
            //};
            //FillGrid(gridGroups, data.Groups, groupsRules);

            //// groups
            //var groupsRules = new Dictionary<string, Func<Group, object>>()
            //{
            //    {"colGroupId", g => g.Id},
            //    {"colGroupName", g => g.Name},
            //    {"colGroupCourse", g => g.Course}
            //};
            //FillGrid(gridGroups, data.Groups, groupsRules);

            //// groups
            //var groupsRules = new Dictionary<string, Func<Group, object>>()
            //{
            //    {"colGroupId", g => g.Id},
            //    {"colGroupName", g => g.Name},
            //    {"colGroupCourse", g => g.Course}
            //};
            //FillGrid(gridGroups, data.Groups, groupsRules);
        }

        public void SetActiveTab(Tab tab)
        {
            switch (tab)
            {
                case Tab.Groups: tabControl.SelectTab(tabEditGroups); break;
                case Tab.Lecturers: tabControl.SelectTab(tabEditLecturers); break;
                case Tab.Rooms: tabControl.SelectTab(tabEditRooms); break;
                case Tab.Subjects: tabControl.SelectTab(tabEditSubjects); break;
                case Tab.Events: tabControl.SelectTab(tabEditEvents); break;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            DialogResult res = MessageBox.Show("Применить внесенные изменения?",
                                               "Составление расписания",
                                               MessageBoxButtons.YesNoCancel,
                                               MessageBoxIcon.Question);
            switch (res)
            {
                case DialogResult.Yes: ApplyChanges(); break;
                case DialogResult.Cancel: e.Cancel = true; break;
            }

            base.OnFormClosing(e);
        }

        public bool DataChanged { get; private set; }

        private void ApplyChanges()
        {
            DataChanged = true;
        }
    }
}
