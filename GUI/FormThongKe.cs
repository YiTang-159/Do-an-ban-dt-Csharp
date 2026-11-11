using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanDienThoai.BUS;
using QuanLyBanDienThoai.DTO;
namespace QuanLyBanDienThoai.GUI
{
    public class FormThongKe:Form
    {
        private readonly ThongKeBUS bus=new ThongKeBUS();
        private readonly DateTimePicker tu=new DateTimePicker{Format=DateTimePickerFormat.Short,Width=120};
        private readonly DateTimePicker den=new DateTimePicker{Format=DateTimePickerFormat.Short,Width=120};
        private readonly ComboBox kieu=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList,Width=145};
        // Chọn kỳ thống kê bất kỳ: tháng (1-12) hoặc quý (1-4) của một năm, hoặc cả năm
        private readonly ComboBox cboKy=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList,Width=90};
        private readonly NumericUpDown numNam=new NumericUpDown{Minimum=2000,Maximum=2100,Value=DateTime.Today.Year,Width=70};
        private readonly Label thu=new Label{AutoSize=true},chi=new Label{AutoSize=true},loi=new Label{AutoSize=true};
        private readonly DataGridView nv=Tv4FormHelper.Luoi(),kh=Tv4FormHelper.Luoi(),sp=Tv4FormHelper.Luoi();
        private KetQuaThongKe data;
        public FormThongKe()
        {
            Tv4FormHelper.KhoiTao(this,"Thống kê thu chi và lợi nhuận");
            FlowLayoutPanel bar=new FlowLayoutPanel{Dock=DockStyle.Top,Height=48,AutoScroll=true,WrapContents=false};
            bar.Controls.Add(new Label{Text="Từ ngày",AutoSize=true,Padding=new Padding(4,12,0,0)});
            bar.Controls.Add(tu);bar.Controls.Add(new Label{Text="Đến ngày",AutoSize=true,Padding=new Padding(4,12,0,0)});
            bar.Controls.Add(den);
            kieu.Items.AddRange(new object[]{"Tùy chọn ngày","Theo tháng","Theo quý","Theo năm"});
            kieu.SelectedIndex=0;bar.Controls.Add(kieu);
            cboKy.Enabled=false;numNam.Enabled=false;bar.Controls.Add(cboKy);
            bar.Controls.Add(new Label{Text="Năm",AutoSize=true,Padding=new Padding(4,12,0,0)});
            bar.Controls.Add(numNam);
            bar.Controls.Add(Tv4FormHelper.Nut("Thống kê",(s,e)=>Tai()));
            bar.Controls.Add(Tv4FormHelper.Nut("Xuất PDF",(s,e)=>InPdf()));
            tu.ValueChanged+=(s,e)=>{data=null;};den.ValueChanged+=(s,e)=>{data=null;};
            FlowLayoutPanel summary=new FlowLayoutPanel{Dock=DockStyle.Top,Height=54,AutoScroll=true};
            foreach(Label lbl in new[]{thu,chi,loi}) {lbl.Font=new Font("Segoe UI",11,FontStyle.Bold);lbl.Margin=new Padding(15,13,25,0);summary.Controls.Add(lbl);}
            TabControl tabs=new TabControl{Dock=DockStyle.Fill};
            ThemTab(tabs,"Theo nhân viên",nv);ThemTab(tabs,"Theo khách hàng",kh);ThemTab(tabs,"Theo sản phẩm",sp);
            Controls.Add(tabs);Controls.Add(summary);Controls.Add(bar);
            kieu.SelectedIndexChanged+=(s,e)=>{NapKy();ChonKhoang();};
            cboKy.SelectedIndexChanged+=(s,e)=>ChonKhoang();
            numNam.ValueChanged+=(s,e)=>ChonKhoang();
            den.Value=DateTime.Today;tu.Value=new DateTime(DateTime.Today.Year,1,1);
            Tai();
        }
        private static void ThemTab(TabControl tabs,string text,DataGridView grid)
        {
            TabPage page=new TabPage(text){Padding=new Padding(8)};
            page.Controls.Add(grid);tabs.TabPages.Add(page);
        }
        private void ChonKhoang()
        {
            int nam=(int)numNam.Value;
            int ky=cboKy.SelectedIndex+1;   // tháng 1-12 hoặc quý 1-4
            DateTime b,e;
            if(kieu.SelectedIndex==1&&ky>=1) {b=new DateTime(nam,ky,1);e=b.AddMonths(1).AddDays(-1);}
            else if(kieu.SelectedIndex==2&&ky>=1) {b=new DateTime(nam,(ky-1)*3+1,1);e=b.AddMonths(3).AddDays(-1);}
            else if(kieu.SelectedIndex==3) {b=new DateTime(nam,1,1);e=new DateTime(nam,12,31);}
            else return;
            tu.Value=b;den.Value=e;data=null;
        }
        // Nạp danh sách tháng/quý theo kiểu thống kê; mặc định chọn tháng/quý hiện tại
        private void NapKy()
        {
            cboKy.Items.Clear();
            int kieuTK=kieu.SelectedIndex;
            cboKy.Enabled=kieuTK==1||kieuTK==2;
            numNam.Enabled=kieuTK!=0;
            if(kieuTK==1) {for(int m=1;m<=12;m++)cboKy.Items.Add("Tháng "+m);cboKy.SelectedIndex=DateTime.Today.Month-1;}
            else if(kieuTK==2) {for(int q=1;q<=4;q++)cboKy.Items.Add("Quý "+q);cboKy.SelectedIndex=(DateTime.Today.Month-1)/3;}
        }
        private static void DinhDang(DataGridView grid,DataTable t)
        {
            grid.DataSource=t;
            foreach(DataGridViewColumn c in grid.Columns)
                if(c.Name!="DoiTuong") {c.DefaultCellStyle.Format="#,##0";c.DefaultCellStyle.Alignment=DataGridViewContentAlignment.MiddleRight;}
            if(grid.Columns.Contains("DoiTuong"))grid.Columns["DoiTuong"].HeaderText="Đối tượng";
        }
        private void Tai()
        {
            string err=bus.LayThongKe(tu.Value,den.Value,out KetQuaThongKe k);
            if(err!="") {MessageBox.Show(err,"Thống kê",MessageBoxButtons.OK,MessageBoxIcon.Warning);data=null;return;}
            data=k;thu.Text="Tổng thu: "+k.TongThu.ToString("#,##0")+" đ";
            chi.Text="Tổng chi: "+k.TongChi.ToString("#,##0")+" đ";
            loi.Text="Lợi nhuận: "+k.LoiNhuan.ToString("#,##0")+" đ";
            DinhDang(nv,k.TheoNhanVien);DinhDang(kh,k.TheoKhachHang);DinhDang(sp,k.TheoSanPham);
        }
        private void InPdf()
        {
            if(data==null){MessageBox.Show("Hãy chạy thống kê lại sau khi đổi khoảng ngày");return;}
            using(SaveFileDialog s=new SaveFileDialog{Filter="PDF (*.pdf)|*.pdf",FileName="ThongKe_"+DateTime.Today.ToString("yyyyMMdd")+".pdf"}) {
                if(s.ShowDialog()!=DialogResult.OK)return;
                try {BaoCaoPdf.Xuat(data,s.FileName);MessageBox.Show("Đã tạo báo cáo PDF: "+s.FileName);}
                catch(Exception ex){Tv4FormHelper.Loi(ex);}
            }
        }
    }
}
