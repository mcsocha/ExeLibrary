namespace SetAppSetting
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Collections.Generic;
    using System.Text;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml.Linq;


    public class SetAppSetting
    {
        private static bool _testing = false;

        /// <summary>
        /// Set the app setting with the given key in the given config xml file.
        /// </summary>
        /// <param name="args">
        /// args[0] = configPath; args[1] = key; args[2] = value;<para/>
        /// </param>
        public static void Main(string[] args)
        {
            var configPath = args[0];
            var key = args[1];
            var value = args[2];           
            
            if (args.Length < 3)
                throw new ArgumentException(string.Format("Unexpected arguments: 1: configPath = {0}; 2: key = {1}, 3: value = {2};)", configPath, key, value));

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

            projDirAttr.Value = value;

            configDoc.Save(configPath, SaveOptions.None);

            if (!_testing)
                PromptAnyKey();
        }        

        private static void PromptAnyKey()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    
        public static void Main(string[] args, bool testing)
        {
            _testing = testing;
            Main(args);
        }
    }
}