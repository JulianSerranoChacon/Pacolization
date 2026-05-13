using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class InternationalitationGUI : EditorWindow
{
    ExtractClass extract = new ExtractClass();
    FileClass file = new FileClass();

    private string dir;
    private string langNum;

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
        GUILayout.Label("Plugin Configuration", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();

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
        }

        //Boton que ejecuta la lectura de las cadenas de strings de un XML concreto
        if (GUILayout.Button("Read from XML"))
        {
            ReadFromXML();
        }


    }

    void ModifyStrings()
    {
        Debug.Log("Poner el script de modificacion aqui");
    }

    void ExtractStrings()
    {
        extract.ExtractStrings();
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
            file.WriteXML(dir);
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
            file.ReadXML(selectedPath);
            Debug.Log("File load from: " + selectedPath);
        }
    }
}
