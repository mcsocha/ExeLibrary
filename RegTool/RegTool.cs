using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegTool
{
    public static class RegTool
    {
        public static RegistryKey OpenKey(string key)
        {
            RegistryKey root;
            if(key.Contains(Registry.ClassesRoot.Name))
                root = Registry.ClassesRoot;
            else if(key.Contains(Registry.CurrentConfig.Name))
                root = Registry.CurrentConfig;
            else if(key.Contains(Registry.CurrentUser.Name))
                root = Registry.CurrentUser;
            else if(key.Contains(Registry.LocalMachine.Name))
                root = Registry.LocalMachine;
            else if(key.Contains(Registry.PerformanceData.Name))
                root = Registry.PerformanceData;
            else if(key.Contains(Registry.Users.Name))
                root = Registry.Users;
            else
                root = Registry.LocalMachine;

            key = key.Replace(root.Name + "\\", "");//remove root portion

            return root.OpenSubKey(key);
        }

        /// <summary>
        /// Copy a registry key.  The parentKey must be writeable.
        /// </summary>
        /// <param name="parentKey"></param>
        /// <param name="keyNameToCopy"></param>
        /// <param name="newKeyName"></param>
        /// <returns></returns>
        public static bool CopyKey(RegistryKey parentKey, string keyNameToCopy, string newKeyName)
        {
            //Create new key
            RegistryKey destinationKey = parentKey.CreateSubKey(newKeyName);

            //Open the sourceKey we are copying from
            RegistryKey sourceKey = parentKey.OpenSubKey(keyNameToCopy);

            RecurseCopyKey(sourceKey, destinationKey);

            return true;
        }

        /// <summary>
        /// Renames a subkey of the passed in registry key since 
        /// the Framework totally forgot to include such a handy feature.
        /// </summary>
        /// <param name="regKey">The RegistryKey that contains the subkey 
        /// you want to rename (must be writeable)</param>
        /// <param name="subKeyName">The name of the subkey that you want to rename
        /// </param>
        /// <param name="newSubKeyName">The new name of the RegistryKey</param>
        /// <returns>True if succeeds</returns>
        public static bool RenameSubKey(this RegistryKey parentKey, string subKeyName, string newSubKeyName)
        {
            CopyKey(parentKey, subKeyName, newSubKeyName);
            parentKey.DeleteSubKeyTree(subKeyName);
            return true;
        }

        private static void RecurseCopyKey(RegistryKey sourceKey, RegistryKey destinationKey)
        {
            //copy all the values
            foreach (string valueName in sourceKey.GetValueNames())
            {
                object objValue = sourceKey.GetValue(valueName);
                RegistryValueKind valKind = sourceKey.GetValueKind(valueName);
                destinationKey.SetValue(valueName, objValue, valKind);
            }

            //For Each subKey 
            //Create a new subKey in destinationKey 
            //Call myself 
            foreach (string sourceSubKeyName in sourceKey.GetSubKeyNames())
            {
                RegistryKey sourceSubKey = sourceKey.OpenSubKey(sourceSubKeyName);
                RegistryKey destSubKey = destinationKey.CreateSubKey(sourceSubKeyName);
                RecurseCopyKey(sourceSubKey, destSubKey);
            }
        }

        /// <summary>
        /// Iterate through registry entries and return the results where the subkey name or value contains the keyword text.
        /// </summary>
        /// <param name="root">Search sub keys recursively starting at this root.</param>
        /// <param name="results"></param>
        /// <param name="errors"></param>
        /// <param name="numScanned"></param>
        /// <param name="keyword"></param>
        /// <param name="Output">The method to call for outputting result details.</param>
        public static void Search(RegistryKey root, List<RegEntry> results, List<string> errors, ref int numScanned, ref string keyword, Action<object> OutputLine)
        {
            try
            {
                foreach (var child in root.GetSubKeyNames())
                {
                    using (var childKey = root.OpenSubKey(child))
                    {
                        Search(childKey, results, errors, ref numScanned, ref keyword, OutputLine);
                    }
                }

                foreach (var valueName in root.GetValueNames())
                {
                    var value = root.GetValue(valueName);

                    if (valueName.ContainsCaseInsensitive(keyword) ||
                        (!string.IsNullOrWhiteSpace(keyword) &&
                        value.GetType() == typeof(string) &&
                        value.ToString().ContainsCaseInsensitive(keyword)))
                    {
                        results.Add(new RegEntry { Key = root.Name, SubKey = valueName, Value = value });
                    }

                    numScanned++;

                    if (numScanned % 10000 == 0)
                        OutputLine(numScanned);
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
        }

        public static void FindReplaceAll(List<RegEntry> entries, List<string> errors, string token, string replace, Action<object> OutputLine)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                foreach (var entry in entries)
                {
                    try
                    {
                        //Reset registry value if type is string and contains token
                        if (entry.IsString && entry.Value.ToString().ContainsCaseInsensitive(token))
                        {
                            OutputLine(entry);
                            OutputLine("changed to");
                            entry.Value =  entry.Value.ToString().ReplaceCaseInsensitive(token, replace);
                            SetRegValue(entry);
                            OutputLine(entry);
                            OutputLine("");
                        }

                        //rename subkey name if contains token
                        if (entry.SubKey.ContainsCaseInsensitive(token))
                        {
                            var key = RegTool.OpenKey(entry.Key);
                            key.RenameSubKey(entry.SubKey, entry.SubKey.ReplaceCaseInsensitive(token, replace));
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add(ex.Message);
                    }
                }
            }
        }

        private static void SetRegValue(RegEntry entry)
        {
            Registry.SetValue(entry.Key, entry.SubKey, entry.Value);
        }
    }
}
