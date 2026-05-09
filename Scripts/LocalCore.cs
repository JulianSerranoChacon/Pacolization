using System.Collections.Generic;

public class LocalCore
{

#region Atributos
    //stringTable es un diccionario (en C# se implementan mediante unordered maps). 
    //Cada key alberga un array de tamano languages. 
    //En cada posicion del array se encuentra el string en un idioma concreto.
    private int languages;
    private Dictionary<int, string> stringTable;

    //Marcador que lleva la cuenta del lenguaje actual
    //Funciona para lectura/escritura y ejecucion
    private int currentLang;
#endregion

#region Singleton
    //La clase necesitara ser un singleton ya que solo queremos que exista una
    private LocalCore() {}

    private static LocalCore _instance;

    public static Singleton GetInstance()
    {
        if (_instance == null)
        {
            _instance = new LocalCore();
        }
        return _instance;
    }
#endregion

#region Metodos
    public string getLine(int ID)
    {
        string[] box;

        if(stringTable.TryGetValue(ID, out box))
            return box[currentLang];

        return null;
    }

    public void setLine(int ID, string value)
    {
        string[] box;

        if(stringTable.TryGetValue(ID, out box))
        {
            box[currentLang] = value;
        }
        else
        {
            box = new string[languages]
        }
    }

    //Cambia el idioma que esta usando la clase
    //Falla si es un idioma fuera del alcance especificado
    public void changeLang(int newLang)
    {
        if(newLang < languages)
            currentLang = newLang; 
    }

#endregion
}