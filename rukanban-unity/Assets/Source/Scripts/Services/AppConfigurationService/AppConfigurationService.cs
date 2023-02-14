using System;
using System.IO;
using System.Xml;
using UnityEngine;

namespace RuKanban.Services.AppConfiguration
{
    public class AppConfigurationService
    {
        public class DesktopWindowScheme
        {
            public readonly int Width;
            public readonly int Height;

            public DesktopWindowScheme(int width, int height)
            {
                Width = width;
                Height = height;
            }
        }
        
        public class ApiScheme
        {
            public readonly string Host;
            public readonly int Port;
            public readonly string BaseUrl;

            public ApiScheme(string host, int port, string baseUrl)
            {
                Host = host;
                Port = port;
                BaseUrl = baseUrl;
            }
        }
        
        public readonly DesktopWindowScheme DesktopWindow;
        public readonly ApiScheme Api;
        public readonly string ThemeName;
        
        public bool IsMobileVersion
        {
            get
            {
#if FORCE_MOBILE
                return true;
#endif
                
#if UNITY_EDITOR
                return false;
#endif
                
#if UNITY_ANDROID || UNITY_IOS
                return true;
#endif
                return false;
            }
        }
        
        public AppConfigurationService()
        {
            XmlDocument xmlConfig = LoadXmlConfiguration();
            DesktopWindow = ParseDesktopWindow(xmlConfig);
            Api = ParseApi(xmlConfig);
            ThemeName = ParseTheme(xmlConfig);
        }

        private XmlDocument LoadXmlConfiguration()
        {
            var xmlText = Resources.Load<TextAsset>("AppConfiguration");
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(new StringReader(xmlText.text));
            return xmlDoc;
        }

        private DesktopWindowScheme ParseDesktopWindow(XmlDocument xmlConfig)
        {
            var desktopWindowNode = xmlConfig.SelectSingleNode("/app/desktop_window");
            if (desktopWindowNode == null)
            {
                throw new Exception("[AppConfigurationService]: Failed to parse 'desktop window' configuration.");
            }

            XmlAttribute widthAttribute = desktopWindowNode.Attributes!["width"];
            XmlAttribute heightAttribute = desktopWindowNode.Attributes!["height"];

            if (widthAttribute == null || heightAttribute == null)
            {
                throw new Exception("[AppConfigurationService]: Failed to parse 'desktop window' configuration.");
            }

            int width = Convert.ToInt32(widthAttribute.Value);
            int height = Convert.ToInt32(heightAttribute.Value);
            return new DesktopWindowScheme(width, height);
        }

        private ApiScheme ParseApi(XmlDocument xmlConfig)
        {
            var apiNode = xmlConfig.SelectSingleNode("/app/api");
            if (apiNode == null)
            {
                throw new Exception("[AppConfigurationService]: Failed to parse 'api' configuration.");
            }

            XmlAttribute hostAttribute = apiNode.Attributes!["host"];
            XmlAttribute portAttribute = apiNode.Attributes!["port"];
            XmlAttribute baseUrlAttribute = apiNode.Attributes!["baseUrl"];

            if (hostAttribute == null || portAttribute == null || baseUrlAttribute == null)
            {
                throw new Exception("[AppConfigurationService]: Failed to parse 'api' configuration.");
            }

            string host = hostAttribute.Value;
            int port = Convert.ToInt32(portAttribute.Value);
            string baseUrl = baseUrlAttribute.Value;
            return new ApiScheme(host, port, baseUrl);
        }
        
        private string ParseTheme(XmlDocument xmlConfig)
        {
            var apiNode = xmlConfig.SelectSingleNode("/app/theme");
            if (apiNode == null)
            {
                throw new Exception("[AppConfigurationService]: Failed to parse 'theme' configuration.");
            }

            XmlAttribute nameAttribute = apiNode.Attributes!["name"];

            if (nameAttribute == null)
            {
                throw new Exception("[AppConfigurationService]: Failed to parse 'theme' configuration.");
            }

            string name = nameAttribute.Value;
            return name;
        }
    }
}