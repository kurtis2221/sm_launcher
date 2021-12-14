using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using sm_launcher;

namespace sm_launcher_cfg
{
    public partial class Form1 : Form
    {
        private bool unsaved_icons;
        private bool list_move;

        public Form1()
        {
            InitializeComponent();
            Text = GlobalHandler.PROG_NAME + " configuration";
            cb_smooth.SelectedIndex = 0;
            cb_ic_win.SelectedIndex = 0;
            unsaved_icons = false;
            GlobalHandler.txt_fnt = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular);
            try
            {
                if (File.Exists(GlobalHandler.CFG_FILE))
                {
                    GlobalHandler.LoadConfig();
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                GlobalHandler.ErrorMsg("Error while loading: " + GlobalHandler.CFG_FILE + "\n" + ex.Message);
            }
        }

        private void bt_colordial_Click(object sender, EventArgs e)
        {
            if (dl_color.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            if (sender == bt_bg_n)
            {
                lb_bg_n.BackColor = dl_color.Color;
            }
            else if (sender == bt_bg_h)
            {
                lb_bg_h.BackColor = dl_color.Color;
            }
            else if (sender == bt_bg_c)
            {
                lb_bg_c.BackColor = dl_color.Color;
            }
            else if (sender == bt_txt_n)
            {
                lb_txt_n.BackColor = dl_color.Color;
            }
            else if (sender == bt_txt_h)
            {
                lb_txt_h.BackColor = dl_color.Color;
            }
            else //if(sender == bt_txt_c)
            {
                lb_txt_c.BackColor = dl_color.Color;
            }
        }

        private void bt_fname_Click(object sender, EventArgs e)
        {
            if (dl_fname.ShowDialog() == DialogResult.OK)
            {
                string fname = dl_fname.FileName;
                tb_ic_fname.Text = fname;
                if (tb_ic_icon.Text.Length == 0) tb_ic_icon.Text = fname;
                if (tb_ic_work.Text.Length == 0) tb_ic_work.Text = Path.GetDirectoryName(fname);
            }
        }

        private void bt_icon_Click(object sender, EventArgs e)
        {
            if (dl_icon.ShowDialog() == DialogResult.OK)
            {
                tb_ic_icon.Text = dl_icon.FileName;
            }
        }

        private void bt_font_Click(object sender, EventArgs e)
        {
            dl_font.Font = GlobalHandler.txt_fnt;
            if (dl_font.ShowDialog() == DialogResult.OK)
            {
                GlobalHandler.txt_fnt = dl_font.Font;
            }
        }

        private void bt_cache_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Config file was saved before? Generate cache?",
                GlobalHandler.PROG_NAME, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    GlobalHandler.CreateCache();
                    MessageBox.Show("Cache generated successfully.",
                        GlobalHandler.PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    GlobalHandler.ErrorMsg("Failed to generate cache: " + ex.Message);
                }
            }
        }

        private void bt_save_Click(object sender, EventArgs e)
        {
            if (!HandleUnsavedIcon()) return;
            if (li_icons.Items.Count == 0)
            {
                GlobalHandler.ErrorMsg("Please add an element to the list before saving!");
                return;
            }
            if (MessageBox.Show("Save config?",
                GlobalHandler.PROG_NAME, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    SaveData();
                    GlobalHandler.SaveConfig();
                    MessageBox.Show("Config file saved successfully.",
                        GlobalHandler.PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    GlobalHandler.ErrorMsg("Failed to save config: " + ex.Message);
                }
            }
        }

        private void li_icons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (list_move) return;
            if (!HandleUnsavedIcon()) return;
            list_move = true;
            int idx = li_icons.SelectedIndex;
            if (idx == -1)
            {
                list_move = false;
                return;
            }
            IconData ic = (IconData)li_icons.Items[idx];
            tb_ic_text.Text = ic.text;
            tb_ic_fname.Text = ic.filename;
            tb_ic_icon.Text = ic.icon;
            nm_ic_icnm.Value = ic.icon_nm;
            tb_ic_args.Text = ic.startarg;
            tb_ic_work.Text = ic.workdir;
            cb_ic_win.SelectedIndex = ic.window;
            list_move = false;
        }

