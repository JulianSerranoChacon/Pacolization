# UAJ-FinalProject - Pacolization

## Autores
- [Julián Serrano Chacón](https://github.com/JulianSerranoChacon)
- Jiale He
- Jose Antonio Carmona Alfonsel
- Pablo Marcos Serrano
- Javier Alonso Ruiz
- Luis Javier Navarrete Pulupa

## Memoria Proyecto Final
https://docs.google.com/document/d/1kHHou4vyVC1tj4d0ZCHDrUKxQv18-MJdiZN4q4rC5qE/edit?usp=sharing

## Guia de usuario

[Hablar sobre como meter el paquete, no estoy segura]

El usuario deberá de tener un archivo de configuracion de idiomas o usar el de ejemplo que proporcionamos [blah blah blah].

Para la extracción, el usuario deberá de abrir la pestaña de Custom Plugins del editor. Ahí deberá de introducir los siguientes datos:
- Elegir si escaneará también ScriptaleObjects y la carpeta en la cuál se encuentran estos
- Elegir cuantos idiomas se crearán en el XML en extracción.
- Elegir si se leerá un archivo de settings de idioma para darle idioma al XML leido, y su ruta. Si hay menos que los idiomas especificados, se creará dichos idiomas. Si hay más idiomas que los especificados, puede dar error.
- Elegir si see va a leer un archivo de configuración de variables de idioma, y su ruta.
- Elegir si añade automáticamente los UI Clampers.

Para ejecutar se necesita un objeto vacío con el componente Pacolization y proporcionar a dicho componente el numero de idiomas a usar, la ruta a los ScriptableObjects (si se han usado), la ruta al archivo con los textos traducidos, la ruta al archivo de configuración de idiomas y la ruta a el archivo de variables. El usuario puede si quiere implementar un componente similar por su cuenta pero deberá tener las mismas funciones (ser singleton, ser don't destroy on load y llamar a los susodichos métodos en `Awake` y `OnQuit`.)

El usuario tendrá que tener en cuenta lo siguiente:
- El usuario deberá comunicarse con LocalInterfaze, no es necesario tocar las clases interiores
- El usuario tendrá que dar a sus idiomas una id lineal eempezando en 0 (por ejemplo, teniendo dos idiomas podría ser `Español 0 English 1`, no `Español 59 English 91`)
- El usuario deberá llevar la cuenta de cuál id corresponde a cada idioma, ya que cambiar de idioma se hace con uint, no con string. 
- Solo se extraerá strings del texto las escenas incluidas en la build del juego
- Si el usuario selecciona extraer el texto de los ScriptableObjects, se extraerá el texto de todos los ScriptableObjects en ese directorio **y los directorios hijos**. Se recomienda cuidado al usuario para evitar tocar ScriptableObjects no propios (de otros paquetes o de Unity), o ScriptableObjects que no necesiten ser traducidos (p.ej., ScriptableObjects de guardado o de configuración)
