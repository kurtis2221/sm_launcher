namespace sm_launcher
{
    internal class IconData
    {
        public string text;
        public string filename;
        public string icon;
        public int icon_nm;
        public string startarg;
        public string workdir;
        public int window;

        public override string ToString()
        {
            return text;
        }
    }
}