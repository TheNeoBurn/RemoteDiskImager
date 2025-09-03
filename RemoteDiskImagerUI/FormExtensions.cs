using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace RemoteDiskImagerUI;

public static class FormsHelper {
    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    public static bool IsWindowsDarkMode() {
        try {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            if (key != null) {
                object? value = key.GetValue("AppsUseLightTheme");
                if (value is int intValue)
                    return intValue == 0;
            }
        } catch { }
        return false;
    }

    public static void ApplyThemeColors(Control control) {
        control.BackColor = Color.FromArgb(32, 32, 32);
        control.ForeColor = Color.White;
        foreach (Control child in control.Controls) {
            ApplyThemeColors(child);
        }
    }

    public static void TryEnableDarkMode(IntPtr handle) {
        if (Environment.OSVersion.Version.Major >= 10) {
            int attribute = 20; // DWMWA_USE_IMMERSIVE_DARK_MODE
            int useDark = 1;
            DwmSetWindowAttribute(handle, attribute, ref useDark, sizeof(int));
        }
    }


}
