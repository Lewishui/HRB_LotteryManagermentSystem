using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
namespace ComboBoxEx
{
    public class ComboBoxEx : ComboBox
    {
        ListBox lst = new ListBox();
        public ComboBoxEx()
        {
            lst.SelectionMode = SelectionMode.MultiExtended;
            this.DrawMode = DrawMode.OwnerDrawFixed;//只有设置这个属性为OwnerDrawFixed才可能让重画起作用
            lst.KeyUp += new KeyEventHandler(lst_KeyUp);
            lst.MouseUp += new MouseEventHandler(lst_MouseUp);
            lst.KeyDown += new KeyEventHandler(lst_KeyDown);
        }
        #region Property
        [Description("选定项的值"), Category("Data")]
        public ListBox.SelectedObjectCollection SelectedItems
        {
            get
            {
                return lst.SelectedItems;
            }
        }
        #endregion
        #region override
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            bool Pressed = (e.Control && ((e.KeyData & Keys.A) == Keys.A));
            if (Pressed)
            {
                for (int i = 0; i < lst.Items.Count; i++)
                    lst.SetSelected(i, true);
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.DroppedDown = false;

        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.DroppedDown = false;
            lst.Focus();
        }
        protected override void OnDropDown(EventArgs e)
        {
            lst.Items.Clear();
            lst.Show();
            lst.ItemHeight = this.ItemHeight;
            lst.BorderStyle = BorderStyle.FixedSingle;
            lst.Size = new Size(this.DropDownWidth, this.ItemHeight * (this.MaxDropDownItems - 1) - (int)this.ItemHeight / 2);
            lst.Location = new Point(this.Left, this.Top + this.ItemHeight + 6);
            lst.BeginUpdate();
            for (int i = 0; i < this.Items.Count; i++)
                lst.Items.Add(this.Items[i]);
            lst.EndUpdate();

            this.Parent.Controls.Add(lst);
        }
        #endregion
        private void lst_KeyUp(object sender, KeyEventArgs e)
        {
            this.OnKeyUp(e);
        }
        private void lst_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                this.Text = "";
                for (int i = 0; i < lst.SelectedItems.Count; i++)
                {
                    if (i == 0)
                        this.Text = lst.SelectedItems[i].ToString();
                    else
                    {
                        this.Text = this.Text + "," + lst.SelectedItems[i].ToString();
                    }
                }
            }
            catch
            {
                this.Text = "";
            }
            bool isControlPressed = (Control.ModifierKeys == Keys.Control);
            bool isShiftPressed = (Control.ModifierKeys == Keys.Shift);
            if (isControlPressed || isShiftPressed)
                lst.Show();
            else
                lst.Hide();
        }
        private void lst_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Down:
                    if (lst.SelectedItems.Count != 0)
                    {
                        this.Text = lst.SelectedItem.ToString();
                    }
                    else
                        this.Text = this.Items[0].ToString();
                    break;
                case Keys.Up:
                    if (lst.SelectedItems.Count != 0)
                    {
                        this.Text = lst.SelectedItem.ToString();
                    }
                    else
                        this.Text = this.Items[0].ToString();
                    break;
            }
        }
    }
}