using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using ShedulerProject.Core;

namespace ShedulerProject.UserInterface
{
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

        public SchedulingPrimitivesGrigView(
            Control parent,
            IEnumerable<DataGridViewColumn> columns,
            Dictionary<DataGridViewColumn, Func<PrimitiveType, string>> fillRules,
            Dictionary<DataGridViewColumn, Action<string, PrimitiveType>> parseRules)
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
            EditMode = DataGridViewEditMode.EditOnEnter;
            DoubleBuffered = true;
            Columns.AddRange(columns.ToArray());
        }

        #endregion

        public IEnumerable<PrimitiveType> Items
        {
            get { return associatedPrimitives.Values.Where(p => !p.IsEmpty); }
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
            RefreshLinkedObjectsData();
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

        void RefreshLinkedObjectsData()
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
        }

        private void ClearChangesCollections()
        {
            latestRemovedRows.Clear();
            latestChangedRows.Clear();
            latestAddedRows.Clear();
        }

        void RefreshLinkedComboboxData(ComboBox cbx)
        {
            cbx.Items.Clear();
            cbx.Items.AddRange(this.Items.Select(p => p.ToString()).ToArray());
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
            for (var i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
            {
                var row = Rows[i];
                foreach (var cell in row.Cells.OfType<DataGridViewComboBoxCell>())
                {
                    cell.Value = EditDataForm.UNDEFINED_COMBOBOX_VALUE;
                }
                // add an empty primitive for each new row
                var elem = new PrimitiveType();
                associatedPrimitives.Add(row, elem);
                latestAddedRows.Add(row, elem.ToString());
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

        protected override void OnCellEndEdit(DataGridViewCellEventArgs e)
        {
            var row = Rows[e.RowIndex];

            if (row.Cells[0].Value == null)
            {
                var ids = associatedPrimitives.Values.Select(p => p.Id);
                var id = ids.Any() ? ids.Max() + 1 : 0;
                associatedPrimitives[row].Id = id;
                row.Cells[0].Value = id.ToString(); // Implying first row is always id-row. Should be refactored
            }

            if (!latestChangedRows.ContainsKey(row) && !latestAddedRows.ContainsKey(row))
                latestChangedRows.Add(row, associatedPrimitives[row].ToString());
            var parseRule = parseRules[Columns[e.ColumnIndex]];
            var val = row.Cells[e.ColumnIndex].Value as string;
            if (val != EditDataForm.UNDEFINED_COMBOBOX_VALUE)
                parseRule(val, associatedPrimitives[row]);
            base.OnCellEndEdit(e);
        }

        protected override void OnDataError(bool displayErrorDialogIfNoHandler, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(string.Format("DataError in cell[{0}, {1}] ({2}):\n{3}",
                                          e.RowIndex, e.ColumnIndex, typeof(PrimitiveType), e.Exception.Message));
            base.OnDataError(false, e);
        }

        protected override void OnLeave(EventArgs e)
        {
            RefreshLinkedObjectsData();
            base.OnLeave(e);
        }

        #endregion
    }
}