using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using oda;
namespace WindowsFormsApplication1
{
    class SelectionControl : OdaControl
    {
        SelectionDataSource DataSet;
        public SelectionDataSource GetDataSet()
        {
            DataSet = new SelectionDataSource(Class);
            return DataSet;
        }
    }
}
