/**********722871**********
 * Date: 2/25/2025
 * Programmer: Kyle Gilbert
 * Program Name: InverseKinematics
 * Program Description: Calculates a series of joints with inverse kinematics.
 **************************/

namespace InverseKinematics;

internal static class Program
{
    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.Run(new Form1());
    }
}
