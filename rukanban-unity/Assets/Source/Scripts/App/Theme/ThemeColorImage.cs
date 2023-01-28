using RuKanban.Services.Theme;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Image))]
public class ThemeColorImage : MonoBehaviour
{
    [SerializeField] private string key;

    private Image _image;

    [Inject]
    public void Construct(ThemeService themeService)
    {
        Color? color = themeService.GetColor(key);
        ApplyColor(color);
    }
    
    #if UNITY_EDITOR
    private void OnValidate()
    {
        Color? color = ThemeService.GetColorForEditor(key);
        ApplyColor(color);
    }
    #endif

    private void ApplyColor(Color? color)
    {
        if (!color.HasValue)
        {
            var go = gameObject;
            Debug.LogWarning($"'{go.name}' gameObject tried get color from theme, but it is not found.", go);
            return;
        }

        _image = GetComponent<Image>();
        _image.color = color.Value;
    }
}
