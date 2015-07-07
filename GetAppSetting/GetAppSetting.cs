namespace GetAppSetting
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    public class GetAppSetting
    {
        /// <summary>
        /// Get the app setting with the given key in the given config xml file.
        /// </summary>
        /// <param name="args">
        /// args[0] = configPath; args[1] = key;<para/>
        /// </param>
        public static void Main(string[] args)
        {
            var configPath = args[0];
            var key = args[1];        
            
            if (args.Length < 2)
                throw new ArgumentException(string.Format("Unexpected arguments: 1: configPath = {0}; 2: key = {1})", configPath, key));

            if (!File.Exists(configPath))
                throw new Exception(string.Format("File '{0}' doesn't exist.", configPath));

            var configDoc = XDocument.Load(configPath);

            XAttribute projDirAttr = null;

            XElement appSettings;
            if (configDoc.Root.Name.LocalName.ToLower() == "appsettings")
                appSettings = configDoc.Root;
            else
                appSettings = configDoc.Root.Elements("appSettings").First();

            var projDirSetting = appSettings.Elements("add").FirstOrDefault(a => a.Attribute("key").Value == key);

            if(projDirSetting == null)
                throw new Exception(string.Format("Could not find the app setting with the key of {0}", key));

            projDirAttr = projDirSetting.Attribute("value");

            Console.WriteLine(projDirAttr.Value);
        }
    }
}