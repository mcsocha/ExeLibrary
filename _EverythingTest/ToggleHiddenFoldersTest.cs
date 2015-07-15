using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _EverythingTest
{
    [TestClass]
    public class ToggleHiddenFoldersTest
    {
        [TestMethod]
        public void ToggleHiddenFolders_Test1()
        {
            ToggleHiddenFolders.ToggleHiddenFolders.Test(new string[] {""});
            //Process.Start(@"%CMDS%\togglehiddenfolders.exe");
        }
    }
}
