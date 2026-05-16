using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using UnityEngine.SceneManagement;

public class LocalInterface
{

    private LocalInterface() {}

#region Singleton
    //La clase necesitara ser un singleton ya que solo queremos que exista una
    private static LocalInterface _instance;

    public static LocalInterface Instance()
    {
         if (_instance == null)
        {
            _instance = new LocalInterface();
        }
        return _instance;
    }
#endregion

#region References
    private LocalCore _core;
    private ExtractClass _extract;
    private FileClass _files;
    //private loqueseaClass _loquesea
#endregion

    public void Initiate(int lang, bool scan, string path)
    {
        _core = LocalCore.Instance();
        _core.Initiate(lang);
        _extract = new ExtractClass(scan, path);
        _files = new FileClass();
        //_loquesea = new loqueseaClass()
    }

    public void ChangeLang(uint newLang)
    {
        _core.ChangeLang(newLang);
    }

    public void Extract()
    {
        _extract.ExtractStrings();
    }
    
    public void WriteToXML(string path) 
    {
        _files.WriteXML(path);
    }

    public void ReadFromXML(string path,List<string> langNames)
    {
        _files.ReadXML(path,langNames);
    }

    public Dictionary<string, XmlNode> ReadListLanguage(string path, List<string> lagNames)
    {
        return _files.ReadXMLLanguage(path,lagNames);
    }

    public void GetLine(int ID)
    {
        //loqueseaClass
    }

    public void FullExtract(string path)
    {
        _extract.ExtractStrings();
        _files.WriteXML(path);
        _extract.ReplaceStrings();  
    }

    public void StartInExecution(string path, uint lang)
    {
        List<string> langNames = new List<string>();
        langNames.Add("Espa�ol");
        langNames.Add("English");
        _files.ReadXML(path, langNames);
        SceneManager.sceneLoaded += NewScene;
        //_extract.GatherTMPReferences();
        //if (scan == true)
        //Extraer scriptable objects a lo mejor?
        _core.ChangeLang(lang);
    }

    public void NewScene(Scene scene, LoadSceneMode mode)
    {
        _core.ClearReferences();
        _extract.GatherTMPReferences();
        _core.SceneLoaded();
    }
}