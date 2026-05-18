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
    private bool readLangNames = false;
    private bool readVariables = false;
    private bool clampUI = false;
    private bool procederClampUI = false;

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

            EditorGUILayout.Space();

            readLangNames = GUILayout.Toggle(readLangNames, "Read Language settings from XML file?");
            if (readLangNames)
                GUILayout.Label("After selecting path in which to save the extracted strings," +
                "a second window \nwill pop up to select the language settings file.");

            EditorGUILayout.Space();

            readVariables = GUILayout.Toggle(readVariables, "Read Variables settings from XML file?");
            if(readVariables)
                GUILayout.Label("After selecting path in which to save the extracted strings," +
                "a window \nwill pop up to select the language settings file.");

            EditorGUILayout.Space();

            clampUI = GUILayout.Toggle(clampUI, "Auto setup all UI Clampers?");

            EditorGUILayout.Space();

            if (GUILayout.Button("Setup"))
            {
                if (clampUI)
                {
                    procederClampUI = EditorUtility.DisplayDialog(
                       "¡Advertencia!",
                       "¿Seguro de que deseas configurar automaticamente todos los UI Clampers? Esto modificara los componentes de la interfaz en las escenas.",
                       "Si",
                       "No"
                    );
                }

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

            if (GUILayout.Button("Auto Setup All UI Clampers"))
            {
                procederClampUI = EditorUtility.DisplayDialog(
                   "¡Advertencia!",
                   "¿Seguro de que deseas configurar automaticamente todos los UI Clampers? Esto modificara los componentes de la interfaz en las escenas.",
                   "Si",
                   "No"
                );

                if (procederClampUI)
                {
                    inter.SetupUIClampers();
                }
            }
            EditorGUILayout.Space();
        }
    }

    void InitializeAll(uint langinit, bool scan, string scrpath)
    {
        string selectedPath = EditorUtility.SaveFilePanel(
            "Select directory to save XML extraction",
            Application.dataPath,
            "extraction.xml", //Nombre por defecto
            "xml");


        if (!string.IsNullOrEmpty(selectedPath))
        {
            inter = LocalInterface.Instance();
            inter.Initiate(langinit, scan, scrpath);

            //Leemos primero el XML de los idiomas, antes de la extraccion
            if(readLangNames)
                ReadListLanguage();

            //Leemos las variables a sustituir en los textos si el usuario quiere
            if(readVariables)
                ReadXMLVariables();

            //Ahora si que hacemos la extracción
            inter.FullExtract(selectedPath);

            if(clampUI && procederClampUI)
                inter.SetupUIClampers();
        }
    }

    void ReadXMLVariables()
    {
        //Abre una ventana en la que el juador a�ada la ruta en la que quiera 
        string selectedPath = EditorUtility.OpenFilePanel(
          "Select XML File with variables",
          Application.dataPath,
          "xml");

        if (!string.IsNullOrEmpty(selectedPath))
        {
            inter.ReadListVariables(selectedPath);
        }

    }

    void ReadListLanguage()
    {
        //Abre una ventana en la que el juador a�ada la ruta en la que quiera 
        string selectedPath = EditorUtility.OpenFilePanel(
          "Select XML File Lenguage Configuration",
          Application.dataPath,
          "xml");
        //Debug.Log(selectedPath);

        if (!string.IsNullOrEmpty(selectedPath))
        {
            inter.ReadListLanguage(selectedPath);
        }
    }

}