        private void bt_list_Click(object sender, EventArgs e)
        {
            list_move = true;
            if (sender == bt_new)
            {
                if (!HandleUnsavedIcon()) return;
                ClearIconData();
            }
            else if (sender == bt_add)
            {
                IconData ic = new IconData();
                SetIconData(ic);
                li_icons.Items.Add(ic);
                ClearIconData();
                unsaved_icons = false;
            }
            else if (sender == bt_upd)
            {
                int idx = li_icons.SelectedIndex;
                if (idx == -1) return;
                IconData ic = (IconData)li_icons.Items[idx];
                SetIconData(ic);
                li_icons.Items[idx] = ic;
                unsaved_icons = false;
            }
            else //if(sender == bt_del)
            {
                if (MessageBox.Show("Are you sure you want to delete this item?",
                    GlobalHandler.PROG_NAME, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    return;
                }
                int idx = li_icons.SelectedIndex;
                if (idx == -1) return;
                li_icons.Items.RemoveAt(idx);
            }
            list_move = false;
        }

        private void bt_updw_Click(object sender, EventArgs e)
        {
            list_move = true;
            int idx = li_icons.SelectedIndex;
            if (sender == bt_up)
            {
                if (idx <= 0) return;
                IconData tmp = (IconData)li_icons.Items[idx];
                li_icons.Items[idx] = li_icons.Items[idx - 1];
                li_icons.Items[idx - 1] = tmp;
                li_icons.SelectedIndex -= 1;
            }
            else //if(sender == bt_dw)
            {
                if (idx >= li_icons.Items.Count - 1) return;
                IconData tmp = (IconData)li_icons.Items[idx];
                li_icons.Items[idx] = li_icons.Items[idx + 1];
                li_icons.Items[idx + 1] = tmp;
                li_icons.SelectedIndex += 1;
            }
            list_move = false;
        }

        private void tb_ic_TextChanged(object sender, EventArgs e)
        {
            if (list_move) return;
            unsaved_icons = true;
        }

        private void cb_ic_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (list_move) return;
            unsaved_icons = true;
        }

        private void nm_ic_ValueChanged(object sender, EventArgs e)
        {
            if (list_move) return;
            unsaved_icons = true;
        }

        private void ch_names_CheckedChanged(object sender, EventArgs e)
        {
            if (!ch_names.Checked)
            {
                ch_icon.Enabled = false;
                ch_icon.Checked = true;
            }
            else ch_icon.Enabled = true;
        }

