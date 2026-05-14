using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.VersionControl;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;


public class ExtractClass 
{
    private bool scanScriptables = false;
    private string scriptablePath = "Assets";
    private uint ID = 0;

    private Dictionary<uint, TMP_Text> objRef;
    private Dictionary<uint,Tuple< ScriptableObject,FieldInfo>> scriptObjRef;

    public ExtractClass(bool scan, string path) 
    {
        scanScriptables= scan;
        scriptablePath = path;
        objRef = new Dictionary<uint, TMP_Text>();
        scriptObjRef = new Dictionary<uint, Tuple<ScriptableObject, FieldInfo>>();
    }

    //metodo que se usa para encontrar objetos de ciertos tipos en unity
    public void ScanScriptables()
    {
        string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(ScriptableObject)),new String[]{ scriptablePath});
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]); 
            ScriptableObject asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath); 
            if (asset != null) 
            {
                ExtractValues(asset);
            }
         }
    }

    public void ExtractValues(ScriptableObject obj)
    {
        Type objectType = obj.GetType();
        foreach (FieldInfo m in objectType.GetFields())
        {
                object val = m.GetValue((obj));
                if(val is string)
                {
                    scriptObjRef[ID] = new Tuple<ScriptableObject,FieldInfo>(obj,m);
       
                    LocalCore.Instance().SetLine(ID, (string)val);
                    ID++;
                }      
        }    
    }

    public void ExtractStrings()
    {
        List<TMP_Text> tmp = new List<TMP_Text>();

        //Se crea una nueva lista al principio para evitar que se llene con infomacion repetida
        uint ID = 0;
        //cogemos primero la direccion de las escena en la que estamos
        string activeScenePath = SceneManager.GetActiveScene().path;

        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            string scenePath = EditorBuildSettings.scenes[i].path;
            //En caso de que ya estemos en la escena, no la cargamos
            if (scenePath != activeScenePath)
            {
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }

            foreach (var root in SceneManager.GetSceneByBuildIndex(i).GetRootGameObjects())
            {
                tmp.AddRange(root.GetComponentsInChildren<TMP_Text>(true));
                foreach (TMP_Text text in tmp)
                {
                    LocalCore.Instance().SetLine(ID, text.text);
                    objRef[ID] = text;
                    ID++;
                }
                tmp.Clear();
            }

            //cerramos la escena antes de irnos a la siguiente escena
            if (scenePath != activeScenePath)
            {
                EditorSceneManager.CloseScene(SceneManager.GetSceneByBuildIndex(i), true);
                
            }
        }
        if(scanScriptables)
        {
            ScanScriptables();
        }
    }

    delegate string Convert(uint ID);
    private static string GetLine(uint ID)
    {
        return LocalCore.Instance().GetLine(ID);
    }

    public void ReplaceStrings()
    {
        Convert func = GetLine;

        foreach(var item in objRef)
        {
            item.Value.text = func(item.Key);
        }

        foreach(var item in scriptObjRef)
        {
            item.Value.Item2.SetValue(item.Value.Item1, func(item.Key));
        }
    }
}