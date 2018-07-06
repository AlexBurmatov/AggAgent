using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Management;

namespace WMI_Tests
{
    [TestClass]
    public class WmiTests
    {
        [TestMethod]
        public void TestThereIsAtLeastOneProcessor()
        {
            var selectQuery = new SelectQuery("Win32_Processor");
            var searcher = new ManagementObjectSearcher(selectQuery);
            var results = searcher.Get();
            Assert.IsTrue(results.Count > 0);
        }

        [TestMethod]
        public void TestGettingOnlyManagementObjects()
        {
//            var classes = getClassesIn(new ManagementScope("root\\cimv2"));
//            var classNames = getClassNames(classes);
            var classNames = new List<String>
                                 {
                                     "Win32_Processor", 
                                     "Win32_Process",
                                     "Win32_Service",
                                 };

            foreach (var className in classNames)
            {
                var selectQuery = new SelectQuery(className);
                var searcher = new ManagementObjectSearcher(selectQuery);
                foreach (var each in searcher.Get())
                {
                    Assert.IsTrue(each is ManagementObject);
                }
            }
        }

        private static List<string> getClassNames(ManagementObjectCollection classes)
        {
            var classNames = new List<String>();
            foreach (var eachClass in classes)
            {
                var className = eachClass.SystemProperties["__CLASS"].Value.ToString();
                classNames.Add(className);
            }
            return classNames;
        }

        private static ManagementObjectCollection getClassesIn(ManagementScope scope)
        {
            var query = new WqlObjectQuery("SELECT * FROM meta_class");
            return new ManagementObjectSearcher(scope, query).Get();
        }
    }
}