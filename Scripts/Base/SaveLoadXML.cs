using Godot;
using System;
//using DialogComp;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace Base
{


    public class SaveLoadXML : Node
    {

        private static string PATH_DEF = @"c:\Cosas\";
        private static string FILE_DEF = "Test.xml";

        
/*

        #region load_save XML
        //Obviamente esto irá en una clase a parte, tanto el load como el save funcan
        public static void SaveDialogXML(in FullDialog dialogs)
        {

            //FullDialog all = new FullDialog(dialogs);

            XmlSerializer xs = new XmlSerializer(typeof(FullDialog));
            TextWriter tw = new StreamWriter(String.Concat(PATH_DEF, FILE_DEF));
            xs.Serialize(tw, dialogs);
            tw.Close();
        }

        public static FullDialog LoadDialogXml()
        {
            XmlSerializer xs = new XmlSerializer(typeof(FullDialog));
            TextReader tr = new StreamReader(String.Concat(PATH_DEF, FILE_DEF));
            FullDialog all = (FullDialog)xs.Deserialize(tr);            
            tr.Close();
            return all;
        }
        #endregion*/

    }
}
