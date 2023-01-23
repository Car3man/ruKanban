using System.IO;
using System.Xml;
using RuKanban.Services.AppConfiguration;
using UnityEngine;

namespace RuKanban.Services.Theme
{
    public class ThemeService
    {
        private readonly AppConfigurationService _appConfigurationService;
        private XmlDocument _themeDocument;

        public ThemeService(AppConfigurationService appConfigurationService)
        {
            _appConfigurationService = appConfigurationService;
        }

#if UNITY_EDITOR
        public static Color? GetColorForEditor(string key)
        {
            XmlDocument themeDocument = LoadThemeDocument("DefaultTheme");
            return GetColorFromThemeDoc(themeDocument, key);
        }
        #endif
        
        public Color? GetColor(string key)
        {
            XmlDocument themeDocument = GetThemeDocument();
            return GetColorFromThemeDoc(themeDocument, key);
        }

        private static Color? GetColorFromThemeDoc(XmlDocument themeDocument, string key)
        {
            foreach (XmlNode colorNode in themeDocument.SelectNodes("/theme/colors/*")!)
            {
                if (colorNode.Name != key)
                {
                    continue;
                }
                
                XmlAttribute hexColor = colorNode.Attributes!["color"];
                ColorUtility.TryParseHtmlString(hexColor.Value, out var color);
                return color;
            }

            return default;
        }

        private XmlDocument GetThemeDocument()
        {
            return _themeDocument ??= LoadThemeDocument(_appConfigurationService.ThemeName);
        }
        
        private static XmlDocument LoadThemeDocument(string themeName)
        {
            var themeDocumentText = Resources.Load<TextAsset>($"Themes/{themeName}");
            var themeDocument = new XmlDocument ();
            themeDocument.Load (new StringReader (themeDocumentText.text));
            return themeDocument;
        }
    }
}