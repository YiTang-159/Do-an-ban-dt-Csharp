using System;
using System.Collections;
using System.Windows.Forms;
using QuanLyBanDienThoai.BUS;
using QuanLyBanDienThoai.DTO;
namespace QuanLyBanDienThoai.GUI
{
    public class FormChiTietKhuyenMai:Form
    {
        private readonly ChiTietKhuyenMaiBUS bus=new ChiTietKhuyenMaiBUS();
        private readonly KhuyenMaiBUS kmBus=new KhuyenMaiBUS();
        private readonly DienThoaiBUS dtBus=new DienThoaiBUS();
        private readonly DataGridView grid=Tv4FormHelper.Luoi();
        private readonly ComboBox cboKM=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList,Width=215};
        private readonly ComboBox cboDT=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList,Width=280};
        private readonly NumericUpDown num=new NumericUpDown{DecimalPlaces=2,Minimum=0,Maximum=100,Increment=1,Width=120};
        private readonly TextBox txtTim=new TextBox{Width=170};
        private int maKM,maDT;
        public FormChiTietKhuyenMai()
        {
            Tv4FormHelper.KhoiTao(this,"Chi tiết khuyến mãi");
            FlowLayoutPanel bar=new FlowLayoutPanel{Dock=DockStyle.Top,Height=46,WrapContents=false,AutoScroll=true};
            bar.Controls.Add(new Label{Text="Tìm",AutoSize=true,Padding=new Padding(4,12,0,0)});bar.Controls.Add(txtTim);
            bar.Controls.Add(Tv4FormHelper.Nut("Thêm",(s,e)=>Luu(false)));
            bar.Controls.Add(Tv4FormHelper.Nut("Sửa",(s,e)=>Luu(true)));
            bar.Controls.Add(Tv4FormHelper.Nut("Xóa",(s,e)=>Xoa()));
            bar.Controls.Add(Tv4FormHelper.Nut("Làm mới",(s,e)=>LamMoi()));
            bar.Controls.Add(Tv4FormHelper.Nut("Tìm nâng cao",(s,e)=>{
                string err=Tv4FormHelper.TimNangCao(typeof(ChiTietKhuyenMai),bus.TimNangCao,grid);maKM=maDT=0;if(err!="")MessageBox.Show(err);
            },130));
            bar.Controls.Add(Tv4FormHelper.Nut("Xuất Excel",(s,e)=>ExcelHelper.XuatCoHopThoai(bus.GetAll(),typeof(ChiTietKhuyenMai),"ChiTietKhuyenMai")));
            bar.Controls.Add(Tv4FormHelper.Nut("Nhập Excel",(s,e)=>NhapExcel()));
            FlowLayoutPanel form=new FlowLayoutPanel{Dock=DockStyle.Top,Height=80,WrapContents=false,AutoScroll=true};
            form.Controls.Add(Tv4FormHelper.Field("Khuyến mãi",cboKM,225));
            form.Controls.Add(Tv4FormHelper.Nut("...",(s,e)=>ChonKM(),42));
            form.Controls.Add(Tv4FormHelper.Field("Điện thoại",cboDT,290));
            form.Controls.Add(Tv4FormHelper.Nut("...",(s,e)=>ChonDT(),42));
            form.Controls.Add(Tv4FormHelper.Field("Giảm (%)",num,130));
            Controls.Add(grid);Controls.Add(form);Controls.Add(bar);
            txtTim.TextChanged+=(s,e)=>Tai();grid.SelectionChanged+=(s,e)=>Chon();
            NapCombo();Tai();
        }
        private void NapCombo()
        {
            cboKM.DataSource=kmBus.GetAll();cboKM.DisplayMember="TenKM";cboKM.ValueMember="MaKM";
            cboDT.DataSource=dtBus.GetAll();cboDT.DisplayMember="TenHienThi";cboDT.ValueMember="MaDT";
        }
        private void Tai()
        {
            try{Tv4FormHelper.Hien(grid,bus.TimKiem(txtTim.Text),typeof(ChiTietKhuyenMai));maKM=maDT=0;cboKM.Enabled=true;cboDT.Enabled=true;}
            catch(Exception ex){Tv4FormHelper.Loi(ex);}
        }
        private void Chon()
        {
            if(grid.CurrentRow?.DataBoundItem is not ChiTietKhuyenMai x)return;
            maKM=x.MaKM;maDT=x.MaDT;cboKM.SelectedValue=maKM;cboDT.SelectedValue=maDT;
            num.Value=Math.Min(100,Math.Max(0,x.PhanTramGiam));
            cboKM.Enabled=false;cboDT.Enabled=false;
        }
        private void ChonKM()
        {
            if(!cboKM.Enabled)return;
            object o=FormChon.Chon(kmBus.GetAll(),new[]{"MaKM","TenKM"},"Chọn khuyến mãi");
            if(o is KhuyenMai x)cboKM.SelectedValue=x.MaKM;
        }
        private void ChonDT()
        {
            if(!cboDT.Enabled)return;
            object o=FormChon.Chon(dtBus.GetAll(),new[]{"MaDT","TenHienThi"},"Chọn điện thoại");
            if(o is DienThoai x)cboDT.SelectedValue=x.MaDT;
        }
        private void Luu(bool sua)
        {
            if(sua&&(maKM==0||maDT==0)){MessageBox.Show("Chọn chi tiết cần sửa");return;}
            ChiTietKhuyenMai x=new ChiTietKhuyenMai { MaKM=sua?maKM:Convert.ToInt32(cboKM.SelectedValue),
                MaDT=sua?maDT:Convert.ToInt32(cboDT.SelectedValue),PhanTramGiam=num.Value };
            Tv4FormHelper.ThongBao(sua?bus.Sua(x):bus.Them(x),LamMoi);
        }
        private void Xoa()
        {
            if(maKM==0||maDT==0){MessageBox.Show("Chọn chi tiết cần xóa");return;}
            if(MessageBox.Show("Xóa chi tiết khuyến mãi?","Xác nhận",MessageBoxButtons.YesNo)!=DialogResult.Yes)return;
            Tv4FormHelper.ThongBao(bus.Xoa(maKM,maDT),LamMoi);
        }
        private void LamMoi()
        {
            maKM=maDT=0;cboKM.Enabled=true;cboDT.Enabled=true;num.Value=0;NapCombo();Tai();
        }
        private void NhapExcel()
        {
            ArrayList ds=ExcelHelper.NhapCoHopThoai(typeof(ChiTietKhuyenMai));if(ds==null)return;
            int n;string err=bus.NhapDanhSach(ds,out n);Tai();
            MessageBox.Show("Đã nhập "+n+" dòng."+(err==""?"":"\n"+err));
        }
    }
}
