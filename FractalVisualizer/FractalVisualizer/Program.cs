/**********722871**********
 * Date: 11/13/2024
 * Programmer: Kyle Gilbert
 * Program Name: Fractal Visualizer
 * Program Description: A visualizer for complex-based iterative fractals.
 **************************/

using FractalVisualizer.Forms;

namespace FractalVisualizer;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.SetHighDpiMode(HighDpiMode.SystemAware);

        Application.Run(new MainForm(MandlebrotSetup, MandlebrotIter)
        {
            MaxIterations = 512,
            CutoffMagnitude = 4,
        });
    }

    private static Complex start;
    private static void MandlebrotSetup(Complex num) => start = num;
    private static Complex MandlebrotIter(Complex num) => num * num + start;
}