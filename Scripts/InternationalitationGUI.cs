using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class InternationalitationGUI : EditorWindow
{

    LocalInterface inter;
    
    private string langNum = "1";
    private bool setup = false;
    private bool scanScriptables = false;
    private string scriptablePath = "Assets";

    // Incluye una entrada en el menu superior de Unity
    [MenuItem("Custom Plugins/Internationalitaion Plugin")]
    
    
    public static void ShowWindow()
    {
        // Nombre del "Tab" en la ventna del editor
        GetWindow<InternationalitationGUI>("Internationalitaion Plugin");
    }

    // Dibuja la interfaz en la ventana del editor
    void OnGUI()
    {
        if (!setup)
        {    
            GUILayout.Label("Localization Extraction Configuration", EditorStyles.boldLabel);
   
            EditorGUILayout.Space();

            scanScriptables = GUILayout.Toggle(scanScriptables, "Scan Scriptable Objects?");
            if (scanScriptables)
                scriptablePath=GUILayout.TextField(scriptablePath, 200);
            
            EditorGUILayout.Space();
            GUILayout.Label("Amount of languages:");
            langNum = GUILayout.TextField(langNum, 25);
            
            if (GUILayout.Button("Setup"))
            {
                if (langNum == null)
                    InitializeAll(1,scanScriptables,scriptablePath);
                else
                    InitializeAll(uint.Parse(langNum),scanScriptables,scriptablePath);
                setup = true;
            }
        }

        if (setup)
        {
            GUILayout.Label("Configuration Finished!", EditorStyles.boldLabel);
   
            EditorGUILayout.Space();

            /*
            // Boton que ejecuta el script de modificacion de los strings
            if (GUILayout.Button("Modify All Strings"))
            {
                ModifyStrings();
            }

            // Boton que ejecuta el script de extraccion
            if (GUILayout.Button("Extract All Strings"))
            {
                ExtractStrings();
            }

            EditorGUILayout.Space();

            //Boton que ejecuta la escritura las cadenas de strings a un XML
            if (GUILayout.Button("Write To XML"))
            {
                WriteToXML();
            }*/
            //Boton que ejecuta la lectura de las cadenas de strings de un XML concreto
            if (GUILayout.Button("Read from XML"))
            {
                ReadFromXML();
            }

            if (GUILayout.Button("Auto Setup All UI Clampers"))
            {
                inter.SetupUIClampers();
            }

        }
    }

    void ModifyStrings()
    {
        Debug.Log("Poner el script de modificacion aqui");
    }

    void ExtractStrings()
    {
        //extract.ExtractStrings();
        inter.Extract();
    }

    void WriteToXML()
    {
        //Abre una ventana del explorador para qe
        string selectedPath = EditorUtility.SaveFilePanel(
            "Select directory to save XML",
            Application.dataPath, 
            "example.xml", //Nombre por defecto
            "xml");

        if (!string.IsNullOrEmpty(selectedPath))
        {
            //file.WriteXML(selectedPath);
            //inter.WriteToXML(selectedPath);
            inter.FullExtract(selectedPath);
            Debug.Log("File saved in: " + selectedPath);
        }
    }

    void ReadFromXML()
    {
        //Abre una ventana en la que el juador a�ada la ruta en la que quiera 
        string selectedPath = EditorUtility.OpenFilePanel(
          "Select XML File to read",
          Application.dataPath,
          "xml");

        if (!string.IsNullOrEmpty(selectedPath))
        {
            //file.ReadXML(selectedPath);
            List<string> langNames = new List<string>();
            langNames.Add("Español");
            langNames.Add("English");
            inter.ReadFromXML(selectedPath,LocalCore.Instance().GetLanguageMap(),langNames);
            Debug.Log("File load from: " + selectedPath);
        }
    }


    void InitializeAll(uint langinit, bool scan, string scrpath)
    {
        string selectedPath = EditorUtility.SaveFilePanel(
            "Select directory to save XML extraction",
            Application.dataPath,
            "example.xml", //Nombre por defecto
            "xml");


        if (!string.IsNullOrEmpty(selectedPath))
        {
            inter = LocalInterface.Instance();
            inter.Initiate(langinit, scan, scrpath);

            //Leemos primero el XML de los idiomas, antes de la extraccion
            readListLanguage();
            //Ahora si que hacenis la extracción
            inter.FullExtract(selectedPath);
        }
    }

    void readListLanguage()
    {
        //Abre una ventana en la que el juador a�ada la ruta en la que quiera 
        string selectedPath = EditorUtility.OpenFilePanel(
          "Select XML File Lenguage Configuration",
          Application.dataPath,
          "xml");
        //Debug.Log(selectedPath);

        if (!string.IsNullOrEmpty(selectedPath))
        {
            List<string> list = new List<string>();
            //file.ReadXML(selectedPath);
            Dictionary<uint, XmlNode> langList = inter.ReadListLanguage(selectedPath, list);
        }
    }

}
