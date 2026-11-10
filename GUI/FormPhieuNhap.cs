using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanDienThoai.BUS;
namespace QuanLyBanDienThoai.GUI
{
    public class FormPhieuNhap:Form
    {
        private readonly PhieuNhapHangBUS bus=new PhieuNhapHangBUS();
        private readonly ListView master=ListViewHelper.Tao(), detail=ListViewHelper.Tao();
        private readonly Label total=new Label{Dock=DockStyle.Bottom,Height=35};
        public FormPhieuNhap()
        {
            Text="Danh sách phiếu nhập và chi tiết";Width=1100;Height=700;
            var refresh=new Button{Text="Làm mới",Dock=DockStyle.Top,Height=36};
            refresh.Click+=(s,e)=>LoadData();
            var split=new SplitContainer{Size=new Size(1000,600),Dock=DockStyle.Fill,Orientation=Orientation.Horizontal,SplitterDistance=300};
            split.Panel1.Controls.Add(master);split.Panel2.Controls.Add(detail);
            Controls.Add(split);Controls.Add(refresh);Controls.Add(total);
            master.SelectedIndexChanged+=(s,e)=>LoadDetail();LoadData();
        }
        private void LoadDetail()
        {
            try {
                DataRow row=ListViewHelper.Chon<DataRow>(master);
                IEnumerable rows=row==null?(IEnumerable)new ArrayList():bus.GetChiTiet(Convert.ToInt32(row["MaPN"])).Rows;
                ListViewHelper.Gan(detail,rows,"MaPN|Mã phiếu", "MaDT|Mã điện thoại", "TenDT|Điện thoại", "SoLuong|Số lượng", "DonGiaNhap|Giá nhập", "ThanhTien|Thành tiền");
            }catch(Exception ex){MessageBox.Show(ex.Message);}
        }
        private void LoadData()
        {
            try {
                var table=bus.GetAll();
                ListViewHelper.Gan(master,table.Rows,"MaPN|Mã phiếu", "NgayNhap|Ngày nhập", "TongTien|Tổng tiền", "TenNCC|Nhà cung cấp", "TenNhanVien|Nhân viên");
                decimal sum=0;foreach(DataRow row in table.Rows)sum+=Convert.ToDecimal(row["TongTien"]);
                total.Text="Tổng tiền nhập: "+sum.ToString("N0")+" đ";LoadDetail();
            }catch(Exception ex){MessageBox.Show(ex.Message);}
        }
    }
}
