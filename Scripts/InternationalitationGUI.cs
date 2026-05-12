using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InternationalitationGUI : EditorWindow
{
    ExtractClass extract = new ExtractClass();
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
    }

    void ModifyStrings()
    {
        Debug.Log("Poner el script de modificacion aqui");
    }

    void ExtractStrings()
    {
        extract.ExtractStrings();
    }
}
