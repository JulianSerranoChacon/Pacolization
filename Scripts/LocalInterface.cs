using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
    //private string varPath;
    //private string genConfPath;
#endregion

    //Inicia todo el sistema de localizacion
    //Llamado por la clase Pacolization
    public void Initiate(bool scan, string path)
    {
        _core = LocalCore.Instance();
        _core.Initiate();
        _extract = new ExtractClass(scan, path);
        _files = new FileClass();
    }

    //[NO LLAMAR EN EJECUCION]
    //Lanza el sistema de extraccion 
    public void FullExtract(string path)
    {
        _extract.ExtractStrings();
        _files.WriteXML(path);
        _extract.ReplaceStrings();  
    }

    //Configura el sistema de localizacion para ejecucion
    //Requiere la ruta al archivo XML, el idioma inicial, la ruta a la configuracion y a las variables
    //Llamado por la clase Pacolization
    public void StartInExecution(string path, uint lang, string confpath/*, string vP, string gCP*/)
    {
        //varPath = vP;
        //genConfPath = gCP;
        _files.ReadXMLLanguage(confpath);
        //_files.ReadVariablesToXML(varPath);
        //_files.ReadGenderConfToXML(genConfPath);
        _files.ReadXML(path);
        _extract.setScriptableRefereces();
        changeLang(lang);
    }


    //Configura el sistema de localizacion para ejecucion
    //Requiere la ruta al archivo XML, el idioma inicial, la ruta a la configuracion y a las variables
    //Llamado por la clase Pacolization
    public void changeLang(string path, uint lang, string confpath/*, string vP, string gCP*/)
    {
        //varPath = vP;
        _core.clearMap();
        //genConfPath = gCP;
        _files.ReadXMLLanguage(confpath);
        //_files.ReadVariablesToXML(varPath);
        //_files.ReadGenderConfToXML(genConfPath);
        _files.ReadXML(path);
        _extract.setScriptableRefereces();
        changeLang(lang);
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
    public void changeLang(uint newLang)
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
    /*
    //Metodo usado para crear variables (sexo, nombre, etc)
    public void WriteVariableToXML(string key, string value)
    {
        _files.WriteVariablesToXML(varPath,key,value);
    }*/
    //Metodo usado para crear variables para la configuracion de genero
    public void WriteGenderConfToXML(string key, int value, string path)
    {
        _files.WriteGenderConfToXML(key,value);
        _files.ReadXML(path);
        _core.refreshTexts();
    }

    public void WriteVariables(string key, string value, string path)
    {
        _files.WriteVariables(key,value);
        _files.ReadXML(path);
        _core.refreshTexts();
    }

#region DebugMethods
    //[DEBUG - No Usar]
    public void WriteToXML(string path) 
    {
        _files.WriteXML(path);
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
        //_files.ReadVariablesToXML(path);
    }    
#endregion

}