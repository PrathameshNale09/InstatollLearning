using InstaTollLearningApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InstaTollLearningApp
{
    public partial class RecordsForm : Form
    {

        private readonly int _userId;
        private readonly RecordService _recordService;
        public RecordsForm(int userId)
        {
            InitializeComponent();
            _userId = userId;
            _recordService = new RecordService();


            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnRefresh.Click += btnRefresh_Click;
            dgvRecords.CellClick += dgvRecords_CellClick;

            LoadRecords();
        }

        private void LoadRecords()
        {
            try
            {
                dgvRecords.DataSource = _recordService.GetAll(_userId);
                dgvRecords.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load records. Please try again.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        private bool ValidateRecordInput()
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Please enter a title.");
                txtTitle.Focus();
                return false;
            }

            if (txtTitle.Text.Trim().Length > 100)
            {
                MessageBox.Show("Title cannot exceed 100 characters.");
                return false;
            }

            if (txtDescription.Text.Length > 500)
            {
                MessageBox.Show("Description cannot exceed 500 characters.");
                return false;
            }

            return true;
        }

        private int GetSelectedRecordId()
        {
            if (dgvRecords.CurrentRow == null ||
                dgvRecords.CurrentRow.IsNewRow)
            {
                return 0;
            }

            return Convert.ToInt32(
                dgvRecords.CurrentRow.Cells["RecordId"].Value);
        }

        private void ClearInputs()
        {
            txtTitle.Clear();
            txtDescription.Clear();
            dgvRecords.ClearSelection();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateRecordInput())
                return;

            try
            {
                _recordService.Create(
                    txtTitle.Text.Trim(),
                    txtDescription.Text.Trim(),
                    _userId);

                MessageBox.Show("Record created successfully.");
                ClearInputs();
                LoadRecords();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to create record.\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!ValidateRecordInput())
                return;

            int recordId = GetSelectedRecordId();

            if (recordId == 0)
            {
                MessageBox.Show("Please select a record to update.");
                return;
            }

            try
            {
                _recordService.Update(
                    recordId,
                    txtTitle.Text.Trim(),
                    txtDescription.Text.Trim(),
                    _userId);

                MessageBox.Show("Record updated successfully.");
                ClearInputs();
                LoadRecords();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to update record.\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int recordId = GetSelectedRecordId();

            if (recordId == 0)
            {
                MessageBox.Show("Please select a record to delete.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this record?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                _recordService.Delete(recordId, _userId);

                MessageBox.Show("Record deleted successfully.");
                ClearInputs();
                LoadRecords();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to delete record.\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadRecords();
        }

        private void dgvRecords_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = dgvRecords.Rows[e.RowIndex];

            if (row.IsNewRow)
                return;

            txtTitle.Text = Convert.ToString(row.Cells["Title"].Value);
            txtDescription.Text =
                Convert.ToString(row.Cells["Description"].Value);
        }



        private void RecordsForm_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnDelete_Click_1(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click_1(object sender, EventArgs e)
        {

        }

        private void btnRefresh_Click_1(object sender, EventArgs e)
        {

        }

        private void btnUpdate_Click_1(object sender, EventArgs e)
        {

        }
    }
}
