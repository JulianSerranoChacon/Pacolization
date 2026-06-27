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
    private string[] confPath;
    [SerializeField]
    private uint currentLang = 0;   

    LocalInterface Li;

    private static Pacolization _instance;
    public static Pacolization Instance()
    { return _instance; }
        
    //Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);

            Li = LocalInterface.Instance();
            Li.Initiate(scanScriptables, scriptablePath);
            Li.StartInExecution(filePath[currentLang], currentLang, confPath[currentLang]);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void changeLang(int id)
    {
        currentLang =(uint) id;
        Li.changeLang(filePath[id], (uint)id, confPath[currentLang]);
    }

    public void WriteGenderConf(string key, int value)
    {
        Li.WriteGenderConfToXML(key,value,filePath[currentLang]);
    }

    public void WriteVariables(string key, string value)
    {
        Li.WriteVariables(key,value,filePath[currentLang]);
    }
    
    public void WriteCantidades(string key, int value)
    {
        Li.WriteCantidades(key,value,filePath[currentLang]);
    }


     void OnApplicationQuit()
    {
      LocalInterface.Instance().OnQuit();    
    }
}
