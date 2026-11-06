using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
namespace QuanLyBanDienThoai.GUI
{
    internal static class Tv4FormHelper
    {
        public static void KhoiTao(Form f,string title)
        {
            f.Text=title;f.BackColor=Color.White;f.Font=new Font("Segoe UI",9.5f);
            f.Size=new Size(1090,730);f.MinimumSize=new Size(820,560);
        }
        public static DataGridView Luoi()
        {
            DataGridView d=new DataGridView{Dock=DockStyle.Fill,AutoGenerateColumns=true};
            GridHelper.CauHinh(d);return d;
        }
        public static Button Nut(string text,EventHandler click,int width=100)
        {
            Button b=new Button{Text=text,Width=width,Height=32,Margin=new Padding(4)};
            b.Click+=click;return b;
        }
        public static void ThongBao(string err,Action thanhCong)
        {
            if(err!="")MessageBox.Show(err,"Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            else {thanhCong();MessageBox.Show("Đã lưu thành công","Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Information);}
        }
        public static Panel Field(string label,Control control,int width=200)
        {
            Panel p=new Panel{Width=width,Height=58,Margin=new Padding(5)};
            p.Controls.Add(new Label{Text=label,Dock=DockStyle.Top,Height=22});
            control.Dock=DockStyle.Bottom;control.Height=27;p.Controls.Add(control);
            return p;
        }
        public static void Hien(DataGridView grid,ArrayList ds,Type type)
        {
            GridHelper.GanDuLieu(grid,ds,type);
            GridHelper.BoChon(grid);
        }
        public static string TimNangCao(Type type,Func<System.Collections.Generic.List<QuanLyBanDienThoai.BUS.DieuKien>,ArrayList> loc,DataGridView grid)
        {
            try {
                using(FormTimNangCao f=new FormTimNangCao(type))
                    if(f.ShowDialog()==DialogResult.OK)Hien(grid,loc(f.KetQua),type);
                return "";
            }catch(Exception ex){return ex.Message;}
        }
        public static void Loi(Exception ex)=>MessageBox.Show(ex.Message,"Lỗi",MessageBoxButtons.OK,MessageBoxIcon.Error);
    }
}
