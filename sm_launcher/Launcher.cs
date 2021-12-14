using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sm_launcher
{
    [System.ComponentModel.DesignerCategory("Code")]
    internal sealed class Launcher : Form
    {
        //Load error levels
        private static string[] err_lvl =
        {
            "Please reconfigure the Launcher!",
            "Please generate a new icon cache!"
        };

        public Launcher()
        {
            try
            {
                SuspendLayout();
                GlobalHandler.LoadIcons(this);
                SetUpLauncher();
                ResumeLayout();
            }
            catch (Exception ex)
            {
                GlobalHandler.ErrorMsg("Error while initializing:\n" + ex.Message +
                    "\n" + err_lvl[GlobalHandler.load_lvl]);
                Environment.Exit(0);
            }
        }

        private void SetUpLauncher()
        {
            Text = GlobalHandler.dock_text;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            AutoScroll = true;
            MaximizeBox = false;
            ShowIcon = false;
            StartPosition = FormStartPosition.Manual;
            Screen curr_sc = Screen.FromControl(this);
            int x = 0, y = 0;
            int width = GlobalHandler.win_width;
            int height = GlobalHandler.win_height;
            if (GlobalHandler.dock_pos)
            {
                x = curr_sc.Bounds.Width / 2 - width / 2;
                y = curr_sc.Bounds.Height / 2 - height / 2;
            }
            x += GlobalHandler.dock_x;
            y += GlobalHandler.dock_y;
            SetBounds(x, y, width, height);
            BackColor = GlobalHandler.icon_col[GlobalHandler.IC_STATE_NORMAL].Color;
            //Helpers
            GlobalHandler.width = Width;
            GlobalHandler.height = Height;
            GlobalHandler.left = Left;
            GlobalHandler.top = Top;
            GlobalHandler.right = Right;
            GlobalHandler.bottom = Bottom;
        }
    }
}