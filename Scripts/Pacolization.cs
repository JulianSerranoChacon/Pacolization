using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pacolization : MonoBehaviour
{
    [SerializeField]
    private bool scanScriptables;
    [SerializeField]
    private string scriptablePath;
    [SerializeField]
    private string[] filePath;
    [SerializeField]
    private string confPath;
    [SerializeField]
    private uint currentLang = 0;
    /*
    [SerializeField]
    public string variablePath;*/
    [SerializeField]
    public string genderConfigurationPath;

    LocalInterface Li;

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

            Li = LocalInterface.Instance();
            Li.Initiate(scanScriptables, scriptablePath);
            Li.StartInExecution(filePath[currentLang], currentLang, confPath, /*variablePath,*/ genderConfigurationPath);
        }
        else
        {
            Destroy(this);
        }
    }

    public void changeLang(int id)
    {
        currentLang =(uint) id;
        Li.changeLang(filePath[id], (uint)id, confPath, /*variablePath,*/ genderConfigurationPath);
    }

     void OnApplicationQuit()
    {
      LocalInterface.Instance().OnQuit();    
    }
}
