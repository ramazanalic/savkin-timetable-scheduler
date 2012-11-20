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

       DataGridViewTextBoxColumn colGroupId = new DataGridViewTextBoxColumn();
       DataGridViewTextBoxColumn colGroupName = new DataGridViewTextBoxColumn();
       DataGridViewComboBoxColumn colGroupCourse = new DataGridViewComboBoxColumn();
       DataGridViewTextBoxColumn colSubjectId = new DataGridViewTextBoxColumn();
       DataGridViewTextBoxColumn colSubjectName = new DataGridViewTextBoxColumn();
       DataGridViewComboBoxColumn colSubjectLecturer = new DataGridViewComboBoxColumn();
       DataGridViewComboBoxColumn colSubjectDifficulty = new DataGridViewComboBoxColumn();
       DataGridViewTextBoxColumn colEventId = new DataGridViewTextBoxColumn();
       DataGridViewComboBoxColumn colEventLecturer = new DataGridViewComboBoxColumn();
       DataGridViewComboBoxColumn colEventType = new DataGridViewComboBoxColumn();
       DataGridViewTextBoxColumn colEventGroups = new DataGridViewTextBoxColumn();
       DataGridViewTextBoxColumn colLecturerId = new DataGridViewTextBoxColumn();
       DataGridViewTextBoxColumn colLecturerName = new DataGridViewTextBoxColumn();
       DataGridViewButtonColumn colLecturerTimeConstraints = new DataGridViewButtonColumn();
       DataGridViewTextBoxColumn colRoomId = new DataGridViewTextBoxColumn();
       DataGridViewTextBoxColumn colRoomNumber = new DataGridViewTextBoxColumn();
       DataGridViewTextBoxColumn colRoomHousing = new DataGridViewTextBoxColumn();
       DataGridViewComboBoxColumn colRoomType = new DataGridViewComboBoxColumn();
       DataGridViewButtonColumn colRoomTimeConstraints = new DataGridViewButtonColumn();

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        public EditDataForm(TimeTable data)
        {
            InitializeComponent();
            this.data = data;
            InitGrids();
            FillGrids();
            cbxSubjectFilter.SelectedIndexChanged += (s, e) =>
            {
                var selectedSubjectName = cbxSubjectFilter.SelectedItem as string;
                var selectedSubject = gridSubjects.Items.FirstOrDefault(sbj => sbj.Name == selectedSubjectName);
                gridEvents.ShowFilter = ev => ev.SubjectId == selectedSubject.Id;
                //gridSubjects.Refresh();
            };
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(EditDataForm));
                      
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
            // colGroupId
            // 
            colGroupId.HeaderText = "ID";
            colGroupId.Name = "colGroupId";
            // 
            // colGroupName
            // 
            colGroupName.HeaderText = "Номер";
            colGroupName.Name = "colGroupName";
            // 
            // colGroupCourse
            // 
            colGroupCourse.HeaderText = "Курс";
            colGroupCourse.Items.AddRange("1", "2", "3", "4", "5", "6");
            colGroupCourse.Name = "colGroupCourse";
            colGroupCourse.Resizable = DataGridViewTriState.True;
            colGroupCourse.SortMode = DataGridViewColumnSortMode.Automatic;
            // 
            // colSubjectId
            // 
            colSubjectId.HeaderText = "ID";
            colSubjectId.Name = "colSubjectId";
            colSubjectId.ReadOnly = true;
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
            // 
            // colSubjectDifficulty
            // 
            colSubjectDifficulty.HeaderText = "Сложность";
            colSubjectDifficulty.Name = "colSubjectDifficulty";
            colSubjectDifficulty.Items.AddRange("0", "1", "2", "3");
            // 
            // colEventId
            // 
            colEventId.HeaderText = "ID";
            colEventId.Name = "colEventId";
            // 
            // colEventLecturer
            // 
            colEventLecturer.HeaderText = "Преподаватель";
            colEventLecturer.Name = "colEventLecturer";
            // 
            // colEventType
            // 
            colEventType.HeaderText = "Тип";
            colEventType.Name = "colEventType";
            colEventType.Items.AddRange(
                RoomType.Laboratory.ToString(),
                RoomType.Lecture.ToString(),
                RoomType.Practice.ToString(),
                RoomType.Special.ToString());
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
            // colLecturerId
            // 
            colLecturerId.HeaderText = "ID";
            colLecturerId.Name = "colLecturerId";
            colLecturerId.ReadOnly = true;
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
            // colRoomId
            // 
            colRoomId.HeaderText = "ID";
            colRoomId.Name = "colRoomId";
            colRoomId.ReadOnly = true;
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
                RoomType.Laboratory.ToString(), 
                RoomType.Lecture.ToString(), 
                RoomType.Practice.ToString(), 
                RoomType.Special.ToString());
            colRoomType.Resizable = DataGridViewTriState.True;
            colRoomType.SortMode = DataGridViewColumnSortMode.Automatic;
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
            FormBorderStyle = FormBorderStyle.Sizable;//.SizableToolWindow;
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
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

        void InitGrids()
        {
            var groupsRules = new Dictionary<DataGridViewColumn, Func<Group, string>>()
            {
                {colGroupId, g => g.Id.ToString()},
                {colGroupName, g => g.Name},
                {colGroupCourse, g => g.Course.ToString()}
            };
            gridGroups = new SchedulingPrimitivesGrigView<Group>(tabEditGroups, groupsRules.Keys, groupsRules);
            
            //gridGroups.LinkControl(); link to the groups select form

            var eventsRules = new Dictionary<DataGridViewColumn, Func<Event, string>>()
            {
                {colEventId, e => e.Id.ToString()},
                {colEventLecturer, e => 
                {
                    var lecturer = gridLecturers.Items.FirstOrDefault(l => l.Id == e.LecturerId);
                    if (lecturer != null)
                    {
                        return lecturer.Name;
                    }
                    else return string.Empty;
                }},
                {colEventType, e => e.RoomType.ToString()},
                {colEventGroups, e =>  string.Join(",", gridGroups.Items
                                                                  .Where(g => e.Groups.Contains(g.Id))
                                                                  .Select(g => g.Name))}
            };
            gridEvents = new SchedulingPrimitivesGrigView<Event>(panelEditEvents, eventsRules.Keys, eventsRules);

            var subjectsRules = new Dictionary<DataGridViewColumn, Func<Subject, string>>()
            {
                {colSubjectId, s => s.Id.ToString()},
                {colSubjectName, s => s.Name},
                {colSubjectLecturer, s =>
                {
                    var lecturer = gridLecturers.Items.FirstOrDefault(l => l.Id == s.LecturerId);
                    if (lecturer != null)
                    {
                        return lecturer.Name;
                    }
                    else return string.Empty;
                }},
                {colSubjectDifficulty, s => s.Difficulty.ToString()}
            };
            gridSubjects = new SchedulingPrimitivesGrigView<Subject>(tabEditSubjects, subjectsRules.Keys, subjectsRules);
            gridSubjects.LinkControl(cbxSubjectFilter);

            var roomsRules = new Dictionary<DataGridViewColumn, Func<Room, string>>()
            {
                {colRoomId, r => r.Id.ToString()},
                {colRoomNumber, r => r.RoomNumber.ToString()},
                {colRoomHousing, r => r.Housing.ToString()},
                {colRoomType, r => r.Type.ToString()},
                //{"colRoomTimeConstraints", (c, r) => 
                //{
                //    var x = c as DataGridViewButtonCell;
                //}}
            };
            gridRooms = new SchedulingPrimitivesGrigView<Room>(tabEditRooms, roomsRules.Keys, roomsRules);

            var lecturerRules = new Dictionary<DataGridViewColumn, Func<Lecturer, string>>()
            {
                {colLecturerId, l => l.Id.ToString()},
                {colLecturerName, l => l.Name}
                //{"colLecturerTimeConstraints", l => new ConstraintsButton(l) }
            };
            gridLecturers = new SchedulingPrimitivesGrigView<Lecturer>(tabEditLecturers, lecturerRules.Keys, lecturerRules);
            gridLecturers.LinkControl(colEventLecturer);
            gridLecturers.LinkControl(colSubjectLecturer);
        }        

        private void FillGrids()
        {
            gridGroups.FillGrid(data.Groups);
            gridLecturers.FillGrid(data.Lecturers);
            gridSubjects.FillGrid(data.Subjects);
            gridRooms.FillGrid(data.Rooms);
            gridEvents.FillGrid(data.Events);
            gridEvents.ShowFilter = e => false;//hide all events after loading
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

        public bool DataChanged { get; private set; }

        private void ApplyChanges()
        {
            DataChanged = true;
            data.Rooms = gridRooms.Items.ToArray();
            data.Groups = gridGroups.Items.ToArray();
            data.Subjects = gridSubjects.Items.ToArray();
            data.Lecturers = gridLecturers.Items.ToArray();
            data.Events = gridEvents.Items.ToArray();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            var res = MessageBox.Show("Применить внесенные изменения?",
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
    }

    public class SchedulingPrimitivesGrigView<PrimitiveType> : DataGridView
        where PrimitiveType : AbstractPrimitive, new()
    {
        Dictionary<DataGridViewColumn, Action<DataGridViewCell, PrimitiveType>> parseRules;
        Dictionary<DataGridViewColumn, Func<PrimitiveType, string>> fillRules;

        public SchedulingPrimitivesGrigView(
            Control parent,
            IEnumerable<DataGridViewColumn> columns,
            Dictionary<DataGridViewColumn, Func<PrimitiveType, string>> fillRules,
            Dictionary<DataGridViewColumn, Action<DataGridViewCell, PrimitiveType>> parseRules = null)
        {
            if (fillRules.Keys.Any(c => !columns.Contains(c)))
            {
                throw new ArgumentException("Column does not belong to the grid", "fillRules");
            }

            if (parseRules != null && parseRules.Keys.Any(c => !columns.Contains(c)))
            {
                throw new ArgumentException("Column does not belong to the grid", "fillRules");
            }

            this.fillRules = fillRules;
            this.parseRules = parseRules;

            parent.Controls.Add(this);
            Dock = DockStyle.Fill;
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            Columns.AddRange(columns.ToArray());

            RowsAdded += (s, e) =>
            {
                for (var i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
                {
                    // add an empty primitive for each new row
                    associatedPrimitives.Add(Rows[i], new PrimitiveType());
                }
            };

            DataError += (s, e) =>
            {
                MessageBox.Show(
                    "DataError in cell[" + e.RowIndex + ", "
                    + e.ColumnIndex + "] (" + typeof(PrimitiveType) + "):\n" +
                    e.Exception.Message);
            };
        }

        public IEnumerable<PrimitiveType> Items
        {
            get { return associatedPrimitives.Values.Where(p => !p.IsEmpty); }
        }

        public void FillGrid(IEnumerable<PrimitiveType> source)
        {
            Rows.Clear();

            foreach (PrimitiveType elem in source)
            {
                var i = Rows.Add();
                var newRow = Rows[i];
                foreach (var fillRule in fillRules)
                {
                    newRow.Cells[fillRule.Key.Name].Value = fillRule.Value(elem);
                }
                associatedPrimitives[newRow] = elem;
            }
            AutoResizeColumns();
            RefreshLinkedControlsData();
        }

        Predicate<PrimitiveType> filterCriteria;

        Dictionary<DataGridViewRow, PrimitiveType> associatedPrimitives =
            new Dictionary<DataGridViewRow, PrimitiveType>();

        public Predicate<PrimitiveType> ShowFilter
        {
            set
            {
                filterCriteria = value;
                if (filterCriteria != null)
                {
                    foreach (DataGridViewRow row in Rows)
                    {
                        row.Visible = row.IsNewRow || filterCriteria(associatedPrimitives[row]);
                    }
                }
            }
        }

        public void LinkControl(DataGridViewComboBoxColumn comboBoxColumn)
        {
            linkedControlsData.Add(comboBoxColumn.Items);
        }

        public void LinkControl(ComboBox comboBox)
        {
            linkedControlsData.Add(comboBox.Items);
        }

        private void RefreshLinkedControlsData()
        {
            var items = Items.Select(p => p.ToString()).OrderBy(s => s).ToArray();
            foreach (var data in linkedControlsData)
            {
                data.Clear();
                foreach (var item in items)
                {
                    data.Add(item);
                }
            }
        }

        List<System.Collections.IList> linkedControlsData = new List<System.Collections.IList>();

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            RefreshLinkedControlsData();
        }
    }
    
    public class ConstraintsButton : Button
    {
        public ConstraintsButton(IConstrainedPrimitive primitive)
            : base()
        {
        }
    }
}
