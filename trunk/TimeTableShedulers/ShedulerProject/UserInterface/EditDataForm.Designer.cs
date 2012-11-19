namespace ShedulerProject.UserInterface
{
    partial class EditDataForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditDataForm));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabEditGroups = new System.Windows.Forms.TabPage();
            this.gridGroups = new System.Windows.Forms.DataGridView();
            this.tabEditSubjects = new System.Windows.Forms.TabPage();
            this.gridSubjects = new System.Windows.Forms.DataGridView();
            this.tabEditRooms = new System.Windows.Forms.TabPage();
            this.gridRooms = new System.Windows.Forms.DataGridView();
            this.tabEditLecturers = new System.Windows.Forms.TabPage();
            this.gridLecturers = new System.Windows.Forms.DataGridView();
            this.tabEditEvents = new System.Windows.Forms.TabPage();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.gridEvents = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.применитьИзмененияToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colSubjectId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSubjectName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSubjectLecturer = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colSubjectDifficulty = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colEventId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEventLecturer = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colEventType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colEventGroups = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRoomId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRoomNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRoomHousing = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRoomType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colRoomTimeConstraits = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colLecturerId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLecturerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLecturerHardTimeConstraits = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colGroupId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colGroupName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colGroupCourse = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.tabControl.SuspendLayout();
            this.tabEditGroups.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridGroups)).BeginInit();
            this.tabEditSubjects.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridSubjects)).BeginInit();
            this.tabEditRooms.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridRooms)).BeginInit();
            this.tabEditLecturers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridLecturers)).BeginInit();
            this.tabEditEvents.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridEvents)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabEditGroups);
            this.tabControl.Controls.Add(this.tabEditLecturers);
            this.tabControl.Controls.Add(this.tabEditSubjects);
            this.tabControl.Controls.Add(this.tabEditRooms);
            this.tabControl.Controls.Add(this.tabEditEvents);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 24);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(708, 538);
            this.tabControl.TabIndex = 2;
            // 
            // tabEditGroups
            // 
            this.tabEditGroups.Controls.Add(this.gridGroups);
            this.tabEditGroups.Location = new System.Drawing.Point(4, 22);
            this.tabEditGroups.Name = "tabEditGroups";
            this.tabEditGroups.Padding = new System.Windows.Forms.Padding(3);
            this.tabEditGroups.Size = new System.Drawing.Size(700, 512);
            this.tabEditGroups.TabIndex = 0;
            this.tabEditGroups.Text = "Группы";
            this.tabEditGroups.UseVisualStyleBackColor = true;
            // 
            // gridGroups
            // 
            this.gridGroups.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridGroups.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridGroups.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colGroupId,
            this.colGroupName,
            this.colGroupCourse});
            this.gridGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridGroups.Location = new System.Drawing.Point(3, 3);
            this.gridGroups.Name = "gridGroups";
            this.gridGroups.Size = new System.Drawing.Size(694, 506);
            this.gridGroups.TabIndex = 0;
            // 
            // tabEditSubjects
            // 
            this.tabEditSubjects.Controls.Add(this.gridSubjects);
            this.tabEditSubjects.Location = new System.Drawing.Point(4, 22);
            this.tabEditSubjects.Name = "tabEditSubjects";
            this.tabEditSubjects.Padding = new System.Windows.Forms.Padding(3);
            this.tabEditSubjects.Size = new System.Drawing.Size(700, 512);
            this.tabEditSubjects.TabIndex = 1;
            this.tabEditSubjects.Text = "Предметы";
            this.tabEditSubjects.UseVisualStyleBackColor = true;
            // 
            // gridSubjects
            // 
            this.gridSubjects.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridSubjects.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridSubjects.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSubjectId,
            this.colSubjectName,
            this.colSubjectLecturer,
            this.colSubjectDifficulty});
            this.gridSubjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridSubjects.Location = new System.Drawing.Point(3, 3);
            this.gridSubjects.Name = "gridSubjects";
            this.gridSubjects.Size = new System.Drawing.Size(694, 506);
            this.gridSubjects.TabIndex = 1;
            // 
            // tabEditRooms
            // 
            this.tabEditRooms.Controls.Add(this.gridRooms);
            this.tabEditRooms.Location = new System.Drawing.Point(4, 22);
            this.tabEditRooms.Name = "tabEditRooms";
            this.tabEditRooms.Padding = new System.Windows.Forms.Padding(3);
            this.tabEditRooms.Size = new System.Drawing.Size(700, 512);
            this.tabEditRooms.TabIndex = 2;
            this.tabEditRooms.Text = "Аудитории";
            this.tabEditRooms.UseVisualStyleBackColor = true;
            // 
            // gridRooms
            // 
            this.gridRooms.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridRooms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridRooms.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colRoomId,
            this.colRoomNumber,
            this.colRoomHousing,
            this.colRoomType,
            this.colRoomTimeConstraits});
            this.gridRooms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridRooms.Location = new System.Drawing.Point(3, 3);
            this.gridRooms.Name = "gridRooms";
            this.gridRooms.Size = new System.Drawing.Size(694, 506);
            this.gridRooms.TabIndex = 1;
            // 
            // tabEditLecturers
            // 
            this.tabEditLecturers.Controls.Add(this.gridLecturers);
            this.tabEditLecturers.Location = new System.Drawing.Point(4, 22);
            this.tabEditLecturers.Name = "tabEditLecturers";
            this.tabEditLecturers.Padding = new System.Windows.Forms.Padding(3);
            this.tabEditLecturers.Size = new System.Drawing.Size(700, 512);
            this.tabEditLecturers.TabIndex = 3;
            this.tabEditLecturers.Text = "Преподаватели";
            this.tabEditLecturers.UseVisualStyleBackColor = true;
            // 
            // gridLecturers
            // 
            this.gridLecturers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridLecturers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridLecturers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colLecturerId,
            this.colLecturerName,
            this.colLecturerHardTimeConstraits});
            this.gridLecturers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridLecturers.Location = new System.Drawing.Point(3, 3);
            this.gridLecturers.Name = "gridLecturers";
            this.gridLecturers.Size = new System.Drawing.Size(694, 506);
            this.gridLecturers.TabIndex = 1;
            // 
            // tabEditEvents
            // 
            this.tabEditEvents.Controls.Add(this.comboBox1);
            this.tabEditEvents.Controls.Add(this.gridEvents);
            this.tabEditEvents.Controls.Add(this.label1);
            this.tabEditEvents.Location = new System.Drawing.Point(4, 22);
            this.tabEditEvents.Name = "tabEditEvents";
            this.tabEditEvents.Padding = new System.Windows.Forms.Padding(3);
            this.tabEditEvents.Size = new System.Drawing.Size(700, 512);
            this.tabEditEvents.TabIndex = 4;
            this.tabEditEvents.Text = "Занятия";
            this.tabEditEvents.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(117, 6);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(208, 21);
            this.comboBox1.TabIndex = 3;
            // 
            // gridEvents
            // 
            this.gridEvents.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridEvents.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridEvents.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colEventId,
            this.colEventLecturer,
            this.colEventType,
            this.colEventGroups});
            this.gridEvents.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gridEvents.Location = new System.Drawing.Point(3, 33);
            this.gridEvents.Name = "gridEvents";
            this.gridEvents.Size = new System.Drawing.Size(694, 476);
            this.gridEvents.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Выберите предмет";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.применитьИзмененияToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(708, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // применитьИзмененияToolStripMenuItem
            // 
            this.применитьИзмененияToolStripMenuItem.Name = "применитьИзмененияToolStripMenuItem";
            this.применитьИзмененияToolStripMenuItem.Size = new System.Drawing.Size(145, 20);
            this.применитьИзмененияToolStripMenuItem.Text = "Применить изменения";
            // 
            // colSubjectId
            // 
            this.colSubjectId.HeaderText = "ID";
            this.colSubjectId.Name = "colSubjectId";
            this.colSubjectId.ReadOnly = true;
            // 
            // colSubjectName
            // 
            this.colSubjectName.HeaderText = "Название";
            this.colSubjectName.Name = "colSubjectName";
            // 
            // colSubjectLecturer
            // 
            this.colSubjectLecturer.HeaderText = "Преподаватель";
            this.colSubjectLecturer.Name = "colSubjectLecturer";
            this.colSubjectLecturer.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colSubjectLecturer.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colSubjectDifficulty
            // 
            this.colSubjectDifficulty.HeaderText = "Сложность";
            this.colSubjectDifficulty.Name = "colSubjectDifficulty";
            // 
            // colEventId
            // 
            this.colEventId.HeaderText = "ID";
            this.colEventId.Name = "colEventId";
            // 
            // colEventLecturer
            // 
            this.colEventLecturer.HeaderText = "Преподаватель";
            this.colEventLecturer.Name = "colEventLecturer";
            // 
            // colEventType
            // 
            this.colEventType.HeaderText = "Тип";
            this.colEventType.Name = "colEventType";
            // 
            // colEventGroups
            // 
            this.colEventGroups.HeaderText = "Группы";
            this.colEventGroups.Name = "colEventGroups";
            // 
            // colRoomId
            // 
            this.colRoomId.HeaderText = "ID";
            this.colRoomId.Name = "colRoomId";
            this.colRoomId.ReadOnly = true;
            // 
            // colRoomNumber
            // 
            this.colRoomNumber.HeaderText = "Номер";
            this.colRoomNumber.Name = "colRoomNumber";
            // 
            // colRoomHousing
            // 
            this.colRoomHousing.HeaderText = "Корпус";
            this.colRoomHousing.Name = "colRoomHousing";
            // 
            // colRoomType
            // 
            this.colRoomType.HeaderText = "Тип";
            this.colRoomType.Name = "colRoomType";
            this.colRoomType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colRoomType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colRoomTimeConstraits
            // 
            this.colRoomTimeConstraits.HeaderText = "Ограничения по времени";
            this.colRoomTimeConstraits.Name = "colRoomTimeConstraits";
            // 
            // colLecturerId
            // 
            this.colLecturerId.HeaderText = "ID";
            this.colLecturerId.Name = "colLecturerId";
            this.colLecturerId.ReadOnly = true;
            // 
            // colLecturerName
            // 
            this.colLecturerName.HeaderText = "ФИО";
            this.colLecturerName.Name = "colLecturerName";
            // 
            // colLecturerHardTimeConstraits
            // 
            this.colLecturerHardTimeConstraits.HeaderText = "Ограничения по времени";
            this.colLecturerHardTimeConstraits.Name = "colLecturerHardTimeConstraits";
            // 
            // colGroupId
            // 
            this.colGroupId.HeaderText = "ID";
            this.colGroupId.Name = "colGroupId";
            // 
            // colGroupName
            // 
            this.colGroupName.HeaderText = "Номер";
            this.colGroupName.Name = "colGroupName";
            // 
            // colGroupCourse
            // 
            this.colGroupCourse.HeaderText = "Курс";
            this.colGroupCourse.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6"});
            this.colGroupCourse.Name = "colGroupCourse";
            this.colGroupCourse.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colGroupCourse.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // EditDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(708, 562);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EditDataForm";
            this.Text = "Правка данных";
            this.tabControl.ResumeLayout(false);
            this.tabEditGroups.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridGroups)).EndInit();
            this.tabEditSubjects.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridSubjects)).EndInit();
            this.tabEditRooms.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridRooms)).EndInit();
            this.tabEditLecturers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridLecturers)).EndInit();
            this.tabEditEvents.ResumeLayout(false);
            this.tabEditEvents.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridEvents)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabEditGroups;
        private System.Windows.Forms.DataGridView gridGroups;
        private System.Windows.Forms.TabPage tabEditSubjects;
        private System.Windows.Forms.TabPage tabEditRooms;
        private System.Windows.Forms.TabPage tabEditLecturers;
        private System.Windows.Forms.DataGridView gridSubjects;
        private System.Windows.Forms.DataGridView gridRooms;
        private System.Windows.Forms.DataGridView gridLecturers;
        private System.Windows.Forms.TabPage tabEditEvents;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView gridEvents;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ToolStripMenuItem применитьИзмененияToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSubjectId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSubjectName;
        private System.Windows.Forms.DataGridViewComboBoxColumn colSubjectLecturer;
        private System.Windows.Forms.DataGridViewComboBoxColumn colSubjectDifficulty;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEventId;
        private System.Windows.Forms.DataGridViewComboBoxColumn colEventLecturer;
        private System.Windows.Forms.DataGridViewComboBoxColumn colEventType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEventGroups;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRoomId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRoomNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRoomHousing;
        private System.Windows.Forms.DataGridViewComboBoxColumn colRoomType;
        private System.Windows.Forms.DataGridViewButtonColumn colRoomTimeConstraits;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLecturerId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLecturerName;
        private System.Windows.Forms.DataGridViewButtonColumn colLecturerHardTimeConstraits;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGroupId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGroupName;
        private System.Windows.Forms.DataGridViewComboBoxColumn colGroupCourse;

    }
}