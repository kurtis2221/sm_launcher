using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sm_launcher
{
    [System.ComponentModel.DesignerCategory("Code")]
    internal sealed class IconControl : Control
    {
        private string pr_name;
        private string pr_file;
        private string pr_args;
        private string pr_work;
        private ProcessWindowStyle pr_window;
        private Bitmap bg_img;
        private int state;

        public IconControl(string pr_name, string pr_file, string pr_args, string pr_work,
            ProcessWindowStyle pr_window, Bitmap img) : base()
        {
            this.pr_name = pr_name;
            this.pr_file = pr_file;
            this.pr_args = pr_args;
            this.pr_work = pr_work;
            this.pr_window = pr_window;
            bg_img = img;
            state = GlobalHandler.IC_STATE_NORMAL;
            AllowDrop = true;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            StateChange(GlobalHandler.IC_STATE_HOVER);
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            StateChange(GlobalHandler.IC_STATE_NORMAL);
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            StateChange(GlobalHandler.IC_STATE_CLICK);
            base.OnMouseDown(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            StateChange(GlobalHandler.IC_STATE_HOVER);
            StartProgram(string.Empty);
            base.OnMouseClick(e);
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            StateChange(GlobalHandler.IC_STATE_HOVER);
            e.Effect = DragDropEffects.Copy;
            base.OnDragEnter(e);
        }

        protected override void OnDragLeave(EventArgs e)
        {
            StateChange(GlobalHandler.IC_STATE_NORMAL);
            base.OnDragLeave(e);
        }

        protected override void OnDragDrop(DragEventArgs e)
        {
            StateChange(GlobalHandler.IC_STATE_CLICK);
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            string args = string.Empty;
            for (int i = 0; i < files.Length; i++)
            {
                args += " \"" + files[i] + "\"";
            }
            StartProgram(args);
            base.OnDragDrop(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            g.InterpolationMode = GlobalHandler.icon_sm;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
            if (GlobalHandler.icon_enable)
                g.DrawImage(bg_img, GlobalHandler.icon_pad, GlobalHandler.icon_pad);
            if (GlobalHandler.icon_name)
                g.DrawString(pr_name, GlobalHandler.txt_fnt, GlobalHandler.txt_col[state], GlobalHandler.icon_textx, 0);
            base.OnPaint(e);
        }

        private void StateChange(int input)
        {
            state = input;
            BackColor = GlobalHandler.icon_col[state].Color;
            Invalidate();
        }

        private void StartProgram(string args)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = pr_file;
                psi.Arguments = pr_args + args;
                psi.WindowStyle = pr_window;
                if (pr_work.Length > 0) psi.WorkingDirectory = pr_work;
                psi.UseShellExecute = true;
                Process.Start(psi);
                if (GlobalHandler.dock_close) Application.Exit();
            }
            catch
            {
            }
        }
    }
}