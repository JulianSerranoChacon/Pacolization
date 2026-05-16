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

    public void Initiate(uint lang, bool scan, string path)
    {
        _core = LocalCore.Instance();
        _core.Initiate(lang);
        _extract = new ExtractClass(scan, path);
        _files = new FileClass();
    }

    public void ChangeLang(uint newLang)
    {
        _core.ChangeLang(newLang);
        _core.SetScriptableStrings();
    }

    public void Extract()
    {
        _extract.ExtractStrings();
    }
    
    public void WriteToXML(string path) 
    {
        _files.WriteXML(path,LocalCore.Instance().GetNumLangs());
    }

    public void ReadFromXML(string path, Dictionary<uint, Dictionary<uint, string>> ret,List<string> langNames)
    {
        _files.ReadXML(path,ret,langNames);
    }

    public Dictionary<uint, XmlNode> ReadListLanguage(string path, List<string> lagNames)
    {
        return _files.ReadXMLLanguage(path,lagNames);
    }

    public string GetLine(uint ID)
    {
        //loqueseaClass
       return _core.GetLine(ID);
    }

    public void FullExtract(string path)
    {
        _extract.ExtractStrings();
        _files.WriteXML(path,_core.GetNumLangs());
        _extract.ReplaceStrings();  
    }

    public void StartInExecution(string path, uint lang)
    {
        List<string> langNames = new List<string>();
        _files.ReadXML(path, _core.GetLanguageMap(),langNames);  
        _extract.setScriptableRefereces();
        ChangeLang(lang);
    }

    public void OnQuit()
    {
        _core.FlushScriptableReferences();  
    }
    public void SetupUIClampers()
    {
        if (_extract != null)
        {
            _extract.AutoUIClampSetup();
        }
        else
        {
            UnityEngine.Debug.LogError("�El sistema de localizaci�n no ha sido inicializado todav�a!");
        }
    }


}