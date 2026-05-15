using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class UIClamper : MonoBehaviour
{
    public enum ScalingMode { Dynamic, Fixed } //Dynamic es si se quiere que el cuadro de texto se escale de manera dinamica hasta el maximo,
                                               //y Fixed es si queremos que el cuadro de texto este fijo

    [Header("Configuracion del Escalado")]
    [SerializeField]
    private ScalingMode mode = ScalingMode.Dynamic;

    [Tooltip("Limites maximos en unidades del Canvas.")]
    [SerializeField]
    private float maxX = 700f;
    [Tooltip("Limites minimos en unidades del Canvas.")]
    [SerializeField]
    private float maxY = 400f;

    [Header("Configuracion del autosize")]
    [Tooltip("Activa o no el Auto Size del texto")]
    [SerializeField]
    private bool activateAutoSize = false;

    [Tooltip("SIZE minimo del texto")]
    [SerializeField]
    private float minFontSize = 5.0f;

    [Tooltip("SIZE maximo del texto")]
    [SerializeField]
    private float maxFontSize = 15.0f;

    [Tooltip("Porcentaje de compresion del texto")]
    [SerializeField]
    private float wdPercent = 0.0f;

    [Tooltip("Espacio entre lineas del Auto Size, SOLO VALORES NEGATIVOS")]
    [SerializeField]
    private float lineSpacing = 0.0f;

    private RectTransform rectTransform;
    private LayoutElement layout;
    private TMP_Text textComp;
    private RectTransform canvasRect;

    void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        layout = GetComponentInChildren<LayoutElement>();
        textComp = GetComponentInChildren<TMP_Text>();

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvasRect = canvas.GetComponent<RectTransform>();
        }
    }

    void LateUpdate()
    {
        if (!rectTransform || !layout || !textComp || !canvasRect) return;

        // Obtener los limites reales del canvas para no salirse
        float finalLimitX = Mathf.Min(maxX > 0 ? maxX : canvasRect.rect.width, canvasRect.rect.width);
        float finalLimitY = Mathf.Min(maxY > 0 ? maxY : canvasRect.rect.height, canvasRect.rect.height);

        // En funcion de la logica de escalado, el cuadro de texto tendra un SIZE diferente
        if (mode == ScalingMode.Fixed) // Modo FIJO: SIZE maximo desde el principio
        {
            layout.minWidth = finalLimitX;
            layout.minHeight = finalLimitY;
            layout.preferredWidth = finalLimitX;
            layout.preferredHeight = finalLimitY;
        }
        else // Modo DINAMICO: Crecer con el texto
        {

            layout.minWidth = 0;
            layout.minHeight = 0;

            textComp.enableAutoSizing = false;
            textComp.fontSize = textComp.fontSizeMax;

            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

            // Si el texto pide mas de lo permitido en X, ponemos el tope establecido para que no se salga de la pantalla
            if (textComp.preferredWidth > finalLimitX)
            {
                layout.preferredWidth = finalLimitX;
            }
            else
            {
                layout.preferredWidth = -1; // -1 permite que el Content Size Fitter mande, importante
            }

            // Si el texto pide mas de lo permitido en Y, ponemos el tope, como con la X antes
            if (textComp.preferredHeight > finalLimitY)
            {
                layout.preferredHeight = finalLimitY;
            }
            else
            {
                layout.preferredHeight = -1;
            }
        }

        textComp.fontSizeMin = minFontSize;
        textComp.fontSizeMax = maxFontSize;
        textComp.characterWidthAdjustment = wdPercent;
        textComp.lineSpacingAdjustment = lineSpacing;

        // Se aplican los topes de LayoutElement
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        // Disminuimos el SIZE del texto si el cuadro ya no puede crecer mas en NINGUNA direccion
        bool isAtMaxX = rectTransform.rect.width >= finalLimitX - 0.5f;
        bool isAtMaxY = rectTransform.rect.height >= finalLimitY - 0.5f;

        if (isAtMaxX && isAtMaxY)
        {
            // Ambos ejes al limite de SIZE, el texto debe encogerse para caber
            textComp.enableAutoSizing = true;
        }
        else
        {
            // AUn hay espacio para poner el texto en SIZE normal
            textComp.enableAutoSizing = activateAutoSize;
            textComp.fontSize = maxFontSize;
        }

        // Hacemos CLAMP para que el texto NO se salga de la pantalla
        ApplyCanvasClamp();
    }

    void ApplyCanvasClamp()
    {
        Vector3 pos = rectTransform.localPosition;
        Vector2 s = rectTransform.rect.size;
        Vector2 p = rectTransform.pivot;

        // Los limites locales dentro del Rect del Canvas
        float cMinX = canvasRect.rect.xMin + (s.x * p.x);
        float cMaxX = canvasRect.rect.xMax - (s.x * (1 - p.x));
        float cMinY = canvasRect.rect.yMin + (s.y * p.y);
        float cMaxY = canvasRect.rect.yMax - (s.y * (1 - p.y));

        pos.x = Mathf.Clamp(pos.x, cMinX, cMaxX);
        pos.y = Mathf.Clamp(pos.y, cMinY, cMaxY);

        rectTransform.localPosition = pos;
    }
}