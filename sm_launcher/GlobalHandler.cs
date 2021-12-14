using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sm_launcher
{
    internal static partial class GlobalHandler
    {
        //Helpers
        public static int width;
        public static int height;
        public static int left;
        public static int top;
        public static int right;
        public static int bottom;
        //Size with padding
        public static int icon_sizew;
        public static int icon_sizeh;
        //Size with margin
        public static int icon_margw;
        public static int icon_margh;
        public static int icon_textx;
        //Show icons, text
        public static bool icon_enable;
        public static bool icon_name;

        //Error message on load
        public static int load_lvl;

        public static void LoadIcons(Launcher laun)
        {
            load_lvl = 0;
            //The icon cache must be consistent with the ini file, otherwise errors will occour
            List<int> img_addrs = new List<int>();
            int img_count;
            using (StreamReader sr = new StreamReader(CFG_FILE, DEF_ENC))
            {
                LoadData(sr);
                icon_enable = (dock_nam & NM_ICON) != 0;
                icon_name = (dock_nam & NM_NAME) != 0;
                load_lvl = 1;
                icon_sizew = icon_width + 2 * icon_pad;
                icon_sizeh = icon_size + 2 * icon_pad;
                icon_margw = icon_sizew + 2 * icon_margin;
                icon_margh = icon_sizeh + 2 * icon_margin;
                icon_textx = icon_enable ? icon_pad + icon_size + icon_pad : icon_pad;
                if (el_max < 2) el_max = 10000;
                img_count = 0;
                if (icon_enable)
                {
                    if (!File.Exists(CACHE_FILE))
                    {
                        ErrorMsg(CACHE_FILE +
                            " file not found!\nGenerate an icon cache before running!");
                        Environment.Exit(0);
                    }
                    img_count = LoadWithIcons(laun, img_addrs, sr);
                    if (img_count == 0)
                    {
                        ErrorMsg("No icons are added to the dock. Exiting.");
                        Environment.Exit(0);
                    }
                }
                else
                    LoadWithoutIcons(laun, sr);
            }
        }

        private static int LoadWithIcons(Launcher laun, List<int> img_addrs, StreamReader sr)
        {
            int img_count;
            using (BinaryReader br = new BinaryReader(new FileStream(CACHE_FILE, FileMode.Open, FileAccess.Read), DEF_ENC))
            {
                //Reading file pointers
                img_count = br.ReadInt32();
                for (int i = 0; i < img_count; i++)
                {
                    img_addrs.Add(br.ReadInt32());
                }
                int img_start = img_addrs[0];
                //Dummy address to skip the file length check
                img_count--;
                //Reading PNG images
                for (int i = 0; i < img_count; i++)
                {
                    int img_len = img_addrs[i + 1] - img_start;
                    img_start += img_len;
                    byte[] img_data = br.ReadBytes(img_len);
                    Bitmap img = new Bitmap(new MemoryStream(img_data));
                    string[] icon_data = sr.ReadLine().Split(CFG_DELIM);
                    CreateAndAddIcon(laun, icon_data, img, i);
                }
            }
            return img_count;
        }

        private static void LoadWithoutIcons(Launcher laun, StreamReader sr)
        {
            for(int i = 0; sr.Peek() > -1; i++)
            {
                string[] icon_data = sr.ReadLine().Split(CFG_DELIM);
                //If invalid, skip
                if (icon_data.Length < ICON_DATA_LEN) continue;
                CreateAndAddIcon(laun, icon_data, null, i);
            }
        }

        private static void CreateAndAddIcon(Launcher laun, string[] icon_data, Bitmap img, int i)
        {
            int x, y;
            IconControl icon = new IconControl(
                icon_data[ICON_TEXT],
                icon_data[ICON_FILENAME],
                icon_data[ICON_STARTARG],
                icon_data[ICON_WORKDIR],
                win_style[ParseInt(icon_data[ICON_WINDOW])],
                img);
            if (el_hor)
            {
                x = i % el_max;
                y = i / el_max;
            }
            else
            {
                x = i / el_max;
                y = i % el_max;
            }
            icon.SetBounds(
                    icon_margin + x * icon_margw,
                    icon_margin + y * icon_margh,
                    icon_sizew,
                    icon_sizeh);
            laun.Controls.Add(icon);
        }
    }
}