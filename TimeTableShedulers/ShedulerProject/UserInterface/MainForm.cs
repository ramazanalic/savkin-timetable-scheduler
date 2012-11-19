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
    public partial class MainForm : Form
    {
        TimeTable _currentData = TimeTable.MakeEmpty();
        EditDataForm _editDataForm;

        string _openedFile;
        private bool _dataChanged;

        private string OpenedFile
        {
            get { return _openedFile; }
            set { _openedFile = value; Text = WindowTitle; }
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
            OpenedFile = "Untitled.xml";
        }

        private void miLoadData_Click(object sender, EventArgs e)
        {
            if (openDataFileDialog.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                _currentData = TimeTable.LoadFromXml(openDataFileDialog.FileName);
                Cursor = Cursors.Arrow;
                OpenedFile = openDataFileDialog.FileName;
                OpenedDataChanged = false;
            }
        }

        private void miSaveData_Click(object sender, EventArgs e)
        {
            saveDataFileDialog.FileName = OpenedFile;
            if (saveDataFileDialog.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                _currentData.SaveToXml(saveDataFileDialog.FileName);
                Cursor = Cursors.Arrow;
                OpenedDataChanged = false;
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
            _editDataForm.Show();
            _editDataForm.SetActiveTab(startTab);
            OpenedDataChanged = _editDataForm.DataChanged;
        }

        private void miShedule_Click(object sender, EventArgs e)
        {

        }
    }
}
