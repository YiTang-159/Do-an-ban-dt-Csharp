using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanDienThoai.BUS;
using QuanLyBanDienThoai.DTO;
namespace QuanLyBanDienThoai.GUI
{
    // Main-sub: trên là khuyến mãi, dưới là các điện thoại được giảm giá.
    public class FormKhuyenMai:Form
    {
        private readonly KhuyenMaiBUS bus=new KhuyenMaiBUS();
        private readonly ChiTietKhuyenMaiBUS ctBus=new ChiTietKhuyenMaiBUS();
        private readonly ListView grid=ListViewHelper.Tao(),gridCT=ListViewHelper.Tao();
        private readonly TextBox txtTim=new TextBox{Width=165},txtTen=new TextBox(),txtMoTa=new TextBox();
        private readonly DateTimePicker dtTu=new DateTimePicker{Format=DateTimePickerFormat.Short},dtDen=new DateTimePicker{Format=DateTimePickerFormat.Short};
        private int selectedId;
        public FormKhuyenMai()
        {
            Tv4FormHelper.KhoiTao(this,"Quản lý khuyến mãi");
            FlowLayoutPanel toolbar=new FlowLayoutPanel{Dock=DockStyle.Top,Height=46,AutoScroll=true,WrapContents=false};
            toolbar.Controls.Add(new Label{Text="Tìm kiếm",AutoSize=true,Padding=new Padding(5,12,0,0)});
            toolbar.Controls.Add(txtTim);
            toolbar.Controls.Add(Tv4FormHelper.Nut("Thêm",(s,e)=>Luu(false)));
            toolbar.Controls.Add(Tv4FormHelper.Nut("Sửa",(s,e)=>Luu(true)));
            toolbar.Controls.Add(Tv4FormHelper.Nut("Xóa",(s,e)=>Xoa()));
            toolbar.Controls.Add(Tv4FormHelper.Nut("Làm mới",(s,e)=>LamMoi()));
            toolbar.Controls.Add(Tv4FormHelper.Nut("Tìm nâng cao",(s,e)=> {
                try {using(var f=new FormTimNangCao(typeof(KhuyenMai))) {if(f.ShowDialog()==DialogResult.OK) {Hien(bus.TimNangCao(f.KetQua));XoaChiTiet();}}}catch(Exception ex){Tv4FormHelper.Loi(ex);}
            },128));
            toolbar.Controls.Add(Tv4FormHelper.Nut("Xuất Excel",(s,e)=>ExcelHelper.XuatCoHopThoai(bus.GetAll(),typeof(KhuyenMai),"KhuyenMai"),100));
            toolbar.Controls.Add(Tv4FormHelper.Nut("Nhập Excel",(s,e)=>NhapExcel(),100));
            FlowLayoutPanel info=new FlowLayoutPanel{Dock=DockStyle.Top,Height=80,AutoScroll=true,WrapContents=false};
            info.Controls.Add(Tv4FormHelper.Field("Tên khuyến mãi",txtTen,245));
            info.Controls.Add(Tv4FormHelper.Field("Ngày bắt đầu",dtTu,155));
            info.Controls.Add(Tv4FormHelper.Field("Ngày kết thúc",dtDen,155));
            info.Controls.Add(Tv4FormHelper.Field("Mô tả",txtMoTa,280));
            SplitContainer split=new SplitContainer{Dock=DockStyle.Fill,Size=new Size(900,600),Orientation=Orientation.Horizontal,SplitterDistance=260};
            Panel upper=new Panel{Dock=DockStyle.Fill};
            upper.Controls.Add(grid);
            upper.Controls.Add(new Label{Text="DANH SÁCH KHUYẾN MÃI",Dock=DockStyle.Top,Height=25,Font=new Font("Segoe UI",10,FontStyle.Bold)});
            Panel lower=new Panel{Dock=DockStyle.Fill};
            lower.Controls.Add(gridCT);
            lower.Controls.Add(new Label{Text="CHI TIẾT SẢN PHẨM ÁP DỤNG (chọn dòng phía trên)",Dock=DockStyle.Top,Height=25,Font=new Font("Segoe UI",10,FontStyle.Bold)});
            split.Panel1.Controls.Add(upper);split.Panel2.Controls.Add(lower);
            Controls.Add(split);Controls.Add(info);Controls.Add(toolbar);
            txtTim.TextChanged+=(s,e)=>Tai();
            grid.SelectedIndexChanged+=(s,e)=>Chon();
            Tai();
        }
        private void Hien(ArrayList ds)=>ListViewHelper.Gan(grid,ds,"MaKM|Mã KM","TenKM|Tên khuyến mãi","NgayBatDau|Ngày bắt đầu","NgayKetThuc|Ngày kết thúc","MoTa|Mô tả");
        private void HienChiTiet(ArrayList ds)=>ListViewHelper.Gan(gridCT,ds,"MaKM|Mã KM","MaDT|Mã điện thoại","TenHienThi|Điện thoại","PhanTramGiam|Giảm (%)");
        private void XoaChiTiet(){selectedId=0;HienChiTiet(new ArrayList());}
        private void Tai()
        {
            try {Hien(bus.TimKiem(txtTim.Text));XoaChiTiet();}
            catch(Exception ex){Tv4FormHelper.Loi(ex);}
        }
        private void Chon()
        {
            KhuyenMai x=ListViewHelper.Chon<KhuyenMai>(grid);if(x==null){XoaChiTiet();return;}
            selectedId=x.MaKM;txtTen.Text=x.TenKM;txtMoTa.Text=x.MoTa;
            dtTu.Value=x.NgayBatDau;dtDen.Value=x.NgayKetThuc;
            try{HienChiTiet(ctBus.GetTheoKhuyenMai(x.MaKM));}
            catch(Exception ex){Tv4FormHelper.Loi(ex);}
        }
        private void Luu(bool sua)
        {
            if(sua&&selectedId<=0){MessageBox.Show("Chọn khuyến mãi cần sửa");return;}
            KhuyenMai x=new KhuyenMai{MaKM=sua?selectedId:0,TenKM=txtTen.Text,
                NgayBatDau=dtTu.Value.Date,NgayKetThuc=dtDen.Value.Date,MoTa=txtMoTa.Text};
            Tv4FormHelper.ThongBao(sua?bus.Sua(x):bus.Them(x),LamMoi);
        }
        private void Xoa()
        {
            if(selectedId<=0){MessageBox.Show("Chọn khuyến mãi cần xóa");return;}
            if(MessageBox.Show("Xóa khuyến mãi đã chọn?","Xác nhận",MessageBoxButtons.YesNo)!=DialogResult.Yes)return;
            Tv4FormHelper.ThongBao(bus.Xoa(selectedId),LamMoi);
        }
        private void LamMoi()
        {
            selectedId=0;txtTen.Clear();txtMoTa.Clear();dtTu.Value=DateTime.Today;dtDen.Value=DateTime.Today.AddMonths(1);Tai();
        }
        private void NhapExcel()
        {
            ArrayList ds=ExcelHelper.NhapCoHopThoai(typeof(KhuyenMai));if(ds==null)return;
            int so;string err=bus.NhapDanhSach(ds,out so);Tai();
            MessageBox.Show("Đã nhập "+so+" dòng. "+(err==""?"": "Lỗi:\n"+err));
        }
    }
}
