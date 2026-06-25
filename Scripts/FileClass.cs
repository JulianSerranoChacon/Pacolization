using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using TMPro;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FileClass
{
    //Lista en la que guardamos los datos de las variables
    private Dictionary<string, string> variables = new Dictionary<string, string>();
    //Lista en la que guardamos los datos de los modificadores de genero
    private Dictionary<string, int> generos = new Dictionary<string, int>();

    private Dictionary<string, int> cantidades = new Dictionary<string, int>();

    NumberFormatInfo numberFormatInfo;
    //Instancia del LocalCore
    private LocalCore _core;    

    public FileClass() 
    {
        _core = LocalCore.Instance();
    }

        //Metodo que crea y escribe todos los textos extraidos en un documentoXML
    //Se le pasa como parametro el path en el que se escribirá
    public void WriteXML(string path)
    {
        //Doc XML donde vamos a guardar los datos del localCore
        XmlDocument xmlDoc = new XmlDocument();

        //Escribimos primero la cabecera del XML
        XmlDeclaration declaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
        xmlDoc.AppendChild(declaration);

        //Elemento raiz del que colgaran todos los textos
        XmlElement root = xmlDoc.CreateElement("translations");
        root.SetAttribute("lang","es");
        xmlDoc.AppendChild(root);

        //Diccionario en el que se almacena el ID del texto y su XMLNode (se le setea al LocalCore mas adelante)
        Dictionary<uint, XmlElement> textnodes = new Dictionary<uint, XmlElement>();

        //Recorremos todo el unorderedMap del stringMap
            //Recorremos el unorder map del idioma ID correspondiente
        foreach (KeyValuePair<uint, string> item in _core.GetLines) 
        {
            //Id del texto en el unordered_map
            uint textId = item.Key;
            //Si no esta en la tabla auxiliar de nodos xml creamos el nodo <text>
            if (!textnodes.ContainsKey(textId)) 
            {
                XmlElement textElement = xmlDoc.CreateElement("text");
                textElement.SetAttribute("id", textId.ToString());

                textElement.InnerText = item.Value;
                //Añadimos al root el elemento <text>
                root.AppendChild(textElement);

                textnodes.Add(textId, textElement);

            }
        }
        //Antes de acabar guardamos el archivo en la ruta
        xmlDoc.Save(path);
    }

    //Metodo para leer un archivo XML a partir de un filename
        public void ReadXML(string filename) 
    {

        //Leemos el documento de la ruta correspondiente
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(filename);

        //Cogemos todos los textos etiquetados con text  
        XmlNodeList texts = xmlDoc.GetElementsByTagName("text");
        
        string langName = xmlDoc.GetElementsByTagName("translations")[0].Attributes["lang"].Value;

        //Recorremos la lista de textos del XML

        for (int i = 0; i < texts.Count; i++) 
        {
            //Id del texto (sera la clave del Diccionario de LocalCore)
            uint id = uint.Parse(texts[i].Attributes["id"].Value);

            
            //Compound text es el texto con las variables sustituidas
            //Debug.Log(texts[i].InnerText);
            string res = ModifyGenderText(CompoundText(texts[i].InnerText));
            //Debug.Log(res);
            res = CompoundText(res);
            res = escribeNumerosConfigurados(res);
            res = ModifyPluralsText(res);
            _core.SetLine(id,  res);
        }
    }

    //Metodo que lee el XML de la configuración de lenguajes
    public void ReadXMLLanguage(string filename)
    {
        //Dictionary que se le seteara a la configuracion de lenguaje de LocalCore 
        Dictionary<uint, XmlNode> ret = new Dictionary<uint, XmlNode>();
        
        //Leemos el documento de la ruta correspondiente
        XmlDocument xmlDoc = new XmlDocument();

        //cargamos el archivo
        xmlDoc.Load(filename);

        //Cogemos todos los textos etiquetados con lenguaje 
        XmlNodeList language = xmlDoc.GetElementsByTagName("Lenguaje");
        _core.SetLanguageConfig(language[0]);

        string dec = xmlDoc.SelectSingleNode("//Decimal").InnerText;
        string mil = xmlDoc.SelectSingleNode("//SeparadorMil").InnerText;
        string dinero = xmlDoc.SelectSingleNode("//Dinero").InnerText;
        string posicionStr = xmlDoc.SelectSingleNode("//PosicionMoneda").InnerText;
        int posicionMoneda = int.TryParse(posicionStr, out int pos) ? pos : 3;

        numberFormatInfo = new NumberFormatInfo
        {
            NumberDecimalSeparator = dec,
            NumberGroupSeparator = mil,
            CurrencyDecimalSeparator = dec,
            CurrencyGroupSeparator = mil,
            CurrencySymbol = dinero,
            CurrencyPositivePattern = posicionMoneda
        };
    }

    public string escribeNumerosConfigurados(string textoOriginal)
    {
        if (string.IsNullOrEmpty(textoOriginal)) return string.Empty;

        // Busca la etiqueta [C:numero] (grupo 'moneda') O un número normal (grupo 'numero')
        string patronRegex = @"\[C:(?<moneda>\d+(?:\.\d+)?)\]|(?<numero>\d+(?:\.\d+)?)";

        string textoProcesado = Regex.Replace(textoOriginal, patronRegex, match =>
        {
            // Determinamos si el patrón coincidió con la etiqueta de moneda
            bool esMoneda = match.Groups["moneda"].Success;

            // Obtenemos el texto numérico en base al grupo que tuvo éxito
            string numeroEncontrado = esMoneda ? match.Groups["moneda"].Value : match.Groups["numero"].Value;

            // Lógica para números con decimales
            if (numeroEncontrado.Contains("."))
            {
                if (float.TryParse(numeroEncontrado, NumberStyles.Any, CultureInfo.InvariantCulture, out float valorFloat))
                {
                    if (esMoneda)
                        return valorFloat.ToString("C2", numberFormatInfo);
                    else
                        return valorFloat.ToString("N2", numberFormatInfo);
                }
            }
            // Lógica para números enteros
            else
            {
                if (int.TryParse(numeroEncontrado, out int valorInt))
                {
                    if (esMoneda)
                        return valorInt.ToString("C0", numberFormatInfo);
                    else
                        return valorInt.ToString("N0", numberFormatInfo);
                }
            }

            // Si el TryParse falla por algún motivo, devolvemos el texto original para no romper nada
            return match.Value;
        });

        return textoProcesado;
    }

    public void WriteVariables(string key, string value)
    { 
        if(variables.ContainsKey(key))
            variables[key] = value;
        else 
            variables.Add(key, value);
    }

    //Metodo auxiliar que nos permite buscar un patron !{variable} en un texto y sustituirlo por el valor correspondiente
    private string CompoundText(string text)
    {
        return Regex.Replace(text,@"!\{(.*?)\}",match =>{
            // Cogemos unicamente el contenido entre {}, es decir, el nombre de la variable Groups[0] seria toda la coincidencia
            string variableName = match.Groups[1].Value;

            // Comprobamos que existe el nombre de la variable y su valor en el dicionario de variables
            if (variables.TryGetValue(variableName, out string value))
            {
                return value;
            }

            // Si no existe la variable, devolvemos vacio
            return "";
        });
    }
        //Metodo que permite añadir una modificación de genero al diccionario de modificaciones de genero, pasandole como parametro el nombre key, y su valor value
    public void WriteGenderConfToXML(string key, int value)
    {
        if (generos.ContainsKey(key)) generos[key] = value;   // Si ya existe, la actualizamos
        else generos.Add(key, value);   // Si no existe, la creamos
    }


    //Metodo auxiliar que nos permite buscar un patron {"nombre"parteMasculina|parteFemenina} en un texto y sustituirlo por el valor correspondiente de segun el genero guardado con ese nombre
    private string ModifyGenderText(string text)
    {
        return Regex.Replace(CompoundText(text), @"\[""([^""]+)""\:([^]]+)\]",match =>{
            // Cogemos unicamente el contenido entre {}, es decir, el nombre de la variable Groups[0] seria toda la coincidencia
            string characterName = match.Groups[1].Value;

            // Se divide las opciones entre los | en un array
            string[] options = match.Groups[2].Value.Split('|');

            // Trata de pillar cual el genero que se debe usar entre las opciones. Si no lo encuentra elige el valor 0 por defecto
            int gender;
            if (!generos.TryGetValue(characterName, out gender)) gender = 0;
                
            // Elige la opcion que haya sido indicada. Si hay cualquier fallo no previsto se escoge la primera opcion por defecto
            if (gender >= 0 && gender < options.Length) return options[gender];
            else return options[0];
        });
    }

    public void WriteCantidades(string key, int value)
    {
        if (cantidades.ContainsKey(key))
            cantidades[key] = value;
        else
            cantidades.Add(key, value);
    }

    private string ModifyPluralsText(string text)
    {
        // CORREGIDO: Ahora busca el patrón [P:"variable":singular|plural] para evitar colisiones
        return Regex.Replace(text, @"\[P\:""([^""]+)""\:([^]]+)\]", match => {
            string variableName = match.Groups[1].Value;
            string[] options = match.Groups[2].Value.Split('|');

            if (!cantidades.TryGetValue(variableName, out int cantidad)) cantidad = 0;

            int indiceElegido = 0;

            if (options.Length == 2)
            {
                indiceElegido = (cantidad == 1) ? 0 : 1;
            }
            else if (options.Length >= 3)
            {
                if (cantidad == 0) indiceElegido = 0;
                else if (cantidad == 1) indiceElegido = 1;
                else indiceElegido = 2;
            }

            if (indiceElegido >= 0 && indiceElegido < options.Length) return options[indiceElegido];
            return options[0];
        });
    }
}