        private bool HandleUnsavedIcon()
        {
            if (unsaved_icons)
            {
                if (MessageBox.Show("Discard icon changes?",
                    GlobalHandler.PROG_NAME, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return false;
                }
                unsaved_icons = false;
            }
            return true;
        }

        private bool CheckIconData()
        {
            string missing = null;
            if (tb_ic_text.Text.Length == 0) missing = "Name";
            if (tb_ic_fname.Text.Length == 0) missing = "Filename";
            if (tb_ic_icon.Text.Length == 0) missing = "Icon";
            if (missing != null)
            {
                GlobalHandler.ErrorMsg("Please fill " + missing + " before saving!");
                return false;
            }
            return true;
        }

        private void SetIconData(IconData ic)
        {
            ic.text = tb_ic_text.Text;
            ic.filename = tb_ic_fname.Text;
            ic.icon = tb_ic_icon.Text;
            ic.icon_nm = (int)nm_ic_icnm.Value;
            ic.startarg = tb_ic_args.Text;
            ic.workdir = tb_ic_work.Text;
            ic.window = cb_ic_win.SelectedIndex;
        }

        private void ClearIconData()
        {
            tb_ic_text.Text = string.Empty;
            tb_ic_fname.Text = string.Empty;
            tb_ic_icon.Text = string.Empty;
            nm_ic_icnm.Value = 0;
            tb_ic_args.Text = string.Empty;
            tb_ic_work.Text = string.Empty;
            cb_ic_win.SelectedIndex = 0;
        }

        private void LoadData()
        {
            try
            {
                lb_bg_n.BackColor = GlobalHandler.icon_col[GlobalHandler.IC_STATE_NORMAL].Color;
                lb_bg_h.BackColor = GlobalHandler.icon_col[GlobalHandler.IC_STATE_HOVER].Color;
                lb_bg_c.BackColor = GlobalHandler.icon_col[GlobalHandler.IC_STATE_CLICK].Color;
                lb_txt_n.BackColor = GlobalHandler.txt_col[GlobalHandler.IC_STATE_NORMAL].Color;
                lb_txt_h.BackColor = GlobalHandler.txt_col[GlobalHandler.IC_STATE_HOVER].Color;
                lb_txt_c.BackColor = GlobalHandler.txt_col[GlobalHandler.IC_STATE_CLICK].Color;
                //Font is directly loaded
                nm_ic_size.Value = GlobalHandler.icon_size;
                nm_ic_pad.Value = GlobalHandler.icon_pad;
                cb_smooth.SelectedIndex = GlobalHandler.icon_sm_idx;
                ch_pos.Checked = GlobalHandler.dock_pos;
                nm_x.Value = GlobalHandler.dock_x;
                nm_y.Value = GlobalHandler.dock_y;
                ch_icon.Checked = (GlobalHandler.dock_nam & GlobalHandler.NM_ICON) != 0;
                ch_names.Checked = (GlobalHandler.dock_nam & GlobalHandler.NM_NAME) != 0;
                ch_el_hor.Checked = GlobalHandler.el_hor;
                nm_el_max.Value = GlobalHandler.el_max;
                ch_close.Checked = GlobalHandler.dock_close;
                nm_ic_margin.Value = GlobalHandler.icon_margin;
                nm_ic_width.Value = GlobalHandler.icon_width;
                tb_text.Text = GlobalHandler.dock_text;
                nm_win_width.Value = GlobalHandler.win_width;
                nm_win_height.Value = GlobalHandler.win_height;
            }
            catch (Exception ex)
            {
                GlobalHandler.ErrorMsg("Error while loading: " + GlobalHandler.CFG_FILE + "\n" + ex.Message);
            }
            li_icons.Items.Clear();
            foreach (IconData ic in GlobalHandler.icon_list)
            {
                li_icons.Items.Add(ic);
            }
        }

        private void SaveData()
        {
            GlobalHandler.icon_col[GlobalHandler.IC_STATE_NORMAL] = new SolidBrush(lb_bg_n.BackColor);
            GlobalHandler.icon_col[GlobalHandler.IC_STATE_HOVER] = new SolidBrush(lb_bg_h.BackColor);
            GlobalHandler.icon_col[GlobalHandler.IC_STATE_CLICK] = new SolidBrush(lb_bg_c.BackColor);
            GlobalHandler.txt_col[GlobalHandler.IC_STATE_NORMAL] = new SolidBrush(lb_txt_n.BackColor);
            GlobalHandler.txt_col[GlobalHandler.IC_STATE_HOVER] = new SolidBrush(lb_txt_h.BackColor);
            GlobalHandler.txt_col[GlobalHandler.IC_STATE_CLICK] = new SolidBrush(lb_txt_c.BackColor);
            //Font is directly saved
            GlobalHandler.icon_size = (int)nm_ic_size.Value;
            GlobalHandler.icon_pad = (int)nm_ic_pad.Value;
            GlobalHandler.icon_sm_idx = cb_smooth.SelectedIndex;
            GlobalHandler.icon_sm = GlobalHandler.icon_interp[GlobalHandler.icon_sm_idx];
            GlobalHandler.dock_pos = ch_pos.Checked;
            GlobalHandler.dock_x = (int)nm_x.Value;
            GlobalHandler.dock_y = (int)nm_y.Value;
            GlobalHandler.dock_nam = (ch_icon.Checked ? GlobalHandler.NM_ICON : 0) | (ch_names.Checked ? GlobalHandler.NM_NAME : 0);
            GlobalHandler.el_hor = ch_el_hor.Checked;
            GlobalHandler.el_max = (int)nm_el_max.Value;
            GlobalHandler.dock_close = ch_close.Checked;
            GlobalHandler.icon_margin = (int)nm_ic_margin.Value;
            GlobalHandler.icon_width = (int)nm_ic_width.Value;
            GlobalHandler.dock_text = tb_text.Text;
            GlobalHandler.win_width = (int)nm_win_width.Value;
            GlobalHandler.win_height = (int)nm_win_height.Value;
            GlobalHandler.icon_list.Clear();
            foreach (IconData ic in li_icons.Items)
            {
                GlobalHandler.icon_list.Add(ic);
            }
        }
    }
}