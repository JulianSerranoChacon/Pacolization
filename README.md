# UAJ-FinalProject

## Autores
- Julián Serrano Chacón
- Jiale He
- Jose Antonio Carmona Alfonsel
- Pablo Marcos Serrano
- Javier Alonso Ruiz
- Luis Javier Navarrete Pulupa

# Memoria Proyecto Final
## 1 Resumen del Trabajo
Unity tiene implementado su propio sistema de Internacionalización. Nosotros queremos hacer nuestro propio sistema de internacionalización desde cero que tenga cosas que no están en el de Unity mediante un plugin de Unity que se pueda usar en varios juegos.

Usaremos este repositorio y este Drive.

## 2 Objetivos
Nuestro objetivo principal es implementar dicho sistema como plugin de Unity. Además, lo probaremos en tres juegos 

### 2.1 Funcionalidades a implementar

- Extracción de cadenas de texto del editor (strings en objetos Serializables o públicos en scripts) automáticamente a un StringTable con claves
- Lectura/escritura de CSV que adecue el texto al idioma
- Variables dentro del texto (nombre del jugador, tipos de objeto, etc)
- Modificadores:
    - Modificadores de género
    - Modificadores del tipo de moneda
    - Modificadores de plurales
    - Modificadores de formato de fecha
- Archivos de configuración de idioma:
    - Dirección del texto 
    - Separadores numéricos 
    - Botones y teclas comunes para aceptar, cancelar, moverte
    - Pronombres personales
    - Fuentes
- Compatibilidad con los modificadores de tamaño y sliders de las UIs.
- Una interfaz que como mínimo tenga un botón para extraer las cadenas de texto.


### 2.2 Juegos sobre los que haremos la prueba

Para hacer la prueba necesitaremos juegos que tengan una significante cantidad de texto para poder demostrar las capacidades de nuestro sistema. Extraemos el texto de tres juegos y los doblaremos al Inglés. Adicionalmente, cada juego lo doblaremos a un tercer idioma (usando la ayuda de Google Translator / ChatGPT) para poder demostrar algunas otras características únicas que no se verían en Español e Inglés.

El primer juego que traduciremos será ‘Civilízame Esta’, de Javier Alonso Ruiz. Será un stress-test de la capacidad de texto que puede llegar a tener además de probar la extracción sobre ScriptableObjects. Por eso solo lo localizaremos al Inglés.

El segundo juego que traduciremos será ‘Lagunas Vivas’, de Javier Alonso Ruiz, Julian Serrano Chacón y Alvaro Piña Sánchez-Sierra. Este juego serio lo usaremos para probar el idioma Chino Simplificado y la adaptabilidad de la UI. 

El tercer juego que probaremos será ‘Stay After Class’, de José Antonio Carmona. Lo utilizaremos para probar el texto en Árabe para demostrar la flexibilidad de los archivos de configuración de idioma.

## Diseño/Implementación

### 3.1 Especificaciones internas

#### **Clases Clave**

Para llevar a cabo 

LocalCore se encargará simplemente de guardar los strings. En un primer lugar creíamos que podía manejar también lectura/escritura pero lo dejamos a una clase exterior para seguir los principios del OOP y tener un código más limpio.

La extracción de strings, la lectura y la escritura serán a cargo de [clase]


La comunicación del usuario con el módulo en ejecución y el parseo de strings estará a cargo de [clase]

#### **Implementacion del CLAMP en los textos de la UI en la ESCENA**

A veces un texto puede salirse fuera de los límites porque en un lenguaje es más largo que en otro, pero hay una serie de cosas a tener en cuenta para que esto no pase.

Configuración manual a realizar:

Cuando añadimos un botón o un cuadro de texto por ejemplo, este tiene un objeto HIJO que es el texto. En el objeto PADRE, que es donde esta el cuadro de texto, debemos tener asignados los siguientes componentes: "Content Size Filter" (con "Horizontal Fit" y "Vertical Fit" en valor "Preferred Size"), y "Vertical Layout Group" (donde marcamos ambos valores de "Control Child Size" a TRUE y el resto a FALSE, ponemos el "Child Alingment" en "Middle Center", y asignamos a los valores de "Padding" un valor superior a 0 para que el cuadro no este tan pegado al texto).

