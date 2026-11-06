using System;
using System.Collections;
using System.Windows.Forms;
using QuanLyBanDienThoai.BUS;
using QuanLyBanDienThoai.DTO;
namespace QuanLyBanDienThoai.GUI
{
    public class FormBaoHanh:Form
    {
        private readonly BaoHanhBUS bus=new BaoHanhBUS();
        private readonly HoaDonBUS hdBus=new HoaDonBUS();
        private readonly DienThoaiBUS dtBus=new DienThoaiBUS();
        private readonly DataGridView grid=Tv4FormHelper.Luoi();
        private readonly TextBox txtTim=new TextBox{Width=160},txtIMEI=new TextBox{MaxLength=15},txtGhiChu=new TextBox();
        private readonly ComboBox cboHD=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList};
        private readonly ComboBox cboDT=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList};
        private readonly ComboBox cboTrangThai=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList};
        private readonly DateTimePicker dtTu=new DateTimePicker{Format=DateTimePickerFormat.Short};
        private readonly DateTimePicker dtDen=new DateTimePicker{Format=DateTimePickerFormat.Short};
        private int maBH;
        public FormBaoHanh()
        {
            Tv4FormHelper.KhoiTao(this,"Quản lý bảo hành");
            FlowLayoutPanel bar=new FlowLayoutPanel{Dock=DockStyle.Top,Height=46,AutoScroll=true,WrapContents=false};
            bar.Controls.Add(new Label{Text="Tìm IMEI / mã",AutoSize=true,Padding=new Padding(4,12,0,0)});
            bar.Controls.Add(txtTim);
            bar.Controls.Add(Tv4FormHelper.Nut("Thêm",(s,e)=>Luu(false)));
            bar.Controls.Add(Tv4FormHelper.Nut("Sửa",(s,e)=>Luu(true)));
            bar.Controls.Add(Tv4FormHelper.Nut("Xóa",(s,e)=>Xoa()));
            bar.Controls.Add(Tv4FormHelper.Nut("Làm mới",(s,e)=>LamMoi()));
            bar.Controls.Add(Tv4FormHelper.Nut("Tìm nâng cao",(s,e)=> {
                string err=Tv4FormHelper.TimNangCao(typeof(BaoHanh),bus.TimNangCao,grid);maBH=0;if(err!="")MessageBox.Show(err);
            },130));
            bar.Controls.Add(Tv4FormHelper.Nut("Xuất Excel",(s,e)=>ExcelHelper.XuatCoHopThoai(bus.GetAll(),typeof(BaoHanh),"BaoHanh")));
            bar.Controls.Add(Tv4FormHelper.Nut("Nhập Excel",(s,e)=>NhapExcel()));
            FlowLayoutPanel input=new FlowLayoutPanel{Dock=DockStyle.Top,Height=156,AutoScroll=true,WrapContents=true};
            input.Controls.Add(Tv4FormHelper.Field("IMEI (15 số)",txtIMEI,165));
            input.Controls.Add(Tv4FormHelper.Field("Hóa đơn",cboHD,125));
            input.Controls.Add(Tv4FormHelper.Nut("...",(s,e)=>ChonHD(),40));
            input.Controls.Add(Tv4FormHelper.Field("Điện thoại",cboDT,245));
            input.Controls.Add(Tv4FormHelper.Nut("...",(s,e)=>ChonDT(),40));
            input.Controls.Add(Tv4FormHelper.Field("Ngày bắt đầu",dtTu,145));
            input.Controls.Add(Tv4FormHelper.Field("Ngày kết thúc",dtDen,145));
            input.Controls.Add(Tv4FormHelper.Nut("Mặc định tháng",(s,e)=>dtDen.Value=bus.LayNgayKetThucMacDinh(dtTu.Value),145));
            input.Controls.Add(Tv4FormHelper.Field("Trạng thái",cboTrangThai,195));
            input.Controls.Add(Tv4FormHelper.Field("Ghi chú",txtGhiChu,330));
            Controls.Add(grid);Controls.Add(input);Controls.Add(bar);
            cboTrangThai.Items.AddRange(new object[]{"Đang bảo hành","Đã hoàn thành","Hết hạn","Đã từ chối"});
            cboTrangThai.SelectedIndex=0;
            txtTim.TextChanged+=(s,e)=>Tai();grid.SelectionChanged+=(s,e)=>Chon();
            dtTu.ValueChanged+=(s,e)=> {if(maBH==0)dtDen.Value=bus.LayNgayKetThucMacDinh(dtTu.Value);};
            NapCombo();dtDen.Value=bus.LayNgayKetThucMacDinh(dtTu.Value);Tai();
        }
        private void NapCombo()
        {
            cboHD.DataSource=hdBus.GetAll();cboHD.DisplayMember="MaHD";cboHD.ValueMember="MaHD";
            cboDT.DataSource=dtBus.GetAll();cboDT.DisplayMember="TenHienThi";cboDT.ValueMember="MaDT";
        }
        private void Tai()
        {
            try {Tv4FormHelper.Hien(grid,bus.TimKiem(txtTim.Text),typeof(BaoHanh));maBH=0;}
            catch(Exception ex){Tv4FormHelper.Loi(ex);}
        }
        private void Chon()
        {
            if(grid.CurrentRow?.DataBoundItem is not BaoHanh x)return;
            maBH=x.MaBH;txtIMEI.Text=x.SoIMEI;cboHD.SelectedValue=x.MaHD;cboDT.SelectedValue=x.MaDT;
            dtTu.Value=x.NgayBatDau;dtDen.Value=x.NgayKetThuc;
            if(!cboTrangThai.Items.Contains(x.TrangThai))cboTrangThai.Items.Add(x.TrangThai);
            cboTrangThai.SelectedItem=x.TrangThai;txtGhiChu.Text=x.GhiChu;
        }
        private void ChonHD()
        {
            object o=FormChon.Chon(hdBus.GetAll(),new[]{"MaHD","NgayLap","TenKhachHang"},"Chọn hóa đơn");
            if(o is HoaDon x)cboHD.SelectedValue=x.MaHD;
        }
        private void ChonDT()
        {
            object o=FormChon.Chon(dtBus.GetAll(),new[]{"MaDT","TenHienThi"},"Chọn điện thoại");
            if(o is DienThoai x)cboDT.SelectedValue=x.MaDT;
        }
        private void Luu(bool sua)
        {
            if(sua&&maBH<=0){MessageBox.Show("Chọn bảo hành cần sửa");return;}
            if(cboHD.SelectedValue==null||cboDT.SelectedValue==null){MessageBox.Show("Chọn hóa đơn và điện thoại");return;}
            BaoHanh x=new BaoHanh {MaBH=sua?maBH:0,SoIMEI=txtIMEI.Text,
                MaHD=Convert.ToInt32(cboHD.SelectedValue),MaDT=Convert.ToInt32(cboDT.SelectedValue),
                NgayBatDau=dtTu.Value.Date,NgayKetThuc=dtDen.Value.Date,
                TrangThai=Convert.ToString(cboTrangThai.SelectedItem),GhiChu=txtGhiChu.Text};
            Tv4FormHelper.ThongBao(sua?bus.Sua(x):bus.Them(x),LamMoi);
        }
        private void Xoa()
        {
            if(maBH<=0){MessageBox.Show("Chọn phiếu cần xóa");return;}
            if(MessageBox.Show("Xóa phiếu bảo hành đã chọn?","Xác nhận",MessageBoxButtons.YesNo)!=DialogResult.Yes)return;
            Tv4FormHelper.ThongBao(bus.Xoa(maBH),LamMoi);
        }
        private void LamMoi()
        {
            maBH=0;txtIMEI.Clear();txtGhiChu.Clear();
            cboTrangThai.SelectedIndex=0;dtTu.Value=DateTime.Today;dtDen.Value=bus.LayNgayKetThucMacDinh(dtTu.Value);
            NapCombo();Tai();
        }
        private void NhapExcel()
        {
            ArrayList ds=ExcelHelper.NhapCoHopThoai(typeof(BaoHanh));if(ds==null)return;
            int n;string err=bus.NhapDanhSach(ds,out n);Tai();MessageBox.Show("Đã nhập "+n+" dòng."+(err==""?"":"\n"+err));
        }
    }
}
