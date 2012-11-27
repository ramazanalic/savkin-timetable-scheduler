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
    public partial class MainForm : Form
    {
        TimeTable _currentData = TimeTable.MakeEmpty();
        EditDataForm _editDataForm;

        string _openedFile;
        string _openedFileName;
        string _openedFileDirectory;
        private bool _dataChanged;
        Circullum _selectedCircullum;

        Circullum SelectedCircullum
        { 
            get { return _selectedCircullum; }            
            set
            {
                _selectedCircullum = value;
                OnSelectedCircullumChanged(value);
            }
        }

        void OnSelectedCircullumChanged(Circullum selectedCircullum)
        {
            var empty = selectedCircullum == null;
            //tabByLecturers.Enabled = tabByGroups.Enabled = tabByGroups.Enabled =
                                     timeTableDetails.Enabled = miSaveTimeTable.Enabled = !empty;
            if (!empty)
            {
            }
        }

        private string OpenedFile
        {
            get { return _openedFile; }
            set 
            { 
                _openedFile = value;
                _openedFileName = System.IO.Path.GetFileName(_openedFile);
                _openedFileDirectory = System.IO.Path.GetDirectoryName(_openedFile);
                Text = WindowTitle; 
            }
        }

        private bool OpenedDataChanged
        {
            get { return _dataChanged; }
            set { _dataChanged = value; Text = WindowTitle; }
        }

        private string WindowTitle
        {
            get
            {
                return "Составление расписания - " + _openedFile + (_dataChanged ? " *" : "");
            }
        }

        public MainForm()
        {
            InitializeComponent();
            var c = new TimeSlotsConstraintsEditControl(new Size(100, 100));
            tabByGroups.Controls.Add(c);
            OpenedFile = "Untitled.xml";
            SelectedCircullum = null;
        }

        private void miLoadData_Click(object sender, EventArgs e)
        {
            openDataFileDialog.InitialDirectory = _openedFileDirectory;
            openDataFileDialog.FileName = _openedFileName;
            openDataFileDialog.Filter = "XML-файлы данных расписания|*.xml";
            if (openDataFileDialog.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                try
                {
                    _currentData = TimeTable.LoadFromXml(openDataFileDialog.FileName);
                    OpenedFile = openDataFileDialog.FileName;
                    OpenedDataChanged = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка при загрузке файла", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Cursor = Cursors.Arrow;
            }
        }

        private void miSaveData_Click(object sender, EventArgs e)
        {
            saveDataFileDialog.InitialDirectory = _openedFileDirectory;
            saveDataFileDialog.FileName = _openedFileName;
            openDataFileDialog.Filter = "XML-файлы данных расписания|*.xml";
            if (saveDataFileDialog.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                try
                {
                    _currentData.SaveToXml(saveDataFileDialog.FileName);
                    OpenedDataChanged = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка при сохранении файла",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Cursor = Cursors.Arrow;
            }
        }

        private void miOpenTimeTable_Click(object sender, EventArgs e)
        {
            openDataFileDialog.InitialDirectory = "";
            openDataFileDialog.Filter = "XML-файлы расписаний|*.xml";
            if (openDataFileDialog.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                Circullum openedCircullum = null;
                try
                {
                    openedCircullum = Circullum.LoadFromXml(_currentData, openDataFileDialog.FileName);
                }
                catch (IdMismatchException)
                {
                    MessageBox.Show("Этот файл расписания не соответствует загруженным данным", "Ошибка при загрузке файла",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка при загрузке файла",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (openedCircullum != null)
                    AddCircullum(openedCircullum);
                Cursor = Cursors.Arrow;
            }
        }

        private void miSaveTimeTable_Click(object sender, EventArgs e)
        {
            saveDataFileDialog.InitialDirectory = "";
            saveDataFileDialog.Filter = "XML-файлы расписаний|*.xml";
            if (saveDataFileDialog.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                try
                {
                    SelectedCircullum.SaveToXml(saveDataFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка при сохранении файла",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Cursor = Cursors.Arrow;
            }
        }

        private void miEditGroups_Click(object sender, EventArgs e)
        {
            ShowEditDataForm(EditDataForm.Tab.Groups);
        }

        private void miEditSubjects_Click(object sender, EventArgs e)
        {
            ShowEditDataForm(EditDataForm.Tab.Subjects);
        }

        private void miEditRooms_Click(object sender, EventArgs e)
        {
            ShowEditDataForm(EditDataForm.Tab.Rooms);
        }

        private void miEditLecturers_Click(object sender, EventArgs e)
        {
            ShowEditDataForm(EditDataForm.Tab.Lecturers);
        }

        private void miEventsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowEditDataForm(EditDataForm.Tab.Events);
        }

        private void ShowEditDataForm(EditDataForm.Tab startTab)
        {
            _editDataForm = new EditDataForm(_currentData);
            _editDataForm.SetActiveTab(startTab);
            _editDataForm.ShowDialog(this);
            if (_editDataForm.DataChanged)
            {
                OpenedDataChanged = true;
                _currentData = _editDataForm.NewData;
            }
        }

        void AddCircullum(Circullum circullum)
        {
            timeTablesList.Items.Add(new ListViewItem(circullum.Name) { Tag = circullum });
            SelectedCircullum = circullum;
        }

        private void miShedule_Click(object sender, EventArgs e)
        {
            var circullum = Scheduler.Shedule(_currentData);
            string newName = "Untitled circullum";
            circullum.Name = newName;
            AddCircullum(circullum);
        }
    }
}
