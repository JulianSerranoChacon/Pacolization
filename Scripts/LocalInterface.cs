using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
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
    private string varPath;
#endregion

    //Inicia todo el sistema de localizacion
    //Llamado por la clase Pacolization
    public void Initiate(uint lang, bool scan, string path)
    {
        _core = LocalCore.Instance();
        _core.Initiate(lang);
        _extract = new ExtractClass(scan, path);
        _files = new FileClass();
    }

    //[NO LLAMAR EN EJECUCION]
    //Lanza el sistema de extraccion 
    public void FullExtract(string path)
    {
        _core.AddRemainingLanguages();
        _extract.ExtractStrings();
        _files.WriteXML(path,_core.GetNumLangs());
        _extract.ReplaceStrings();  
    }

    //Configura el sistema de localizacion para ejecucion
    //Requiere la ruta al archivo XML, el idioma inicial, la ruta a la configuracion y a las variables
    //Llamado por la clase Pacolization
    public void StartInExecution(string path, uint lang, string confpath, string vP)
    {
        varPath = vP;
        _files.ReadXMLLanguage(confpath);
        _files.ReadVariablesToXML(varPath);
        _files.ReadXML(path);
        _extract.setScriptableRefereces();
        ChangeLang(lang);
    }

    //IMPORTANTE:
    //Llamar cuando se cierra la aplicacion para restaurar ScriptableObjects
    //Llamado por la clase Pacolization
    public void OnQuit()
    {
        _core.FlushScriptableReferences();  
    }

    //Registra un componente de TextUpdate al sistema
    public void RegisterTextUpdate(TextUpdate textUp)
    {
        _core.RegisterTextUpdate(textUp);   
    }

    //Quita la referencia a TextUpdate del sistema
    public void DeregisterTextUpdate(TextUpdate textUp)
    {   if(textUp != null)
            _core.DeregisterTextUpdate(textUp);
    }

    //Devuelve la orientación horizontal del texto
    public bool IsRightToLeft()
    {
        return _core.IsRightToLeft(); 
    }

    //Cambia el idioma actual
    public void ChangeLang(uint newLang)
    {
        _core.ChangeLang(newLang);
        _core.SetScriptableStrings();
    }

    //Devuelve la linea con susodicha ID en el idioma actual
    public string GetLine(uint ID)
    {
       return _core.GetLine(ID);
    }
    
    //Prepara los clamperm de UI
    public void SetupUIClampers()
    {
        if (_extract != null)
            _extract.AutoUIClampSetup();
    }  

#region DebugMethods
    //[DEBUG - No Usar]
    public void WriteToXML(string path) 
    {
        _files.WriteXML(path,LocalCore.Instance().GetNumLangs());
    }

    //[DEBUG - No Usar]
    public void ReadFromXML(string path)
    {
        _files.ReadXML(path);
    }

    //[DEBUG - No Usar]
    public void ReadListLanguage(string path)
    {
        _files.ReadXMLLanguage(path);
    }

    //[DEBUG - No Usar]
    public void ReadListVariables(string path)
    {
        _files.ReadVariablesToXML(path);
    }

    //Metodo usado para crear variables (sexo, nombre, etc)
    public void WriteVariableToXML(string key, string value)
    {
        _files.WriteVariablesToXML(varPath,key,value);
    }
#endregion

}