using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace _EverythingTest
{
    [TestClass]
    public class GetAppSettingTest
    {
        [TestMethod]
        public void SetAppSetting_Test1()
        {
            var dir = @"C:\source\VSO\MLM\.shared\SharedAppSettings.Config";
            SetAppSetting.SetAppSetting.Main(new string[] { dir, "SolutionDirectory", DateTime.Now.ToShortDateString()}, true);
        }
    }
}
