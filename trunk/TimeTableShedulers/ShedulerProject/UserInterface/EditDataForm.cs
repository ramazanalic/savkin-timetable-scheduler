using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SchedulerProject.Core;

namespace SchedulerProject.UserInterface
{
    public partial class EditDataForm : Form
    {
        public const string UNDEFINED_COMBOBOX_VALUE = "Не выбрано";

        #region Construction routines

        SchedulingPrimitivesGrigView<Group> gridGroups;
        SchedulingPrimitivesGrigView<Subject> gridSubjects;
        SchedulingPrimitivesGrigView<Room> gridRooms;
        SchedulingPrimitivesGrigView<Lecturer> gridLecturers;
        SchedulingPrimitivesGrigView<Event> gridEvents;

       Label label1 = new Label();
       MenuStrip menuStrip1 = new MenuStrip();
       ToolStripMenuItem applyChangesToolStripMenuItem = new ToolStripMenuItem();

       TabControl tabControl = new TabControl();
       TabPage tabEditGroups = new TabPage(),
               tabEditLecturers = new TabPage(),
               tabEditSubjects = new TabPage(),
               tabEditRooms = new TabPage(),
               tabEditEvents = new TabPage();
       Panel panelEditEvents = new Panel();
       ComboBox cbxSubjectFilter = new ComboBox();

