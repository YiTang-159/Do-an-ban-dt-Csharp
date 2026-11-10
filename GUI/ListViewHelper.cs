using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;
namespace QuanLyBanDienThoai.GUI
{
    public static class ListViewHelper
    {
        public static ListView Tao() => new ListView {
            Dock=DockStyle.Fill, View=View.Details, FullRowSelect=true,
            GridLines=true, MultiSelect=false, HideSelection=false
        };
        public static T Chon<T>(ListView view) where T:class =>
            view.SelectedItems.Count==0 ? null : view.SelectedItems[0].Tag as T;
        private static string Hien(object value)
        {
            if(value==null || value==DBNull.Value)return "";
            if(value is DateTime day)return day.ToString("dd/MM/yyyy");
            if(value is decimal money)return money.ToString("#,##0");
            return Convert.ToString(value);
        }
        // Column definitions: property name | visible caption.
        public static void Gan(ListView view,IEnumerable data,params string[] columns)
        {
            view.BeginUpdate();
            try {
                view.Items.Clear(); view.Columns.Clear();
                foreach(string column in columns) {
                    var parts=column.Split('|');view.Columns.Add(parts.Length>1?parts[1]:parts[0],150);
                }
                foreach(object item in data) {
                    var values=new string[columns.Length];
                    for(int i=0;i<columns.Length;i++) {
                        string key=columns[i].Split('|')[0];
                        object value=item is DataRow row?row[key]:item.GetType().GetProperty(key)?.GetValue(item);
                        values[i]=Hien(value);
                    }
                    view.Items.Add(new ListViewItem(values){Tag=item});
                }
            } finally {view.EndUpdate();}
        }
    }
}
