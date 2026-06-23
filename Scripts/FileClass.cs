using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using TMPro;
using UnityEditor;
using UnityEngine;

public class FileClass
{
    //Lista de los idiomas ordenados al leer el XML de lenguajes
    private List<string> languagesOrder = new List<string>();
    //Lista que sirve como conversor de nombre del idioma a ID del idioma
    private Dictionary<string, uint> transLang = new Dictionary<string, uint>();
    //Lista en la que guardamos los datos de las variables
    private Dictionary<string, string> variables = new Dictionary<string, string>();
    //Instancia del LocalCore
    private LocalCore _core;

    public FileClass() 
    {
        _core = LocalCore.Instance();
    }

    //Metodo que crea y escribe todos los textos extraidos en un documentoXML
    //Se le pasa como parametro el path en el que se escribirá
    public void WriteXML(string path, uint lenguages)
    {
        //Doc XML donde vamos a guardar los datos del localCore
        XmlDocument xmlDoc = new XmlDocument();

        //Escribimos primero la cabecera del XML
        XmlDeclaration declaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
        xmlDoc.AppendChild(declaration);

        //Elemento raiz del que colgaran todos los textos
        XmlElement root = xmlDoc.CreateElement("translations");
        xmlDoc.AppendChild(root);

        //Diccionario en el que se almacena el ID del texto y su XMLNode (se le setea al LocalCore mas adelante)
        Dictionary<uint, XmlElement> textnodes = new Dictionary<uint, XmlElement>();

        //Recorremos todo el unorderedMap del stringMap
        foreach (KeyValuePair<uint, Dictionary<uint, string>> pair in _core.GetLines)
        {
            //Es el ID del idioma en el unordermap que almacena todos los idiomas
            uint langId = pair.Key;

            //Comprobamos el ID del idioma la que pertenecen los textos
            string langName;
            if (langId < languagesOrder.Count)
                langName = languagesOrder[(int)langId];
            else
                langName = "langNotDefined_" + (int)langId;

            //Recorremos el unorder map del idioma ID correspondiente
            foreach (KeyValuePair<uint, string> item in pair.Value) 
            {
                //Id del texto en el unordered_map
                uint textId = item.Key;
                //Si no esta en la tabla auxiliar de nodos xml creamos el nodo <text>
                if (!textnodes.ContainsKey(textId)) 
                {
                    XmlElement textElement = xmlDoc.CreateElement("text");
                    textElement.SetAttribute("id", textId.ToString());
                    //Añadimos al root el elemento <text>
                    root.AppendChild(textElement);

                    textnodes.Add(textId, textElement);
                }

                //Creamos el nodo hijo del <text> -> <es> <en>....
                XmlElement langNode = xmlDoc.CreateElement(langName);
                langNode.InnerText = item.Value;

                //Almacenamos el nuevo idioma en la lista de nodos del ID
                textnodes[textId].AppendChild(langNode);
            }
        }
        //Antes de acabar guardamos el archivo en la ruta
        xmlDoc.Save(path);
    }
    //Metodo para leer un archivo XML a partir de un filename
 /*   public void ReadXML(string filename) 
    {
        //Leemos el documento de la ruta correspondiente
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(filename);

        //Cogemos todos los textos etiquetados con text  
        XmlNodeList texts = xmlDoc.GetElementsByTagName("text");

        //Recorremos la lista de textos del XML

        for (int i = 0; i < texts.Count; i++) 
        {
            //Id del texto (sera la clave del Diccionario de LocalCore)
            uint id = uint.Parse(texts[i].Attributes["id"].Value);
            //Numero de lenguajes que tiene el texto
            int numLang = texts[i].ChildNodes.Count;

            //Recorremos los hijos
            for (int j = 0; j < numLang; j++)
            {
                //Nodo hijo
                XmlNode lang = texts[i].ChildNodes[j];

                //Compound text es el texto con las variables sustituidas
                string res = CompoundText(lang.InnerText);
                _core.SetLine(id, transLang[lang.Name], res);
            }
        }
    }*/

