using com.tibbo.aggregate.common.datatable;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AggreGateTests
{
    /// <summary>
    /// Summary description for DataRecordTests
    /// </summary>
    [TestClass]
    public class DataRecordTests
    {
        [TestMethod]
        public void TestFloatFieldToString()
        {
            var dr = new DataRecord( new TableFormat(FieldFormat.create("data", FieldFormat.FLOAT_FIELD)));
            dr.setValue(0, 1.1f);
            Assert.AreEqual("1,1", dr.dataAsString(false, false));
        }

        [TestMethod]
        public void TestDoubleFieldToString()
        {
            var dr = new DataRecord(new TableFormat(FieldFormat.create("data", FieldFormat.DOUBLE_FIELD)));
            dr.setValue(0, 123456789.123456789d);
            Assert.AreEqual("123456789,123457", dr.dataAsString(false, false));
        }
    }
}