using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraGrid;
using oda;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        
        private const int captionHeight = 33;
        private const int rowHeight = 20;
        DataTable table2;
        SelectionDataSource DataSet = new SelectionControl().GetDataSet();
        public Form1()
        {
            InitializeComponent();

            

            DataTable table1 = new DataTable();
            table1.Columns.Add("Id");
            table1.Columns.Add("Name");
            table1.Columns.Add("Created");
            table1.Columns.Add("IsAdult");
            table1.Columns.Add("Genged");
            table1.Rows.Add(1, "Alex", new DateTime(2018, 06, 15), true, 'M');
            table1.Rows.Add(2, "Olga", new DateTime(2018, 06, 16), false, 'F');
            table1.Rows.Add(3, "Oleg", new DateTime(2018, 06, 17), true, 'M');
            table1.Rows.Add(4, "Maria", new DateTime(2018, 06, 18), false, 'F');
            System.Data.DataSet set = new System.Data.DataSet();
            set.Tables.Add(table1);
            
            gridControl1.DataSource = set.Tables[0];
            

            gridControl1.DoubleClick += new EventHandler(gridControl1_DoubleClick);

            table2 = new DataTable();
            table2.Columns.Add("Id");
            table2.Columns.Add("Name");
            table2.Columns.Add("Created");
            table2.Columns.Add("IsAdult");
            table2.Columns.Add("Genger");

            gridControl2.DataSource = DataSet;
        }

        private void gridControl1_DoubleClick(object sender, EventArgs e)
        {
            GridControl gc = sender as GridControl;
            int rowsCount = ((DataTable)gc.DataSource).Rows[0].ItemArray.Length;
            int[] intervalArr = GetIntervalArr(rowsCount);
            int rowNum = GetRowNum(intervalArr, e);

            object[] item = ((DataTable)gc.DataSource).Rows[rowNum].ItemArray;
            
            //PopulateSlaveGridControl(item);


            DataSet.AddObject(item, true);
        }

        private void PopulateSlaveGridControl(object[] item)
        {
            table2.Rows.Add(item[0], item[1], item[2], item[3], item[4]);

            System.Data.DataSet set2 = new System.Data.DataSet();
            set2.Tables.Add(table2);
            gridControl2.DataSource = set2.Tables[0];
            set2.Tables.Remove(table2);
        }

        private int GetRowNum(int[] intervalArr, EventArgs e) 
        {
            var y = (e as DXMouseEventArgs).Y;
            for (int i = 0; i < intervalArr.Length; i++)
            {
                if (y > intervalArr[i] && y <= intervalArr[i + 1])
                    return i;
            }
            return 0;
        }

        private int[] GetIntervalArr(int rowsCount)
        {
            int[] arr = new int[rowsCount + 1];
            arr[0] = captionHeight;
            for (int i = 1; i < arr.Length; i++)
            {
                arr[i] = captionHeight + rowHeight * (i + 1);
            }
            return arr;
        }
    }

     class SelectionDataSource : System.ComponentModel.BindingList<oda.xmlElement>, System.ComponentModel.ITypedList
        {

            #region ITypedList Members
            PropertyDescriptorCollection list;
            public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
            {
                if (list != null)
                    return list;
                list = new PropertyDescriptorCollection(listAccessors);
                list.Add(new oda.StringFieldDescriptor("name"));
                list.Add(new oda.NumericFieldDescriptor("count"));
                return list;
            }

            public string GetListName(PropertyDescriptor[] listAccessors)
            {
                return "";
            }

            #endregion
            oda.Class Class;
            oda.xmlDocument Doc;

            internal SelectionDataSource(oda.Class cls)
            {
                Class = cls;
                ObjectList = new oda.ObjectList(Class);
                Doc = new oda.xmlDocument();
                Doc.LoadXML("<X/>");
            }
            internal oda.ObjectList ObjectList;
            internal void ClearDataSet()
            {
                try  
                {
                    Clear();
                    ObjectList.Clear();
                    Doc.Root.RemoveNodes("*");
                }
                catch { }
            }

            internal void AddObject(object[] obj, bool show_count)
            {
                decimal cnt = 1;
                if (show_count)
                    cnt = oda.Dialog.InputNumBox("Введите количество", cnt);
                if (cnt == 0)
                    return;
                oda.xmlElement el = Doc.Root.SelectElement("R[@oid='" + obj[0] + "']");
                if (el == null)
                {
                    el = Doc.Root.CreateChildElement("R");
                    el.SetAttribute("oid", obj[0]);
                    el.SetAttribute("name", obj[1]);
                    el.SetAttribute("count", cnt);
                    Add(el);
                }
                else
                {
                    el.SetAttribute("count", el.GetInt("count") + cnt);
                    ResetItem(IndexOf(el));
                }
            }

            internal void RemoveDataItem(int p)
            {
                try
                {
                    oda.xmlElement el = this.Items[p];
                    string oid = el.GetAttribute("oid");
                    // int i = ObjectList.IndexOf(oid);
                    ObjectList.Remove(oid);
                    el.Remove();
                    RemoveAt(p);
                }
                catch { }
            }
        }
}