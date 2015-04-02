using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _EverythingTest
{
    [TestClass]
    public class CleanDirectoriesTest
    {
        [TestMethod]
        public void CleanDirectories_Test1()
        {
            CleanDirectories.CleanDirectories.Main(new string[]
            {
                @"C:\source\VSO\MLM|C:\source\VSO\MLM|C:\source\VSO\MLM\.nuget\packages",
                "bin|obj|*"
            }, true);
        }
    }
}
