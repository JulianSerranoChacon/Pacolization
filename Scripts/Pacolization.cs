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
        
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);

            Li = LocalInterface.Instance();
            Li.Initiate(scanScriptables, scriptablePath);
            Li.StartInExecution(filePath[currentLang], currentLang, confPath[currentLang]);
           
            WriteVariables("item", "camisa");
            WriteGenderConf("camisa", 1);
            WriteVariables("objeto", "[\"!{item}\": El|La] !{item}");

            WriteVariables("oro", "100000");
            WriteCantidades("oro", 100000);
            WriteCantidades("manzanas", 5); // Cambia dinámicamente en el juego

        }
        else
        {
            Destroy(this);
        }
    }

    public void changeLang(int id)
    {
        currentLang =(uint) id;
        Li.changeLang(filePath[id], (uint)id, confPath[currentLang]);
    }

    public void test()
    {
        WriteVariables("item", "boligrafo");
        WriteGenderConf("boligrafo", 0);
        changeLang(1);

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