       DataGridViewTextBoxColumn colGroupName = new DataGridViewTextBoxColumn();
       DataGridViewComboBoxColumn colGroupCourse = new DataGridViewComboBoxColumn();
       DataGridViewTextBoxColumn colSubjectName = new DataGridViewTextBoxColumn();
       DataGridViewComboBoxColumn colSubjectLecturer = new DataGridViewComboBoxColumn();
       DataGridViewComboBoxColumn colSubjectDifficulty = new DataGridViewComboBoxColumn();
       DataGridViewComboBoxColumn colEventSubject = new DataGridViewComboBoxColumn();
       DataGridViewComboBoxColumn colEventLecturer = new DataGridViewComboBoxColumn();
       DataGridViewComboBoxColumn colEventType = new DataGridViewComboBoxColumn();
       DataGridViewTextBoxColumn colEventGroups = new DataGridViewTextBoxColumn();
       DataGridViewTextBoxColumn colLecturerName = new DataGridViewTextBoxColumn();
       DataGridViewButtonColumn colLecturerTimeConstraints = new DataGridViewButtonColumn();
       DataGridViewTextBoxColumn colRoomNumber = new DataGridViewTextBoxColumn();
       DataGridViewTextBoxColumn colRoomHousing = new DataGridViewTextBoxColumn();
       DataGridViewComboBoxColumn colRoomType = new DataGridViewComboBoxColumn();
       DataGridViewButtonColumn colRoomTimeConstraints = new DataGridViewButtonColumn();

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {                      
            tabControl.SuspendLayout();
            tabEditEvents.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabEditGroups);
            tabControl.Controls.Add(tabEditLecturers);
            tabControl.Controls.Add(tabEditSubjects);
            tabControl.Controls.Add(tabEditRooms);
            tabControl.Controls.Add(tabEditEvents);
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new Point(0, 24);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(708, 538);
            tabControl.TabIndex = 2;
            // 
            // tabEditGroups
            // 
            tabEditGroups.Location = new Point(4, 22);
            tabEditGroups.Name = "tabEditGroups";
            tabEditGroups.Padding = new Padding(3);
            tabEditGroups.Size = new Size(700, 512);
            tabEditGroups.TabIndex = 0;
            tabEditGroups.Text = "Группы";
            tabEditGroups.UseVisualStyleBackColor = true;
            // 
            // tabEditLecturers
            // 
            tabEditLecturers.Location = new Point(4, 22);
            tabEditLecturers.Name = "tabEditLecturers";
            tabEditLecturers.Padding = new Padding(3);
            tabEditLecturers.Size = new Size(700, 512);
            tabEditLecturers.TabIndex = 3;
            tabEditLecturers.Text = "Преподаватели";
            tabEditLecturers.UseVisualStyleBackColor = true;
            // 
            // tabEditSubjects
            // 
            tabEditSubjects.Location = new Point(4, 22);
            tabEditSubjects.Name = "tabEditSubjects";
            tabEditSubjects.Padding = new Padding(3);
            tabEditSubjects.Size = new Size(700, 512);
            tabEditSubjects.TabIndex = 1;
            tabEditSubjects.Text = "Предметы";
            tabEditSubjects.UseVisualStyleBackColor = true;
            // 
            // tabEditRooms
            // 
            tabEditRooms.Location = new Point(4, 22);
            tabEditRooms.Name = "tabEditRooms";
            tabEditRooms.Padding = new Padding(3);
            tabEditRooms.Size = new Size(700, 512);
            tabEditRooms.TabIndex = 2;
            tabEditRooms.Text = "Аудитории";
            tabEditRooms.UseVisualStyleBackColor = true;          
            // 
            // tabEditEvents
            // 
            tabEditEvents.Controls.Add(cbxSubjectFilter);
            tabEditEvents.Controls.Add(label1);
            tabEditEvents.Controls.Add(panelEditEvents);
            tabEditEvents.Location = new Point(4, 22);
            tabEditEvents.Name = "tabEditEvents";
            tabEditEvents.Padding = new Padding(3);
            tabEditEvents.Size = new Size(700, 512);
            tabEditEvents.TabIndex = 4;
            tabEditEvents.Text = "Занятия";
            tabEditEvents.UseVisualStyleBackColor = true;
            // 
            // cbxSubjectFilter
            // 
            cbxSubjectFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxSubjectFilter.FormattingEnabled = true;
            cbxSubjectFilter.Location = new Point(117, 6);
            cbxSubjectFilter.Name = "cbxSubjectFilter";
            cbxSubjectFilter.Size = new Size(208, 21);
            cbxSubjectFilter.TabIndex = 3;
            cbxSubjectFilter.SelectedValue = EditDataForm.UNDEFINED_COMBOBOX_VALUE;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(8, 9);
            label1.Name = "label1";
            label1.Size = new Size(103, 13);
            label1.TabIndex = 1;
            label1.Text = "Выберите предмет";
            //
            // panelEditEvents
            //
            panelEditEvents.Anchor =  AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelEditEvents.Size = tabEditEvents.Size;
            panelEditEvents.Location = new Point(0, cbxSubjectFilter.Bottom + 10);  
            // 
            // colGroupName
            // 
            colGroupName.HeaderText = "Номер";
            colGroupName.Name = "colGroupName";
            // 
            // colGroupCourse
            // 
            colGroupCourse.HeaderText = "Курс";
            colGroupCourse.Items.AddRange(EditDataForm.UNDEFINED_COMBOBOX_VALUE, "1", "2", "3", "4", "5", "6");
            colGroupCourse.Name = "colGroupCourse";
            colGroupCourse.Resizable = DataGridViewTriState.True;
            colGroupCourse.SortMode = DataGridViewColumnSortMode.Automatic;
            colGroupCourse.AutoComplete = true;
            colGroupCourse.FlatStyle = FlatStyle.Flat;
            // 
            // colSubjectName
            // 
            colSubjectName.HeaderText = "Название";
            colSubjectName.Name = "colSubjectName";
            // 
            // colSubjectLecturer
            // 
            colSubjectLecturer.HeaderText = "Преподаватель";
            colSubjectLecturer.Name = "colSubjectLecturer";
            colSubjectLecturer.Resizable = DataGridViewTriState.True;
            colSubjectLecturer.SortMode = DataGridViewColumnSortMode.Automatic;
            colSubjectLecturer.Items.AddRange(EditDataForm.UNDEFINED_COMBOBOX_VALUE);
            colSubjectLecturer.AutoComplete = true;
            colSubjectLecturer.FlatStyle = FlatStyle.Flat;
            // 
            // colSubjectDifficulty
            // 
            colSubjectDifficulty.HeaderText = "Сложность";
            colSubjectDifficulty.Name = "colSubjectDifficulty";
            colSubjectDifficulty.Items.AddRange(EditDataForm.UNDEFINED_COMBOBOX_VALUE, "1", "2", "3");
            colSubjectDifficulty.AutoComplete = true;
            colSubjectDifficulty.FlatStyle = FlatStyle.Flat;
            //
            // colEventSubject
            //
            colEventSubject.HeaderText = "Предмет";
            colEventSubject.Name = "colEventSubject";
            colEventSubject.Items.AddRange(EditDataForm.UNDEFINED_COMBOBOX_VALUE);
            colEventSubject.AutoComplete = true;
            colEventSubject.FlatStyle = FlatStyle.Flat;
            colEventSubject.ReadOnly = true;
            colEventSubject.Visible = false;
            // 
            // colEventLecturer
            // 
            colEventLecturer.HeaderText = "Преподаватель";
            colEventLecturer.Name = "colEventLecturer";
            colEventLecturer.Items.AddRange(EditDataForm.UNDEFINED_COMBOBOX_VALUE);
            colEventLecturer.AutoComplete = true;
            colEventLecturer.FlatStyle = FlatStyle.Flat;
            // 
            // colEventType
            // 
            colEventType.HeaderText = "Тип";
            colEventType.Name = "colEventType";
            colEventType.Items.AddRange(
                EditDataForm.UNDEFINED_COMBOBOX_VALUE, 
                RoomType.Laboratory.ToString(),
                RoomType.Lecture.ToString(),
                RoomType.Practice.ToString(),
                RoomType.Special.ToString());
            colEventType.AutoComplete = true;
            colEventType.FlatStyle = FlatStyle.Flat;
            // 
            // colEventGroups
            // 
            colEventGroups.HeaderText = "Группы";
            colEventGroups.Name = "colEventGroups";
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { applyChangesToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(708, 24);
            menuStrip1.TabIndex = 3;
            menuStrip1.Text = "menuStrip1";
            // 
            // applyChangesToolStripMenuItem
            // 
            applyChangesToolStripMenuItem.Name = "applyChangesToolStripMenuItem";
            applyChangesToolStripMenuItem.Size = new Size(145, 20);
            applyChangesToolStripMenuItem.Text = "Применить изменения";
            // 
            // colLecturerName
            // 
            colLecturerName.HeaderText = "ФИО";
            colLecturerName.Name = "colLecturerName";
            // 
            // colLecturerTimeConstraints
            // 
            colLecturerTimeConstraints.HeaderText = "Ограничения по времени";
            colLecturerTimeConstraints.Name = "colLecturerTimeConstraints";
            // 
            // colRoomNumber
            // 
            colRoomNumber.HeaderText = "Номер";
            colRoomNumber.Name = "colRoomNumber";
            // 
            // colRoomHousing
            // 
            colRoomHousing.HeaderText = "Корпус";
            colRoomHousing.Name = "colRoomHousing";
            // 
            // colRoomType
            // 
            colRoomType.HeaderText = "Тип";
            colRoomType.Name = "colRoomType";
            colRoomType.Items.AddRange(
                EditDataForm.UNDEFINED_COMBOBOX_VALUE, 
                RoomType.Laboratory.ToString(), 
                RoomType.Lecture.ToString(), 
                RoomType.Practice.ToString(), 
                RoomType.Special.ToString());
            colRoomType.Resizable = DataGridViewTriState.True;
            colRoomType.SortMode = DataGridViewColumnSortMode.Automatic;
            colRoomType.AutoComplete = true;
            colRoomType.FlatStyle = FlatStyle.Flat;
            // 
            // colRoomTimeConstraints
            // 
            colRoomTimeConstraints.HeaderText = "Ограничения по времени";
            colRoomTimeConstraints.Name = "colRoomTimeConstraints";
            // 
            // EditDataForm
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(708, 562);
            Controls.Add(tabControl);
            Controls.Add(menuStrip1);
            FormBorderStyle = FormBorderStyle.Sizable;
            Icon = Properties.Resources.AppIcon;
            Name = "EditDataForm";
            Text = "Правка данных";
            tabControl.ResumeLayout(false);
            tabEditEvents.ResumeLayout(false);
            tabEditEvents.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        public enum Tab
        {
            Groups,
            Subjects,
            Rooms,
            Lecturers,
            Events
        }

        public EditDataForm(TimeTable data)
        {
            InitializeComponent();
            InitGrids();
            cbxSubjectFilter.SelectedIndexChanged += (s, e) => SetSubjectsFilter();
            cbxSubjectFilter.SelectedValueChanged += (s, e) => SetSubjectsFilter();
            applyChangesToolStripMenuItem.Click += (s, e) => ApplyChanges(true);
            FillGrids(data);
            NewData = null;
        }

        void SetSubjectsFilter()
        {
            var selectedSubjectName = cbxSubjectFilter.SelectedItem as string;
            var selectedSubject = gridSubjects.Items.FirstOrDefault(sbj => sbj.Name == selectedSubjectName);
            gridEvents.ShowFilter = ev => ev.SubjectId == selectedSubject.Id;
        }

        void InitGrids()
        {
            InitGroupsGrid();
            InitEventsGrid();
            InitSubjectsGrid();
            InitRoomsGrid();
            InitLecturersGrid();
        }        

        private void FillGrids(TimeTable data)
        {
            gridGroups.FillGrid(data.Groups);
            gridLecturers.FillGrid(data.Lecturers);
            gridSubjects.FillGrid(data.Subjects);
            gridRooms.FillGrid(data.Rooms);
            gridEvents.FillGrid(data.Events);
            SetSubjectsFilter();
        }

        void InitGroupsGrid()
        {
            var columnConstraints = new Dictionary<DataGridViewColumn, ColumnConstraintsType>()
            {
                {colGroupName, ColumnConstraintsType.UniqueValues},
                {colGroupCourse, ColumnConstraintsType.NotEmpty}
            };

            var fillGroupsRules = new Dictionary<DataGridViewColumn, Func<Group, string>>()
            {
                {colGroupName, g => g.Name},
                {colGroupCourse, g => g.Course.ToString()}
            };

            var parseGroupsRules = new Dictionary<DataGridViewColumn, Action<string, Group>>()
            {
                {colGroupName, (val, g) => g.Name = val},
                {colGroupCourse, (val, g) => g.Course = int.Parse(val)}
            };

            gridGroups = new SchedulingPrimitivesGrigView<Group>(
                tabEditGroups, columnConstraints, fillGroupsRules, parseGroupsRules);
            
            //gridGroups.LinkControl(); link to the groups select form
        }

        void InitEventsGrid()
        {
            var columnConstraints = new Dictionary<DataGridViewColumn, ColumnConstraintsType>()
            {
                {colEventSubject, ColumnConstraintsType.NotEmpty},
                {colEventLecturer, ColumnConstraintsType.NotEmpty},
                {colEventType, ColumnConstraintsType.NotEmpty},
                {colEventGroups, ColumnConstraintsType.NotEmpty}
            };

            var fillEventsRules = new Dictionary<DataGridViewColumn, Func<Event, string>>()
            {
                {colEventSubject, e => 
                {
                    var subject = gridSubjects.Items.FirstOrDefault(s => s.Id == e.SubjectId);
                    if (subject != null)
                    {
                        return subject.Name;
                    }
                    else return UNDEFINED_COMBOBOX_VALUE;
                }},
                {colEventLecturer, e => 
                {
                    var lecturer = gridLecturers.Items.FirstOrDefault(l => l.Id == e.LecturerId);
                    if (lecturer != null)
                    {
                        return lecturer.Name;
                    }
                    else return UNDEFINED_COMBOBOX_VALUE;
                }},
                {colEventType, e => e.RoomType.ToString()},
                {colEventGroups, e =>  string.Join(",", gridGroups.Items
                                                                  .Where(g => e.Groups.Contains(g.Id))
                                                                  .Select(g => g.Name))}
            };

            var parseEventsRules = new Dictionary<DataGridViewColumn, Action<string, Event>>()
            {
                {colEventSubject, (val, e) => 
                {
                    var subject = gridSubjects.Items.FirstOrDefault(s => s.ToString() == val);
                    e.SubjectId = subject.Id;
                }},
                {colEventLecturer, (val, e) => 
                {
                    var lecturer = gridLecturers.Items.FirstOrDefault(l => l.ToString() == val);
                    e.LecturerId = lecturer.Id;
                }},
                {colEventType, (val, e) => e.RoomType = ParseHelper.ParseEnum<RoomType>(val)},
                {colEventGroups, (val, e) => 
                    e.Groups = string.IsNullOrWhiteSpace(val) ? new int[0] : 
                                val.Split(',')
                                  .Select(s => gridGroups.Items.First(g => g.Name == s).Id)
                                  .ToArray()}
            };


            gridEvents = new SchedulingPrimitivesGrigView<Event>(
                panelEditEvents, columnConstraints, fillEventsRules, parseEventsRules);

            gridEvents.RowsAdded += (s, e) =>
            {
                for (var r = e.RowIndex; r < e.RowIndex + e.RowCount; r++)
                {
                    var row = gridEvents.Rows[r];
                    if (cbxSubjectFilter.SelectedItem != null)
                    {
                        gridEvents.SetCellValue(row.Cells[colEventSubject.Name],
                                                cbxSubjectFilter.SelectedItem.ToString());
                    }
                }
            };
        }

        void InitSubjectsGrid()
        {
            var columnConstraints = new Dictionary<DataGridViewColumn, ColumnConstraintsType>()
            {
                {colSubjectName, ColumnConstraintsType.UniqueValues},
                {colSubjectLecturer, ColumnConstraintsType.NoConstraints},
                {colSubjectDifficulty, ColumnConstraintsType.NoConstraints}
            };

            var fillSubjectsRules = new Dictionary<DataGridViewColumn, Func<Subject, string>>()
            {
                {colSubjectName, s => s.Name},
                {colSubjectLecturer, s =>
                {
                    var lecturer = gridLecturers.Items.FirstOrDefault(l => l.Id == s.LecturerId);
                    if (lecturer != null)
                    {
                        return lecturer.Name;
                    }
                    else return UNDEFINED_COMBOBOX_VALUE;
                }},
                {colSubjectDifficulty, s => s.Difficulty.ToString()}
            };

            var parseSubjectsRules = new Dictionary<DataGridViewColumn, Action<string, Subject>>()
            {
                {colSubjectName, (val, s) => s.Name = val},
                {colSubjectLecturer, (val, s) =>
                {
                    var lecturer = gridLecturers.Items.FirstOrDefault(l => l.Id == s.LecturerId);
                    if (lecturer != null)
                    {
                        s.LecturerId = lecturer.Id;
                    }
                    else s.LecturerId = null;
                }},
                {colSubjectDifficulty, (val, s) => s.Difficulty = int.Parse(val)}
            };

            gridSubjects = new SchedulingPrimitivesGrigView<Subject>(
                tabEditSubjects, columnConstraints, fillSubjectsRules, parseSubjectsRules);
            gridSubjects.LinkControl(colEventSubject);
            gridSubjects.LinkControl(cbxSubjectFilter);
        }

        void InitRoomsGrid()
        {
            var columnConstraints = new Dictionary<DataGridViewColumn, ColumnConstraintsType>()
            {
                {colRoomNumber, ColumnConstraintsType.NotEmpty},
                {colRoomHousing, ColumnConstraintsType.NotEmpty},
                {colRoomType, ColumnConstraintsType.NotEmpty},
            };

            var fillRoomsRules = new Dictionary<DataGridViewColumn, Func<Room, string>>()
            {
                {colRoomNumber, r => r.RoomNumber.ToString()},
                {colRoomHousing, r => r.Housing.ToString()},
                {colRoomType, r => r.Type.ToString()},
                //{"colRoomTimeConstraints", (c, r) => 
                //{
                //    var x = c as DataGridViewButtonCell;
                //}}
            };

            var parseRoomsRules = new Dictionary<DataGridViewColumn, Action<string, Room>>()
            {
                {colRoomNumber, (val, r) => r.RoomNumber = val},
                {colRoomHousing, (val, r) => r.Housing = int.Parse(val)},
                {colRoomType, (val, r) => r.Type = ParseHelper.ParseEnum<RoomType>(val)},
                //{"colRoomTimeConstraints", (c, r) => 
                //{
                //    var x = c as DataGridViewButtonCell;
                //}}
            };

            gridRooms = new SchedulingPrimitivesGrigView<Room>(
                tabEditRooms, columnConstraints, fillRoomsRules, parseRoomsRules);
        }

        void InitLecturersGrid()
        {
            var columnConstraints = new Dictionary<DataGridViewColumn, ColumnConstraintsType>()
            {
                {colLecturerName, ColumnConstraintsType.UniqueValues}
            };

            var fillLecturerRules = new Dictionary<DataGridViewColumn, Func<Lecturer, string>>()
            {
                {colLecturerName, l => l.Name}
                //{"colLecturerTimeConstraints", l => new ConstraintsButton(l) }
            };

            var parseLecturerRules = new Dictionary<DataGridViewColumn, Action<string, Lecturer>>()
            {
                {colLecturerName, (val, l) => l.Name = val}
                //{"colLecturerTimeConstraints", l => new ConstraintsButton(l) }
            };

            gridLecturers = new SchedulingPrimitivesGrigView<Lecturer>(
                tabEditLecturers, columnConstraints, fillLecturerRules, parseLecturerRules);
            gridLecturers.LinkControl(colEventLecturer);
            gridLecturers.LinkControl(colSubjectLecturer);
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

        public bool DataChanged { get { return NewData != null; } }

        public TimeTable NewData { get; private set; }

        private bool ApplyChanges(bool showSuccessMessage)
        {
            var conflictsGroups = new List<string>();
            if (!gridRooms.DataValid)
                conflictsGroups.Add("Аудитории");
            if (!gridGroups.DataValid)
                conflictsGroups.Add("Группы");
            if (!gridSubjects.DataValid)
                conflictsGroups.Add("Предметы");
            if (!gridLecturers.DataValid)
                conflictsGroups.Add("Преподаватели");
            if (!gridEvents.DataValid)
                conflictsGroups.Add("Занятия");

            if (!conflictsGroups.Any())
            {
                NewData = TimeTable.MakeEmpty();
                NewData.Rooms = gridRooms.Items.ToArray();
                NewData.Groups = gridGroups.Items.ToArray();
                NewData.Subjects = gridSubjects.Items.ToArray();
                NewData.Lecturers = gridLecturers.Items.ToArray();
                NewData.Events = gridEvents.Items.ToArray();
                if (showSuccessMessage)
                {
                    MessageBox.Show("Изменения успешно применены",
                                    "Составление расписания",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
                return true;
            }
            else
            {
                MessageBox.Show("Невозможно применить внесенные изменения из-за существующих ошибок данных (вкладк" +
                                (conflictsGroups.Count == 1 ? "а" : "и") + " " +
                                string.Join(", ", conflictsGroups) + 
                                ").\nИсправте их и попробуйте снова.",
                                "Составление расписания",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return false;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            var res = MessageBox.Show("Применить внесенные изменения?",
                                    "Составление расписания",
                                    MessageBoxButtons.YesNoCancel,
                                    MessageBoxIcon.Question);
            switch (res)
            {
                case DialogResult.Yes: e.Cancel = !ApplyChanges(false); break;
                case DialogResult.Cancel: e.Cancel = true; break;
            }

            base.OnFormClosing(e);
        }
    }
}
