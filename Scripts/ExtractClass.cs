using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExtractClass 
{
    // Start is called before the first frame update
    public void FindAssetsByType<T>() where T : UnityEngine.Object
    {
        List<T> assets = new List<T>();
        string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));

        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]); 
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath); 
            if (asset != null) 
            {
              assets.Add(asset);
                Debug.Log(asset);
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
                    LocalCore.GetInstance().SetLine(ID, text.text);
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
        LocalCore.GetInstance().Flush();
    }

}
