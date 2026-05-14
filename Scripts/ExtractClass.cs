using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;


public class ExtractClass 
{
    private bool scanScriptables = false;
    private string scriptablePath = "Assets";
    private uint ID = 0;

    private Dictionary<uint, TMP_Text> objRef;

    public ExtractClass(bool scan, string path) 
    {
        scanScriptables= scan;
        scriptablePath = path;
        objRef = new Dictionary<uint, TMP_Text>();
    }

    //metodo que se usa para encontrar objetos de ciertos tipos en unity
    public void ScanScriptables<T>() where T : UnityEngine.Object
    {
        string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)),new String[]{ scriptablePath});
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]); 
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath); 
            if (asset != null) 
            {
                ExtractValues<T>(asset);
            }
         }
    }

    public void ExtractValues<T>(T obj)
    {
        Type objectType = obj.GetType();
        foreach (FieldInfo m in objectType.GetFields())
        {
                object val = m.GetValue((obj));
                if(val is string)
                {
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
            ScanScriptables<ScriptableObject>();
        }
    }


    public void ReplaceStrings()
    {
        foreach(var item in objRef)
        {
            item.Value.text = item.Key.ToString();
        }
    }
}