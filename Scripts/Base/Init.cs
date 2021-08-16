using Godot;
using System;
using System.IO;

namespace Base
{

    /// <summary>
    /// UNUSED: 
    /// Vale, esto es para arrancar el juego según los settings y un config.ini Obviamente, esto deberemos cambiarlo
    /// según necesidades. Pero no lo borramos, porque lo usaremos
    /// </summary>
    public class Init : Node
    {
        private const string init = "config2.ini";
        private byte fullscreen = 255;
        private float multplier = -1;
        private byte volume = 255;

        public override void _EnterTree()
        {
            this.Loadconfig();
        }

        private void Loadconfig()
        {
            StreamReader file;
            try
            {
                file = new StreamReader(System.IO.Directory.GetCurrentDirectory() + "\\" + init);
            }
            catch
            {
                GD.PrintErr("File not found, creating a new one");
                CreateFile f = new CreateFile(0, 2, 10, System.IO.Directory.GetCurrentDirectory() + "\\" + init);
                file = new StreamReader(System.IO.Directory.GetCurrentDirectory() + "\\" + init);
            }

            string line;
            bool error = false;

            while ((line = file.ReadLine()) != null)
            {
                GD.Print("Line: ", line);
                if (line.Contains("full"))
                {
                    line = line.Remove(0, "fullscreen=".Length);

                    if (byte.TryParse(line, out fullscreen) == false)
                    {
                        fullscreen = 255;
                        error = true;
                    }
                    GD.Print("fullscreen:", fullscreen);
                }
                else if (line.Contains("mult"))
                {
                    line = line.Remove(0, "multiplier=".Length);

                    if (float.TryParse(line, out multplier) == false)
                    {
                        multplier = -1;
                        error = true;
                    }
                    if(multplier < 1 || multplier > 7){
                        multplier = 2;
                        error = true;
                    }
                    GD.Print("mult: ", multplier);
                }
                else if (line.Contains("volume"))
                {
                    line = line.Remove(0, "volume=".Length);
                    if (byte.TryParse(line, out volume) == false)
                    {
                        volume = 255;
                        error = true;
                    }
                    GD.Print("volume: ", volume);
                }
                else
                {
                    GD.Print("nooooo");
                    error = true;
                }
            }
            file.Close();

            this.Check(error);
        }

        private void Check(bool error)
        {
            if (fullscreen == 0)
            {
                OS.WindowFullscreen = false;

                if (multplier != -1)
                {
                    OS.WindowSize *= multplier;
                }
                ExtensionMethods.CenterWindow();
            }
            else if (fullscreen == 1)
            {
                OS.WindowFullscreen = true;
            }
            else
            {
                error = true;
            }

            if (volume != 255)
            {
                AudioServer.SetBusVolumeDb(0, (volume - 10) * 7.2f);
            }

            if (error)
            {
                GD.PrintErr("Found an error on file, creating a new one");
                GD.Print("F:", fullscreen, "\nM: ", multplier, "\nv:", volume);
                CreateFile f = new CreateFile(0, 2, 10, System.IO.Directory.GetCurrentDirectory() + "\\" + init);
                this.Loadconfig();
            }
        }
    }

    public struct CreateFile
    {

        public CreateFile(byte fullscreen, in float mult, byte volume, in string dir)
        {
            StreamWriter w = new StreamWriter(dir);
            if (fullscreen > 1)
            {
                fullscreen = 0;
            }
            w.WriteLine("fullscreen=" + fullscreen.ToString());

            w.WriteLine("multiplier=2");

            if (volume > 10)
            {
                volume = 10;
            }
            w.WriteLine("volume=" + volume);

            w.Close();
        }
    }    
}
