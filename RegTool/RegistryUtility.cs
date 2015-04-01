using Mlm.Business.Utility.Extensions;
using System;
using System.Collections.Generic;
using Win32 = Microsoft.Win32;

namespace RegTool
{
    public static class RegistryUtility
    {
        public static Win32.RegistryKey OpenKey(string key)
        {
            Win32.RegistryKey root;
            if (key.Contains(Win32.Registry.ClassesRoot.Name))
                root = Win32.Registry.ClassesRoot;
            else if (key.Contains(Win32.Registry.CurrentConfig.Name))
                root = Win32.Registry.CurrentConfig;
            else if (key.Contains(Win32.Registry.CurrentUser.Name))
                root = Win32.Registry.CurrentUser;
            else if (key.Contains(Win32.Registry.LocalMachine.Name))
                root = Win32.Registry.LocalMachine;
            else if (key.Contains(Win32.Registry.PerformanceData.Name))
                root = Win32.Registry.PerformanceData;
            else if (key.Contains(Win32.Registry.Users.Name))
                root = Win32.Registry.Users;
            else
                root = Win32.Registry.LocalMachine;

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
        public static bool CopyKey(Win32.RegistryKey parentKey, string keyNameToCopy, string newKeyName)
        {
            //Create new key
            Win32.RegistryKey destinationKey = parentKey.CreateSubKey(newKeyName);

            //Open the sourceKey we are copying from
            Win32.RegistryKey sourceKey = parentKey.OpenSubKey(keyNameToCopy);

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
        public static bool RenameSubKey(this Win32.RegistryKey parentKey, string subKeyName, string newSubKeyName)
        {
            CopyKey(parentKey, subKeyName, newSubKeyName);
            parentKey.DeleteSubKeyTree(subKeyName);
            return true;
        }

        private static void RecurseCopyKey(Win32.RegistryKey sourceKey, Win32.RegistryKey destinationKey)
        {
            //copy all the values
            foreach (string valueName in sourceKey.GetValueNames())
            {
                object objValue = sourceKey.GetValue(valueName);
                Win32.RegistryValueKind valKind = sourceKey.GetValueKind(valueName);
                destinationKey.SetValue(valueName, objValue, valKind);
            }

            //For Each subKey 
            //Create a new subKey in destinationKey 
            //Call myself 
            foreach (string sourceSubKeyName in sourceKey.GetSubKeyNames())
            {
                Win32.RegistryKey sourceSubKey = sourceKey.OpenSubKey(sourceSubKeyName);
                Win32.RegistryKey destSubKey = destinationKey.CreateSubKey(sourceSubKeyName);
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
        public static void Search(Win32.RegistryKey root, List<RegEntry> results, List<string> errors, ref int numScanned, ref string keyword, Action<object> OutputLine)
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
                            entry.Value = entry.Value.ToString().ReplaceCaseInsensitive(token, replace);
                            entry.Save();
                            OutputLine(entry);
                            OutputLine("");
                        }

                        //rename subkey name if contains token
                        if (entry.SubKey.ContainsCaseInsensitive(token))
                        {
                            var key = RegistryUtility.OpenKey(entry.Key);
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
    }
}
