using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TTNhomWF
{
    public partial class KhoHang : Form
    {
        private string Trangthai = "LOAD";
        private String connecttionString = @"Data Source=CHIEN\SQLEXPRESS;Initial Catalog=QuanlyXuat_Nhapkho;Integrated Security=True";
        private SqlConnection conn;
        private SqlDataAdapter da;
        private SqlCommand cmd;
        private DataTable dt;
        private string sql = "";
        public KhoHang()
        {

            InitializeComponent();
            dgvKhoHang.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvKhoHang.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void KhoHang_Load(object sender, EventArgs e)
        {
            TrangThaiButton(true);
            TrangThaiTXT(false);
            conn = new SqlConnection(connecttionString);
            try
            {
                conn.Open();
                sql = "SELECT * FROM tbl_KhoHang";
                HienThi(sql);

            }
            catch (Exception ex)
            {

                MessageBox.Show("Lỗi " + ex, "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            }
        }

        private void KhoHang_FormClosing(object sender, FormClosingEventArgs e)
        {
            conn.Close();
        }
        #region [ các hàm dùng chung cho tất cả các FORM]
        private void TrangThaiButton(bool b)
        {
            btnThem.Enabled = b;
            btnSua.Enabled = b;
            btnXoa.Enabled = b;
            btnLuu.Enabled = !b;
            btnReset.Enabled = !b;
        }
        private void ClearTXT()
        {
            txtMakho.Text = "";
            txtTenkho.Text = "";
        }
        private void TrangThaiTXT(bool b)
        {
            txtTenkho.Enabled = b;
        }
        private void HienThi(string sql)
        {
            try
            {

                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvKhoHang.DataSource = dt;
            }
            catch (Exception ex)
            {

                MessageBox.Show("Lỗi " + ex, "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            }
        }
        private bool KiemTraTrungDL(string sql)
        {
            bool bKiemtra = false;
            try
            {

                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    bKiemtra = true;
                }
                dt.Dispose();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Lỗi " + ex, "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            }
            return bKiemtra;
        }
        private string SinhMaTuDong(string ma)
        {
            string Matusinh = "";
            int count = 0;
            count = dgvKhoHang.Rows.Count; //lấy số dòng của dgv.
            int chuoiSo = 0;
            string ChuoiMa = Convert.ToString(dgvKhoHang.Rows[count - 2].Cells[0].Value);
            chuoiSo = Convert.ToInt32(ChuoiMa.Replace(ma, ""));
            if (chuoiSo + 1 < 10)
            {
                Matusinh = ma + "00" + (chuoiSo + 1).ToString();
            }
            else if (chuoiSo + 1 < 100)
            {
                Matusinh = ma + "0" + (chuoiSo + 1).ToString();
            }

            return Matusinh;
        }
        #endregion

        private void btnThem_Click(object sender, EventArgs e)
        {
            Trangthai = "Them";
            ClearTXT();
            TrangThaiTXT(true);
            TrangThaiButton(false);
            dgvKhoHang.Enabled = false;
            txtMakho.Text = SinhMaTuDong("mk");
            txtTenkho.Focus();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            Trangthai = "Sua";
            txtTenkho.Enabled = true;
            TrangThaiButton(false);
            dgvKhoHang.Enabled = false;
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn xóa không ?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {

                cmd = new SqlCommand("DELETEKhoHang", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@makho", txtMakho.Text);
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Cập nhật dữ liệu thành công !", "THÔNG BÁO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Lỗi " + ex, "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                }

                // load lại dữ liệu trong datagridview.
                sql = "SELECT * FROM tbl_KhoHang";
                HienThi(sql);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
             string sMakho = "";
            if (Trangthai == "Them")
            {
                // Kiểm tra xem mã đã có chưa trước khi thêm.
                sMakho = txtMakho.Text;
                sql = "SELECT * FROM tbl_KhoHang WHERE Makho = '" + sMakho +"'";
                if (KiemTraTrungDL(sql))
                {
                    MessageBox.Show("Mã kho đã tồn tại !", "THÔNG BÁO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMakho.Focus();
                    txtMakho.SelectAll();
                    return;
                }
                // lấy dữa liệu từ text để lưu vào csdl.
                cmd = new SqlCommand("ThemKhoHang",conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@makho", txtMakho.Text);
                cmd.Parameters.AddWithValue("@tenkho", txtTenkho.Text);
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Cập nhật dữ liệu thành công !", "THÔNG BÁO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Lỗi " + ex, "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                }

            }
            else if (Trangthai == "Sua")
            {
                cmd = new SqlCommand("EDITKhoHang", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@makho", txtMakho.Text);
                cmd.Parameters.AddWithValue("@tenkho", txtTenkho.Text);
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Cập nhật dữ liệu thành công !", "THÔNG BÁO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Lỗi " + ex, "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                }

            }
            btnReset_Click(sender, e);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            TrangThaiTXT(false);
            TrangThaiButton(true);
            dgvKhoHang.Enabled = true;
            dgvKhoHang.CurrentCell = dgvKhoHang[0, 0];
            dgvKhoHang_SelectionChanged(sender, e);
            sql = "SELECT Makho AS 'Mã kho', Tenkho AS 'Tên Kho' FROM tbl_KhoHang";
            txtTim.Clear();
            HienThi(sql);
        }

        private void dgvKhoHang_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvKhoHang.SelectedCells.Count > 0 && dgvKhoHang.SelectedRows.Count > 0)
            {
                int Index = dgvKhoHang.CurrentCell.RowIndex;
                txtMakho.Text = dgvKhoHang.Rows[Index].Cells[0].Value.ToString();
                txtTenkho.Text = dgvKhoHang.Rows[Index].Cells[1].Value.ToString();
            }
            else
            {
                ClearTXT();
            }
        }

        private void ckLuaChon_CheckedChanged(object sender, EventArgs e)
        {
            if (ckLuaChon.Checked == true)
            {
                btnTim.Enabled = true;
                txtTim.Enabled = true;
                btnReset.Enabled = true;
            }
            else
            {
                btnTim.Enabled = false;
                txtTim.Enabled = false;
                btnReset.Enabled = false;
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            cmd = new SqlCommand("TimKiem", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@makho", txtTim.Text);
            da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            dgvKhoHang.DataSource = dt;
        }

        

    }
}
