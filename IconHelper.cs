using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ProxmoxVEGui
{
    public static class IconHelper
    {
        public static void ApplyIcon(Form form)
        {
            try
            {
                string[] possibleIconPaths =
                {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "img", "logo.ico"),
                    Path.Combine(Application.StartupPath, "assets", "img", "logo.ico"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.ico")
                };

                foreach (string iconPath in possibleIconPaths)
                {
                    if (File.Exists(iconPath))
                    {
                        form.Icon = new Icon(iconPath);
                        return;
                    }
                }
            }
            catch
            {
                // Icon is optional. Let the application load even if it fails to apply the icon.
            }
        }
    }
}
