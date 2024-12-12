/**********722871**********
 * Date: 12/2/2024
 * Programmer: Kyle Gilbert
 * Program Name: AirTrajectoryBuilder
 * Program Description: Sweeps possible launch angles and speeds in a given scene
 *                      and applies aerodynamic physics.
 **************************/

using AirTrajectoryBuilder.ObjectModels;
using System;
using System.Windows.Forms;

namespace AirTrajectoryBuilder;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.SetHighDpiMode(HighDpiMode.SystemAware);

        Scene? scene;
        if (args.Length > 0)
        {
            try
            {
                scene = Scene.Read(args[0]);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening scene file: {ex.GetType().Name}");
                scene = null;
            }
        }
        else scene = null;

        Application.Run(new MainForm(scene));
    }
}