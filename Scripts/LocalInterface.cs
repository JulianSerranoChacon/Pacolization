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

    public void ReadFromXML(string path)
    {
        _files.ReadXML(path);
    }

    public void ReadListLanguage(string path)
    {
        _files.ReadXMLLanguage(path);
    }
    public void ReadListVariables(string path)
    {
        _files.ReadVariablesToXML(path);
    }



    public string GetLine(uint ID)
    {
        //loqueseaClass
       return _core.GetLine(ID);
    }

    public void FullExtract(string path)
    {
        _core.AddRemainingLanguages();
        _extract.ExtractStrings();
        _files.WriteXML(path,_core.GetNumLangs());
        _extract.ReplaceStrings();  
    }

    public void StartInExecution(string path, uint lang, string confpath)
    { 
        _files.ReadXMLLanguage(confpath);
        _files.ReadXML(path);  
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