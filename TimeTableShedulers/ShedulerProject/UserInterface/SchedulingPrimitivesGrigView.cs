using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using SchedulerProject.Core;

namespace SchedulerProject.UserInterface
{
    public enum ColumnConstraintsType
    {
        NoConstraints = 0,
        NotEmpty = 1,
        UniqueValues = 2
    }

    public class SchedulingPrimitivesGrigView<PrimitiveType> : DataGridView
        where PrimitiveType : AbstractPrimitive<PrimitiveType>, new()
    {
        #region Internal data and construction

        // how to fill primitive type using values of row cells
        Dictionary<DataGridViewColumn, Action<string, PrimitiveType>> parseRules;

        // how to get values of row cells from a primitive instance
        Dictionary<DataGridViewColumn, Func<PrimitiveType, string>> fillRules;

        Predicate<PrimitiveType> filterCriteria;
        List<object> linkedObjects = new List<object>();

        // stores changed rows with their pre-changed/added/removed values
        Dictionary<DataGridViewRow, string> latestChangedRows = new Dictionary<DataGridViewRow, string>(),
                                            latestAddedRows = new Dictionary<DataGridViewRow, string>(),
                                            latestRemovedRows = new Dictionary<DataGridViewRow, string>();

        Dictionary<DataGridViewRow, PrimitiveType> associatedPrimitives =
            new Dictionary<DataGridViewRow, PrimitiveType>();

        Dictionary<DataGridViewColumn, ColumnConstraintsType> columnsConstraints;

        public SchedulingPrimitivesGrigView(
            Control parent,
            Dictionary<DataGridViewColumn, ColumnConstraintsType> columnsConstraints,
            Dictionary<DataGridViewColumn, Func<PrimitiveType, string>> fillRules,
            Dictionary<DataGridViewColumn, Action<string, PrimitiveType>> parseRules)
        {
            if (fillRules.Keys.Any(c => !columnsConstraints.Keys.Contains(c)))
            {
                throw new ArgumentException("Column does not belong to the grid", "fillRules");
            }

            if (parseRules != null && parseRules.Keys.Any(c => !columnsConstraints.Keys.Contains(c)))
            {
                throw new ArgumentException("Column does not belong to the grid", "fillRules");
            }

            this.fillRules = fillRules;
            this.parseRules = parseRules;
            this.columnsConstraints = columnsConstraints;

            parent.Controls.Add(this);
            Dock = DockStyle.Fill;
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            EditMode = DataGridViewEditMode.EditOnKeystroke;
            DoubleBuffered = true;
            Columns.AddRange(columnsConstraints.Keys.ToArray());
        }

        #endregion

        public bool DataValid
        {
            get;
            private set;
        }

        public IEnumerable<PrimitiveType> Items
        {
            get { return associatedPrimitives.Values.Where(p => !p.IsEmpty); }
        }

        public void SetCellValue(DataGridViewCell cell, string value)
        {
            cell.Value = value;
            parseRules[cell.OwningColumn](value, associatedPrimitives[cell.OwningRow]);
        }

        void ValidateRowData(DataGridViewRow validatedRow)
        {
            if (!validatedRow.IsNewRow)
            {
                var valid = validatedRow.Cells.OfType<DataGridViewCell>().Select(IsCellValueValid).ToArray();
                for (var i = 0; i < valid.Length; i++)
                {
                    if (!valid[i])
                    {
                        DataValid = false;
                    }
                    HighlinghtCell(validatedRow.Cells[i], valid[i]);
                }
            }
        }

        void ValidateWholeData()
        {
            DataValid = true;
            foreach (var r in Rows.OfType<DataGridViewRow>())
            {
                ValidateRowData(r);
            }
        }

        bool IsCellValueValid(DataGridViewCell cell)
        {
            var val = cell.Value as string;
            switch (columnsConstraints[cell.OwningColumn])
            {
                case ColumnConstraintsType.NotEmpty:
                    return !string.IsNullOrWhiteSpace(val) && val != EditDataForm.UNDEFINED_COMBOBOX_VALUE;
                case ColumnConstraintsType.UniqueValues:
                    return Rows.OfType<DataGridViewRow>()
                                .Where(r => r.Visible && r != cell.OwningRow)
                                .All(r => r.Cells[cell.OwningColumn.Name].Value as string != val);
                default:
                    return true;
            }
        }

        public void FillGrid(IEnumerable<PrimitiveType> source)
        {
            Rows.Clear();
            ClearChangesCollections();
            foreach (PrimitiveType elem in source)
            {
                var i = Rows.Add();
                var newRow = Rows[i];
                foreach (var fillRule in fillRules)
                {
                    newRow.Cells[fillRule.Key.Name].Value = fillRule.Value(elem);
                }
                elem.CopyTo(associatedPrimitives[newRow]);
                latestAddedRows[newRow] = elem.ToString();
            }
            AutoResizeColumns();
            bool valid = RefreshLinkedObjectsData();
            HighlightParentText(valid);
        }

        public Predicate<PrimitiveType> ShowFilter
        {
            set
            {
                filterCriteria = value;
                if (filterCriteria != null)
                {
                    SuspendLayout();
                    foreach (DataGridViewRow row in Rows)
                    {
                        row.Visible = row.IsNewRow || filterCriteria(associatedPrimitives[row]);
                    }
                    ResumeLayout();
                }
            }
        }

        public void LinkControl(DataGridViewComboBoxColumn comboBoxColumn)
        {
            linkedObjects.Add(comboBoxColumn);
        }

        public void LinkControl(ComboBox comboBox)
        {
            linkedObjects.Add(comboBox);
        }

        bool RefreshLinkedObjectsData()
        {
            ValidateWholeData();
            if (DataValid)
            {
                foreach (var cbxCol in linkedObjects.OfType<DataGridViewComboBoxColumn>())
                {
                    RefreshLinkedComboboxColumnData(cbxCol);
                }
                foreach (var cbx in linkedObjects.OfType<ComboBox>())
                {
                    RefreshLinkedComboboxData(cbx);
                }
                ClearChangesCollections();
                return true;
            }
            else return false;
        }

        private void ClearChangesCollections()
        {
            latestRemovedRows.Clear();
            latestChangedRows.Clear();
            latestAddedRows.Clear();
        }

        void RefreshLinkedComboboxData(ComboBox cbx)
        {
            var prevSelection = cbx.SelectedItem as string;
            cbx.Items.Clear();
            cbx.Items.AddRange(this.Items.Select(p => p.ToString()).ToArray());
            if (cbx.Items.OfType<string>().Contains(prevSelection))
            {
                cbx.SelectedItem = prevSelection;
            }
            else if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = 0;
            }
        }

