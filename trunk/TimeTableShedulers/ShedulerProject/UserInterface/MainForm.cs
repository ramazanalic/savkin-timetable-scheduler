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
            tabByGroups.Controls.Add(new TimeSlotsControl<Control>());
            OpenedFile = "Untitled.xml";
        }

        private void miLoadData_Click(object sender, EventArgs e)
        {
            openDataFileDialog.InitialDirectory = _openedFileDirectory;
            openDataFileDialog.FileName = _openedFileName;
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
            if(_editDataForm.DataChanged)
            {
                OpenedDataChanged = true;
                _currentData = _editDataForm.NewData;
            }
        }

        private void miShedule_Click(object sender, EventArgs e)
        {

        }
    }
}
