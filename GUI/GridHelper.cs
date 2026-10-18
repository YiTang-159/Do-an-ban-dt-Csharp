using System;
using System.Windows.Forms;

namespace QuanLyBanDienThoai.GUI
{
    public static class GridHelper
    {
        // Định dạng ngày và số tiền cho các cột trên lưới
        public static void DinhDang(DataGridView dgv)
        {
            foreach (DataGridViewColumn c in dgv.Columns)
            {
                if (c.ValueType == typeof(DateTime)) c.DefaultCellStyle.Format = "dd/MM/yyyy";
                else if (c.ValueType == typeof(decimal)) c.DefaultCellStyle.Format = "#,##0.##";
            }
        }
    }
}
