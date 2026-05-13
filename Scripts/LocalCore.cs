using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

public class LocalCore
{

#region Atributos
    //stringTable es un diccionario (en C# se implementan mediante unordered maps). 
    //Cada key alberga un array de tamano languages. 
    //En cada posicion del array se encuentra el string en un idioma concreto.
    private int languages;
    private Dictionary<uint, string[]> stringTable;

    //Marcador que lleva la cuenta del lenguaje actual
    //Funciona para lectura/escritura y ejecucion
    private int currentLang;

    public IReadOnlyDictionary<uint, string[]> GetLines => stringTable;
#endregion

#region Singleton
    //La clase necesitara ser un singleton ya que solo queremos que exista una
    public LocalCore() {}

    private static LocalCore _instance;

    public static LocalCore Instance()
    {
         if (_instance == null)
        {
            _instance = new LocalCore();
        }
        return _instance;
    }
#endregion

#region Metodos
    //Inicia los atributos de la clase
    //Establece el maximo de idiomas a langAm

    public void Initiate(int langAm)
    {
        if(langAm <= 0)
            throw new ArgumentException("Ammount of languages cannot be negative or 0.");

        languages = langAm;
        stringTable = new Dictionary<uint, string[]>();
        currentLang = 0;
    }

    //Devuelve el string de la ID correspondiente del idioma que esta activo.
    public string GetLine(uint ID)
    {
        string[] box;

        if(stringTable.TryGetValue(ID, out box))
            return box[currentLang];
        else
            throw new ArgumentException("No value assigned to corresponding key.");
    }

    //Escribe la linea de la ID correspondiente al idioma que esta activo. 
    //Si la ID es nueva, crea un array y lo almacena
    public void SetLine(uint ID, string value)
    {
        string[] box;

        if(stringTable.TryGetValue(ID, out box))
            box[currentLang] = value;
        else
        {
            box = new string[languages];
            box[currentLang] = value;
            stringTable[ID] = box;
        }

        //Debug.Log(currentLang);
    }

    //Cambia el idioma que esta usando la clase
    //Falla si es un idioma fuera del alcance especificado.
    public void ChangeLang(int newLang)
    {
        if(newLang >= languages)
            throw new ArgumentException("New language value exceeding range of languages.");

        currentLang = newLang; 
    }

#endregion
}