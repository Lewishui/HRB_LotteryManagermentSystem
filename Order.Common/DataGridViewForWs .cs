using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Order.Common
{
    public class DataGridViewForWs : DataGridView
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);
            }
            catch
            {

                Invalidate();
            }
        }
    }
}
