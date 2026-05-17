# UAJ-FinalProject - Pacolization

## Autores
- Julián Serrano Chacón
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

Para la extracción, el usuario deberá de abrir la pestaña de Custom Plugins del editor y darle [x] datos.

Para ejecutar necesita la clase [paco o como se llame] y proporcionar a paco [x, y, z]. 

El usuario tendrá que tener en cuenta lo siguiente:
- El usuario deberá comunicarse con LocalInterfaze, no es necesario tocar las clases interiores
- El usuario tendrá que dar a sus idiomas una id lineal eempezando en 0 (por ejemplo, teniendo dos idiomas podría ser `Español 0 English 1`, no `Español 59 English 91`)
- El usuario deberá llevar la cuenta de cuál id corresponde a cada idioma, ya que cambiar de idioma se hace con uint, no con string. 
- Solo se extraerá strings del texto las escenas incluidas en la build del juego
- Si el usuario selecciona extraer el texto de los ScriptableObjects, se extraerá el texto de todos los ScriptableObjects en ese directorio **y los directorios hijos**. Se recomienda cuidado al usuario para evitar tocar ScriptableObjects no propios (de otros paquetes o de Unity)