    //Metodo para leer un archivo XML a partir de un filename
        public void ReadXML(string filename) 
    {

        //Leemos el documento de la ruta correspondiente
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(filename);

        //Cogemos todos los textos etiquetados con text  
        XmlNodeList texts = xmlDoc.GetElementsByTagName("text");
        
        string langName = xmlDoc.GetElementsByTagName("translations")[0].Attributes["lang"];

        //Recorremos la lista de textos del XML

        for (int i = 0; i < texts.Count; i++) 
        {
            //Id del texto (sera la clave del Diccionario de LocalCore)
            uint id = uint.Parse(texts[i].Attributes["id"].Value);

            
            //Compound text es el texto con las variables sustituidas
            string res = CompoundText(texts[i].InnerText);
            _core.SetLine(id, transLang[langName], res);
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
        XmlNodeList texts = xmlDoc.GetElementsByTagName("Lenguaje");

        //Limpiamos primero el orden de los idiomas leidos en el XML
        languagesOrder.Clear();

        //Recorremos los XMLNode 
        foreach (XmlNode node in texts)
        {
            //Id del lenguaje
            uint id = uint.Parse(node.Attributes["id"].Value);

            //añadimos a mapas como lenguajes haya
            _core.AddNewLanguage(id);

            //Nombre del Idioma (etiqueta Lenguaje)
            string langName = node.ChildNodes.Item(0).InnerText;

            //Map con clave el nombre del idioma y el id correspondiente
            transLang.Add(langName,id);

            //Nombre del lenguaje
            languagesOrder.Add(langName);

            //Metemos el lenguaje devuelto con los idiomas y sus parametros
            ret[id] = node;
        }
        _core.SetLanguageConfig(ret);
    }

    //Metodo que permite añadir una variable a un archivo XML, pasandole como parametro el path, y su clave 
    public void WriteVariablesToXML(string path, string key, string value)
    {
        XmlDocument xmlDoc = new XmlDocument();

        // Cargar o crear documento
        if (File.Exists(path))
        {
            xmlDoc.Load(path);
        }
        else
        {
            //creamos la cabecera con la declaracion y el nodo raiz
            XmlDeclaration declaration =
                xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);

            XmlElement root = xmlDoc.CreateElement("Variables");

            xmlDoc.AppendChild(declaration);
            xmlDoc.AppendChild(root);
        }

        // Nodo raíz
        XmlNode rootNode = xmlDoc.SelectSingleNode("/Variables");

        // Buscar variable existente
        XmlElement textElement = rootNode.SelectSingleNode(key) as XmlElement;

        // Crear si no existe
        if (textElement == null)
        {
            textElement = xmlDoc.CreateElement(key);
            rootNode.AppendChild(textElement);
        }

        // Actualizar valor
        textElement.InnerText = value;

        // Escritura inmediata
        using (FileStream fs = new FileStream(
            path,
            FileMode.OpenOrCreate,
            FileAccess.Write,
            FileShare.None))
        {
            fs.SetLength(0); // limpiar archivo anterior

            xmlDoc.Save(fs);

            fs.Flush(true); // forzar escritura física
        }
    }

    //Metodo que lee las variables de un archivo XML dado un path
    public void ReadVariablesToXML(string path)
    {
        //Si el archivo no existe, no devuelve nada
        if (!File.Exists(path))
            return;

        XmlDocument xmlDoc = new XmlDocument();
        //cargamos el documento XML
        xmlDoc.Load(path);

        // Obtener el nodo raíz
        XmlNode rootNode = xmlDoc.SelectSingleNode("Variables");

        if (rootNode == null)
            return;

        //Recorremos todos los nodos hijos del nodo raiz
        foreach (XmlNode c in rootNode.ChildNodes)
        {
            // Evitar nodos raros (#comment, espacios, etc.)
            if (c.NodeType != XmlNodeType.Element)
                continue;

            // Evitar claves duplicadas
            if (!variables.ContainsKey(c.Name))
            {
                variables.Add(c.Name, c.InnerText);
            }
            else
            {
                // Si ya existe, la actualizamos
                variables[c.Name] = c.InnerText;
            }
        }
    }

    //Metodo auxiliar que nos permite buscar un patron !{variable} en un texto y sustituirlo por el valor correspondiente
    private string CompoundText(string text)
    {
        return Regex.Replace(text,@"!\{(.*?)\}",match =>{
            // Cogemos unicamente el contenido entre !{}, es decir, el nombre de la variable Groups[0] seria toda la coincidencia
            string variableName = match.Groups[1].Value;

            //Comprobamos que existe el nombre de la variable y su valor en el dicionario de variables
            if (variables.TryGetValue(variableName, out string value))
            {
                return value;
            }

            // Si no existe la variable, dejamos el texto original
            return match.Value;
        });
    }
}


