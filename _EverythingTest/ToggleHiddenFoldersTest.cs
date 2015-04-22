using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace _EverythingTest
{
    [TestClass]
    public class ToggleHiddenFoldersTest
    {
        [TestMethod]
        public void ToggleHiddenFolders_Test1()
        {
            ToggleHiddenFolders.ToggleHiddenFolders.Main(new string[] {""}, true);
            //Process.Start(@"c:\source\git\mikesocha3\exelibrary\togglehiddenfolders\bin\debug\togglehiddenfolders.exe");
        }
    }
}
