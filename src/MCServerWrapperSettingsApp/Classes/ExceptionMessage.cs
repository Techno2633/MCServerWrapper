using System;
using System.Windows.Forms;

namespace MCServerWrapperSettingsApp.Classes
{
    public static class ExceptionMessage
    {
        public static void PrintException(Exception ex)
        {
            string msg = $"[Source]: {ex.Source}\n[Target Site]: {ex.TargetSite}\n[Message]: {ex.Message}";
            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void PrintException(Exception ex, string message)
        {
            string msg = $"[Message]: {message}\n[Source]: {ex.Source}\n[Target Site]: {ex.TargetSite}\n[Error Message]: {ex.Message}";
            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
