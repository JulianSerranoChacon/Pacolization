using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

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

    public void ChangeLang(int newLang)
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

    public void ReadFromXML(string path)
    {
        _files.ReadXML(path);
    }
    public Dictionary<string, XmlNode>ReadListLanguage(string path, List<string> lagNames)
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

    public void StartInExecution(string path)
    {
        _files.ReadXML(path);
        _extract.GatherTMPReferences();
        _core.ChangeLang(0);
    }
}