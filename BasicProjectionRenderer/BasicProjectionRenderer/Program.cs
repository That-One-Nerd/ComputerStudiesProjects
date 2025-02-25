/**********722871**********
 * Date: 2/19/2025
 * Programmer: Kyle Gilbert
 * Program Name: BasicProjectionRenderer
 * Program Description: Renders an OBJ file in orthographic view. All code
 *                      (even the matrix multiplication) is my own.
 **************************/

using System;
using System.IO;
using System.Windows.Forms;

namespace BasicProjectionRenderer;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Application.SetCompatibleTextRenderingDefault(false);
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        
        string path = "monkey.obj";
        FileStream fs = new(path, FileMode.Open);
        Mesh obj = Mesh.FromObj(fs);
        fs.Close();
        Form1 form = new()
        {
            Mesh = obj
        };
        Application.Run(form);
    }
}