En el objeto PADRE debemos de cambiar una cosa del componente "Image", concretamente la variable "Image Type" le cambiamos a "Sliced" para que el cuadro de texto se redimensione mejor y no parezca que se estire como un chicle (la Imagen debe de estar configurada de antes en el Sprite Editor para que se sepan los bordes del cuadro de texto de forma clara), o lo dejamos en "Simple", ya eso es criterio del desarrollador.

También debemos de añadir al PADRE otro componente que está incluido en el plugin, el "UIClamper", que se encarga de mantener el cuadro de texto dentro de la pantalla y procura que el texto se escriba sin desbordarse del cuadro de texto (el cual puede modificarse en plena escritura de manera dinámica o fija de pendiendo de la variable "Mode"), tiene dos parametros "Max X" y "Max Y" que son el ancho y alto maximo de resolución que puede tener el cuadro de texto EN RELACIÓN con la RESOLUCION DEL CANVAS.

Es recomendado que el Canvas Scaler tenga resolucion 1920x1080, y que el "UI Scale Mode" esté puesto en "Scale With Screen Size", además de que a lo mejor es necesario cambiar el "Reference Pixels Per Unit" para ver el tamaño de los elementos de la UI más grandes o más pequeños, pero también habría que cambiar el "Padding" del "Vertical Layout Group" en según qué cuadros de texto, esto último ya es cuestión de probar.

El "UIClamper" también dispone de un apartado de variables que modifican el tamaño y espaciado del texto. "Activate Auto Size" fuerza al componente "TextMeshPro - Text (UI)" del objeto HIJO a activar el "Auto Size" en todo momento, y el resto de variables de "Configuración del autosize" modifican el tamaño y espaciado del texto. Cuanto más bajo sea el valor de "Min Font Size", mejor se asegura de que el texto no se desborde.

En el objeto HIJO, que es el propio texto, también es necesario añadir el componente de "Layout Element" al HIJO, o sino el texto se sale de la pantalla y el "UIClamper" no funciona.

Para cambiar la alineación del texto, si se pone a los lados, en el centro, justificado, etc, hay que ir al objeto HIJO, luego a "TextMeshPro - Text (UI)", y ahí probar las diferentes opciones de "Alignment" agusto del usuario.

#### **Implementacion del CLAMP en los textos de la UI en SCRIPTABLE OBJECTS**

En según qué juegos, se usan Scriptable Objects para los textos de la UI. Es normal usarlos cuando por ejemplo hablas con un NPC y este tiene sus diálogos guardados en un fichero de Scriptable Objects, y para coger y cambiar los textos de la UI por los de los Scriptable Objects se usan scripts que hacen dicho trabajo y son hechos por los desarrolladores del videojuego. Si no se quieren que estos textos se salgan de la pantalla, el proceso de configuración es exactamente el mismo que el que se explicó en el apartado anterior de "textos de la UI en la ESCENA", pero si hay varios textos en un Scriptable Object y se quiere que tengan diferentes tamaños de cuadro de texto, porque a lo mejor un texto es muy corto y otro es muy largo, en el componente de "UIClamper" ponemos "Mode" en "Dynamic" y el cuadro de texto tendrá diferente tamaño dependiendo de la cantidad de texto (aunque se construirá de forma dinámica).

Los apartados anteriores de configuración de textos se enfocan en ser una guía GENERAL y en ofrecer una herramienta GENERAL para que los textos no se salgan, sea el idioma que sea, pero ya si un desarrollador quiere hacer algo más específico, entonces puede coger nuestro plugin y adaptarlo a su juego, ahí es cosa suya.

#### **Implementacion AUTOMATICA del CLAMP en los textos de la UI**
En el plugin de "Internationalitation Plugin", hay una opción después del setup que pone "Auto Setup All UI Clampers". Configura gran parte de las cosas mencionadas anteriormente, añadiendo los componentes necesarios y valores por defecto, salvo por lo del "Image type" en "Rect Transform" del objeto PADRE que eso ya es cosa de tocarlo manual por parte del usuario.

