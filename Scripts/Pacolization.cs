using TMPro;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pacolization : MonoBehaviour
{
    [SerializeField]
    public uint langs;
    [SerializeField]
    public bool scanScriptables;
    [SerializeField]
    public string scriptablePath;
    [SerializeField]
    public string filePath;
    [SerializeField]
    public string confPath;
    [SerializeField]
    public string variablePath;

    private static Pacolization _instance;
    public static Pacolization Instance()
    { return _instance; }
        
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);

            LocalInterface.Instance().ChangeLang(1);
            LocalInterface.Instance().Initiate(langs, scanScriptables, scriptablePath);
            LocalInterface.Instance().StartInExecution(filePath, 0, confPath, variablePath);
        }
        else
        {
            Destroy(this);
        }
    }
     void OnApplicationQuit()
    {
      LocalInterface.Instance().OnQuit();    
    }
}
