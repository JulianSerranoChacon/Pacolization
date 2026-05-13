using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class InternationalitationGUI : EditorWindow
{
    ExtractClass extract = new ExtractClass();
    FileClass file = new FileClass();

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
        // Boton que busca tus Scriptables
        if (GUILayout.Button("Get All ScriptableObjects"))
        {
            FindAllScriptableObjects();
        }

        //Boton que ejecuta la escritura las cadenas de strings a un CSV
        if (GUILayout.Button("Write To XML"))
        {
            WriteToXML();
        }

        //Boton que ejecuta la lectura de las cadenas de strings de un CSV concreto
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
    void FindAllScriptableObjects()
    {
        extract.FindAssetsByType<ScriptableObject>();
    }
    void WriteToXML()
    {
        string path = Application.streamingAssetsPath + "/xml/ejemplo.xml";
        file.WriteXML(path);
    }

    void ReadFromXML()
    {
        string path = Application.streamingAssetsPath + "/xml/ejemplo_Idiomas.xml";
        file.ReadXML(path);
    }
}