Es totalmente RECOMENDADO (por no decir casi obligatorio) que el Canvas Scaler que contenga los objetos con los recuadros y los textos de la UI tenga resolucion 1920x1080 o superior, y que el "UI Scale Mode" esté puesto en "Scale With Screen Size", o al ejecutar todo de manera automática se pueden descuadrar demasiado las posiciones de los cuadros de textos en la UI.

Aun así, puede que sí o sí haya que ir mirando si cambiar o no el tamaño y la posición de algunos cuadros de texto de manera manual, pero al menos la parte de ir añadiendo componentes está automatizada y reduce la carga de trabajo en el usuario.

#### **SIMPLIFICACIONES:**

- El usuario será el encargado de llevar la cuenta del idioma en el que está y de actualizar a la clase LocalCore cuando cambie. Además, el usuario deberá llevar la cuenta de que int corresponde a cada idioma cuando extraiga los strings o guarde los cambios en el CSV
- Asumiremos que solo leeremos y guardaremos los strings que solo están dentro de la escena y no en los assets (salvo por los scriptable objects, eso lo leemos), para evitar repeticiones y problemas de solapamientos al guardar los datos
- Los strings que leeremos serán strings de escenas que están incluidas en la build del juego.

### 3.2 Clases de Unity

Estructura de clase, como si fuera una API (como el punto 3 de la práctica anterior)

#### **LocalCore**

Atributos:
```
- int languages
- Dictionary<int, string[]> stringTable
- int currentLang
```
Métodos:
```
- Initiate(int langAm)
- GetLine(int ID)
- SetLine(int ID, string value)
- ChangeLang(int newlang)
```

`LocalCore` será una clase que siga el patrón singleton.

`stringTable` es un diccionario (en C# se implementan mediante unordered maps). Cada key alberga un array de tamaño `languages`. En cada posición del array se encuentra el string correspondiente a un idioma distinto para esa key.
`currentLang` indica qué idioma está activo en este momento.

`Initiate` inicia los atributos de la clase. Establece el máximo de idiomas `languages` a `langAm`.

`GetLine` devuelve la línea de la `ID` correspondiente del idioma que está activo.

`SetLine` escribe `value` a la línea de la `ID` correspondiente al idioma que está activo. Si la `ID` es nueva, crea un array.

`ChangeLang` cambia el idioma que está usando la clase. Falla si es un idioma fuera del alcance especificado.

#### **ExtractClass**


Métodos:
```
- ExtractStrings()
```
`ExtractClass` será una clase que extraiga todas las cadenas de texto del juego guardándo los textos en una List temporal de tipo `TMP_Text` que luego lo pasa al `LocalCore`.

`ExtractStrings` lo que va a hacer es usando el el `EditorSceneManager`, abre todas las escenas que están en la build con un for, en el modo `Additive` para evitar cerrar la escena activa en la que estamos, y recorre los objetos de la escena para obtener todos los componentes `TMP_Text` y añadirlos a la `List texts` y luego pasar los textos al `LocalCore`. Antes de seguir con la siguiente escena, esta cierra la escena que abrió a menos de que sea la escena activa.


#### **Translateclass (o transcript class o como la quearais llamar en serio podeis caambiarle el nombre)**
//El metodo de getString(ID) con ID como int o string para dejar decidir al usuario 


#### **Interfaz (oloqueseaelnombre)**
//


### 3.3 Realización de las pruebas

Aurora: Oye que esto es para cuando realicemos las pruebas, explicamos que vamos a hacer arriba.

## 4 Resultados Obtenidos

### 4.1 Problemas durante el desarrollo 

Si vemos que no podemos implementar algo, comentarlo aquí (obviamente en el apartado 3 solo vamos a poner lo que hayamos hecho, esto es en referencia a si nos falta algo del apartado 2)

### 4.2 Objetivos logrados

Aquí simplemente yapear de lo que tenemos y lo que no
Y luego otro párrafo sobre las pruebas en juegos

### 4.3 Extras

Por si hacemos una cosa guapa porque nos sobra el tiempo

## 5 Conclusiones

