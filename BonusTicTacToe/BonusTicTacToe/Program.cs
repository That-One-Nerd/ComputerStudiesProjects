/**********722871**********
 * Date: 9/24/2024
 * Programmer: Kyle Gilbert
 * Program Name: BonusTicTacToe
 * Program Description: Customizable tic-tac-toe with some nice colors.
 **************************/

using BonusTicTacToe.Resources;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace BonusTicTacToe;

public static class Program
{
    internal static PrivateFontCollection fonts = new();

    [STAThread]
    public static void Main()
    {
        Application.SetCompatibleTextRenderingDefault(false);
        Application.SetHighDpiMode(HighDpiMode.SystemAware);

        // Load fonts into our collection via an unsafe marshal copy.
        // Somewhat unsafe, but that's where the fun happens anyway.
        // Maybe variable fonts could have worked?
        IEnumerable<byte[]> fontsToLoad = [
            Fonts.Outfit_SemiBold, // 600
        ];
        foreach (byte[] toLoad in fontsToLoad)
        {
            int length = toLoad.Length;
            nint ptr = Marshal.AllocCoTaskMem(length);
            Marshal.Copy(toLoad, 0, ptr, length);
            fonts.AddMemoryFont(ptr, length);
        }

        Application.Run(new MainForm());
    }
}
