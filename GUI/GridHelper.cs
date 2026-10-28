using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyBanDienThoai.GUI
{
    // Hàm dùng chung cho DataGridView của cả nhóm
    public static class GridHelper
    {
        // Cấu hình lưới chỉ để xem: không cho sửa trực tiếp, chọn nguyên dòng
        public static void CauHinh(DataGridView dgv)
        {
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.MultiSelect = false;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dgv.BackgroundColor = Color.White;
        }

        // Định dạng cột ngày (dd/MM/yyyy) và cột tiền (#,##0)
        public static void DinhDang(DataGridView dgv)
        {
            foreach (DataGridViewColumn c in dgv.Columns)
            {
                Type t = c.ValueType;
                if (t == null) continue;
                t = Nullable.GetUnderlyingType(t) ?? t;
                if (t == typeof(DateTime))
                {
                    c.DefaultCellStyle.Format = "dd/MM/yyyy";
                }
                else if (t == typeof(decimal))
                {
                    c.DefaultCellStyle.Format = "#,##0";
                    c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }
        }

        // Gán ArrayList DTO vào lưới. Dùng BindingList<kiểu> để lưới vẫn có đủ cột khi danh sách rỗng.
        public static void GanDuLieu(DataGridView dgv, ArrayList ds, Type kieu)
        {
            Type loai = typeof(BindingList<>).MakeGenericType(kieu);
            IList danhSach = (IList)Activator.CreateInstance(loai);
            if (ds != null)
            {
                foreach (object o in ds) danhSach.Add(o);
            }
            dgv.DataSource = danhSach;
            DinhDang(dgv);
        }

        // Bỏ chọn dòng hiện tại (để bấm lại dòng đầu vẫn nhận sự kiện chọn)
        public static void BoChon(DataGridView dgv)
        {
            dgv.CurrentCell = null;
            dgv.ClearSelection();
        }
    }
}
