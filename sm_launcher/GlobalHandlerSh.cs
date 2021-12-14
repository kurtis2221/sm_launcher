using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sm_launcher
{
    internal static partial class GlobalHandler
    {
        //Delimiter
        const char CFG_DELIM = (char)1;

        //First line indexes
        //0 - Normal bgcolor
        //1 - Hover bgcolor
        //2 - Click bgcolor
        //3 - Text normal color
        //4 - Text hover color
        //5 - Text click color
        //6 - Font name
        //7 - Font size
        //8 - Font style
        //9 - Icon size
        //10 - Padding
        //11 - Smoothing quality
        //12 - Position
        //13 - Position X
        //14 - Position Y
        //15 - Show names
        //16 - Horizontal
        //17 - Maximum elements
        //18 - Close after launch
        //19 - Launcher margin
        //20 - Icon object width
        //21 - Launcher text
        //22 - Launcher width
        //22 - Launcher height
        const int SET_DATA_LEN = 24;
        const int SET_N_BGCOLOR = 0;
        const int SET_H_BGCOLOR = 1;
        const int SET_C_BGCOLOR = 2;
        const int SET_TXT_N_COLOR = 3;
        const int SET_TXT_H_COLOR = 4;
        const int SET_TXT_C_COLOR = 5;
        const int SET_TXT_FONT = 6;
        const int SET_TXT_FONT_SZ = 7;
        const int SET_TXT_FONT_ST = 8;
        const int SET_ICONSIZE = 9;
        const int SET_PADDING = 10;
        const int SET_SMOOTH = 11;
        const int SET_POSITION = 12;
        const int SET_POS_X = 13;
        const int SET_POS_Y = 14;
        const int SET_NAMES = 15;
        const int SET_EL_HOR = 16;
        const int SET_EL_MAX = 17;
        const int SET_CLOSE = 18;
        const int SET_MARGIN = 19;
        const int SET_WIDTH = 20;
        const int SET_TEXT = 21;
        const int SET_WIN_WIDTH = 22;
        const int SET_WIN_HEIGHT = 23;

        //Icon data indexes
        //0 - Text
        //1 - File name
        //2 - Icon - File name
        //3 - Icon - Number
        //4 - Start arguments
        //5 - Working directory
        //6 - Start window style
        const int ICON_DATA_LEN = 7;
        const int ICON_TEXT = 0;
        const int ICON_FILENAME = 1;
        const int ICON_ICONFILE = 2;
        const int ICON_ICONNUM = 3;
        const int ICON_STARTARG = 4;
        const int ICON_WORKDIR = 5;
        const int ICON_WINDOW = 6;

        //Icon color states
        public const int IC_STATES = 3;
        public const int IC_STATE_NORMAL = 0;
        public const int IC_STATE_HOVER = 1;
        public const int IC_STATE_CLICK = 2;

        //Name and icon settings
        public const int NM_ICON = 1;
        public const int NM_NAME = 2;

        //For FileStream
        const int BUFFER_SIZE = 4096;

        //Program name
        public const string PROG_NAME = "Simple Launcher";

        //Files
        public const string CFG_FILE = "sm_launcher.ini";
        public const string CACHE_FILE = "icons.img";
        public const string TMP_CACHE_FILE = "icons.img.tmp";

        //Settings
        public static int icon_size;
        public static int icon_pad;
        public static InterpolationMode icon_sm;
        public static int icon_sm_idx;
        public static bool dock_pos;
        public static int dock_x, dock_y;
        public static SolidBrush[] txt_col = new SolidBrush[IC_STATES];
        public static Font txt_fnt;
        public static SolidBrush[] icon_col = new SolidBrush[IC_STATES];
        public static int dock_nam;
        public static bool el_hor;
        public static int el_max;
        public static bool dock_close;
        public static int icon_margin;
        public static int icon_width;
        public static string dock_text;
        public static int win_width;
        public static int win_height;

        //Smoothing levels
        public static InterpolationMode[] icon_interp =
        {
            InterpolationMode.NearestNeighbor,
            InterpolationMode.Low,
            InterpolationMode.Bilinear,
            InterpolationMode.HighQualityBilinear
        };


        //Start window state
        public static ProcessWindowStyle[] win_style =
        {
            ProcessWindowStyle.Normal,
            ProcessWindowStyle.Maximized,
            ProcessWindowStyle.Minimized
        };

        //Encoding
        private static Encoding DEF_ENC = Encoding.Default;

        private static void LoadData(StreamReader sr)
        {
            string[] cols = sr.ReadLine().Split(CFG_DELIM);
            icon_col[IC_STATE_NORMAL] = new SolidBrush(Color.FromArgb(ParseInt(cols[SET_N_BGCOLOR], true)));
            icon_col[IC_STATE_HOVER] = new SolidBrush(Color.FromArgb(ParseInt(cols[SET_H_BGCOLOR], true)));
            icon_col[IC_STATE_CLICK] = new SolidBrush(Color.FromArgb(ParseInt(cols[SET_C_BGCOLOR], true)));
            txt_col[IC_STATE_NORMAL] = new SolidBrush(Color.FromArgb(ParseInt(cols[SET_TXT_N_COLOR], true)));
            txt_col[IC_STATE_HOVER] = new SolidBrush(Color.FromArgb(ParseInt(cols[SET_TXT_H_COLOR], true)));
            txt_col[IC_STATE_CLICK] = new SolidBrush(Color.FromArgb(ParseInt(cols[SET_TXT_C_COLOR], true)));
            txt_fnt = new Font(new FontFamily(cols[SET_TXT_FONT]),
                ParseFloat(cols[SET_TXT_FONT_SZ], 10),
                (FontStyle)ParseInt(cols[SET_TXT_FONT_ST]));
            icon_size = ParseInt(cols[SET_ICONSIZE]);
            icon_pad = ParseInt(cols[SET_PADDING]);
            icon_sm_idx = ParseInt(cols[SET_SMOOTH]);
            icon_sm = icon_interp[icon_sm_idx];
            dock_pos = ParseBool(cols[SET_POSITION]);
            dock_x = ParseInt(cols[SET_POS_X]);
            dock_y = ParseInt(cols[SET_POS_Y]);
            dock_nam = ParseInt(cols[SET_NAMES]);
            el_hor = ParseBool(cols[SET_EL_HOR]);
            el_max = ParseInt(cols[SET_EL_MAX]);
            dock_close = ParseBool(cols[SET_CLOSE]);
            icon_margin = ParseInt(cols[SET_MARGIN]);
            icon_width = ParseInt(cols[SET_WIDTH]);
            dock_text = cols[SET_TEXT];
            win_width = ParseInt(cols[SET_WIN_WIDTH]);
            win_height = ParseInt(cols[SET_WIN_HEIGHT]);
        }

        private static float ParseFloat(string input, float def)
        {
            float res;
            if (!float.TryParse(input, out res)) res = def;
            return res;
        }

        private static int ParseInt(string input, bool hex = false)
        {
            int res;
            if (hex) int.TryParse(input, NumberStyles.HexNumber, null, out res);
            else int.TryParse(input, out res);
            return res;
        }

        private static bool ParseBool(string input)
        {
            return input != "0";
        }

        private static string BoolToStr(bool input)
        {
            return input ? "1" : "0";
        }

        public static void ErrorMsg(string msg)
        {
            MessageBox.Show(msg, PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}