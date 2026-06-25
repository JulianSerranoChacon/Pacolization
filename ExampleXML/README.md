#### ADVERTENCIA DE USO DE EXAMPLE XML

Para su correcto funcionamiento se recomienda inicializar las variables, géneros y cantidades con el siguiente código:

```csharp
WriteVariables("item", "camisa");
WriteGenderConf("camisa", 1);
WriteVariables("objeto", "[\"!{item}\": El|La] !{item}");

WriteVariables("oro", "100000");
WriteCantidades("oro", 100000);
WriteCantidades("manzanas", 5); // Cambia dinámicamente en el juego
```

Además se puede comrpobar el cambio dinamico de texto de las variables con la llamada al siguiente metodo:

```csharp
public void test()
{
    WriteVariables("item", "boligrafo");
    WriteGenderConf("boligrafo", 0);
    changeLang(1);

}
```