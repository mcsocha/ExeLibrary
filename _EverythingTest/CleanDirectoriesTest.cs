using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _EverythingTest
{
    [TestClass]
    public class CleanDirectoriesTest
    {
        [TestMethod]
        public void CleanDirectories_Test1()
        {
            var dir = @"C:\source\VSO\MLM";
            CleanDirectories.CleanDirectories.Test(new string[] { dir, "obj", "-r"});
        }
    }
}
