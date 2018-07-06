using com.tibbo.aggregate.common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AggreGateTests
{
    [TestClass]
    public class CresTests
    {
        [TestMethod]
        public void Test()
        {
            Assert.AreEqual(Cres.get().getString("ls"), "LinkServer");
        }
    }
}
