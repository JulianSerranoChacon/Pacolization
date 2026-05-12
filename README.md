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

#### [ExtractClass]

Atributos:
```
- List<TMP_Text> texts
```
Métodos:
```
- ExtractStrings()
```
`ExtractClass` será una clase que extraiga todas las cadenas de texto del juego guardándo los textos en una List de tipo `TMP_Text`.

`ExtractStrings` lo que va a hacer es usando el el `EditorSceneManager`, abre todas las escenas que están en la build con un for, en el modo `Additive` para evitar cerrar la escena activa en la que estamos, y recorre los objetos de la escena para obtener todos los componentes `TMP_Text` y añadirlos a la `List texts`. Antes de seguir con la siguiente escena, esta cierra la escena que abrió a menos de que sea la escena activa.


#### [Translateclass]
//El metodo de getString(ID) con ID como int o string para dejar decidir al usuario 

### 3.3 

### 3.4 Realización de las pruebas

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