        void RefreshLinkedComboboxColumnData(DataGridViewComboBoxColumn col)
        {
            foreach (var pair in latestRemovedRows.Where(r => !r.Key.IsNewRow))
            {
                var removedValue = pair.Value;
                foreach (DataGridViewRow row in col.DataGridView.Rows)
                {
                    if ((string)row.Cells[col.Name].Value == removedValue)
                        row.Cells[col.Name].Value = EditDataForm.UNDEFINED_COMBOBOX_VALUE;
                }
                col.Items.Remove(removedValue);
            }

            foreach (var pair in latestChangedRows.Where(r => !r.Key.IsNewRow))
            {
                var oldValue = pair.Value;
                var newValue = associatedPrimitives[pair.Key].ToString();
                col.Items.Add(newValue);
                foreach (DataGridViewRow row in col.DataGridView.Rows)
                {
                    if ((string)row.Cells[col.Name].Value == oldValue)
                        row.Cells[col.Name].Value = newValue;
                }
                col.Items.Remove(oldValue);
            }

            foreach (var pair in latestAddedRows.Where(r => !r.Key.IsNewRow))
            {
                var newValue = pair.Value;
                col.Items.Add(newValue);
            }
        }

        #region Overrides

        protected override void OnRowsAdded(DataGridViewRowsAddedEventArgs e)
        {
            for (var r = e.RowIndex; r < e.RowIndex + e.RowCount; r++)
            {
                var row = Rows[r];
                foreach (var cell in row.Cells.OfType<DataGridViewComboBoxCell>())
                {
                    cell.Value = EditDataForm.UNDEFINED_COMBOBOX_VALUE;
                }
                // add an empty primitive for each new row
                var elem = new PrimitiveType();
                associatedPrimitives.Add(row, elem);
                latestAddedRows.Add(row, elem.ToString());

                ValidateWholeData();
                //ValidateRowData(row);
            }
            base.OnRowsAdded(e);
        }

        protected override void OnUserDeletingRow(DataGridViewRowCancelEventArgs e)
        {             
            var row = e.Row;
            if (latestChangedRows.ContainsKey(row))
                latestChangedRows.Remove(row);
            if (latestAddedRows.ContainsKey(row))
                latestAddedRows.Remove(row);
            latestRemovedRows.Add(row, associatedPrimitives[row].ToString());
            associatedPrimitives.Remove(row);
            base.OnUserDeletingRow(e);
        }

        protected override void OnUserDeletedRow(DataGridViewRowEventArgs e)
        {
            ValidateWholeData();
            base.OnUserDeletedRow(e);
        }

        protected override void OnCellEndEdit(DataGridViewCellEventArgs e)
        {
            var row = Rows[e.RowIndex];
            var cell = row.Cells[e.ColumnIndex];

            if (row.Cells[0].Value == null)
            {
                var ids = associatedPrimitives.Values.Select(p => p.Id);
                var id = ids.Any() ? ids.Max() + 1 : 0;
                associatedPrimitives[row].Id = id;
            }

            if (!latestChangedRows.ContainsKey(row) && !latestAddedRows.ContainsKey(row))
                latestChangedRows.Add(row, associatedPrimitives[row].ToString());

            var parseRule = parseRules[Columns[e.ColumnIndex]];
            var val = cell.Value as string;
            if (val != EditDataForm.UNDEFINED_COMBOBOX_VALUE)
                parseRule(val, associatedPrimitives[row]);

            var isValid = IsCellValueValid(cell);
            if(!isValid)
            {
                DataValid = false;
            }
            ValidateWholeData();
            //HighlinghtCell(cell, isValid);

            base.OnCellEndEdit(e);
        }

        void HighlinghtCell(DataGridViewCell cell, bool valid)
        {
            if (!cell.OwningRow.IsNewRow)
            {
                cell.Style.BackColor = valid ? Color.White : Color.Red;
                InvalidateCell(cell);
            }
        }

        void HighlightParentText(bool dataValid)
        {
            //if (dataValid)
            //{
            //    Parent.ForeColor = Color.Black;
            //}
            //else
            //{
            //    Parent.ForeColor = Color.Red;
            //}
        }

        protected override void OnDataError(bool displayErrorDialogIfNoHandler, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(string.Format("DataError in cell[{0}, {1}] ({2}):\n{3}",
                                          e.RowIndex, e.ColumnIndex, typeof(PrimitiveType), e.Exception.Message));
            base.OnDataError(false, e);
        }

        protected override void OnLeave(EventArgs e)
        {
            var done = RefreshLinkedObjectsData();
            HighlightParentText(DataValid);
            base.OnLeave(e);
        }

        protected override void OnEnter(EventArgs e)
        {
            HighlightParentText(DataValid);
            base.OnEnter(e);
        }

        #endregion
    }
}