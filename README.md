# UAJ-FinalProject - Pacolization

## Autores
- [Julián Serrano Chacón](https://github.com/JulianSerranoChacon)
- [Jiale He (Chao)](https://github.com/ChaoIsBalling)
- [Jose Antonio Carmona Alfonsel](https://github.com/JoseAntonioCA)
- [Pablo Marcos Serrano](https://github.com/PablooMS)
- [Javier Alonso Ruiz](https://github.com/Javalonso1)
- [Luis Javier Navarrete Pulupa](https://github.com/luisja30)

## Memoria Proyecto Final
https://docs.google.com/document/d/1kHHou4vyVC1tj4d0ZCHDrUKxQv18-MJdiZN4q4rC5qE/edit?usp=sharing

## Guia de usuario

El plugin se ha realizado en la versión de Unity 6000.0.66f2 y 2021.3.6f1 no se asegura el correcto funcionamiento en versiones anteriores.

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
- En caso de usar la opción de automatizar la inclusión de componentes necesarios para el CLAMPLING de los textos de la UI, el usuario deberá de cambiar a mano los tamaños de los cuadros de textos para tener las dimensiones esperadas, además de adecuar el formato de los cuadros de texto (tipo de gameObjects de la UI a tener que usar) y adecuar el formato del Canvas, que se explican en el apartado 3.5 de la memoria.

Example Manual CLAMPLING Implementation:  
  
Expected Hierarchy Scene:
  
<p align="center">
  <img src="Example Manual CLAMPLING Implementation/Expected Hierarchy Scene.png" alt="Expected Hierarchy Scene" width="500"/>
</p>
  
Canvas Scaler Settings:
  
<p align="center">
  <img src="Example Manual CLAMPLING Implementation/Canvas Scaler Settings.png" alt="Canvas Scaler Settings" width="500"/>
</p>
  
Components in PARENT of the Text:
  
<p align="center">
  <img src="Example Manual CLAMPLING Implementation/Components in PARENT of the Text.png" alt="Components in PARENT of the Text" width="500"/>
</p>
  
Components in GameObject with Text:
  
<p align="center">
  <img src="Example Manual CLAMPLING Implementation/Components in GameObject with Text.png" alt="Components in GameObject with Text" width="500"/>
</p>
  