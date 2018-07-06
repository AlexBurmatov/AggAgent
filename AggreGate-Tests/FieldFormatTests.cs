using System;
using com.tibbo.aggregate.common.datatable;
using com.tibbo.aggregate.common.datatable.field;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AggreGateTests
{
    [TestClass]
    public class FieldFormatTests
    {
        [TestMethod]
        public void TestCreatingString()
        {
            var ff = FieldFormat.create("reference", 'S');
            Assert.IsNotNull(ff);
            Assert.AreEqual(ff.getName(), "reference");
            Assert.AreEqual(ff.getType(), 'S');
        }

        [TestMethod]
        public void TestCreatingBoolean()
        {
            var ff = FieldFormat.create("name", 'B');
            Assert.IsNotNull(ff);
            Assert.AreEqual(ff.getName(), "name");
            Assert.AreEqual(ff.getType(), 'B');
        }

        [TestMethod]
        public void TestComparingStrings()
        {
            Assert.IsTrue(String.Compare("true", "TrUe", true) == 0);
        }

        [TestMethod]
        public void TestReadingDoubles()
        {
            var ff = new DoubleFieldFormat("testDoubleFieldFormat");

            Assert.AreEqual<Double>(0, (double)ff.valueFromString("0"));

            Assert.AreEqual<Double>(0.0d, (double)ff.valueFromString("0"));
            Assert.AreEqual<Double>(-0.0d, (double)ff.valueFromString("-0.0"));

            Assert.AreEqual<Double>(12345.12345, (double)ff.valueFromString("12345.12345"));
            Assert.AreEqual<Double>(-54321.12345, (double)ff.valueFromString("-54321.12345"));

            Assert.AreEqual<Double>(2.2250738585072014E-308, (double)ff.valueFromString("2.2250738585072014E-308"));

            Assert.AreEqual<Double>(4.9E-324, (double)ff.valueFromString("4.9E-324"));
            Assert.AreEqual<Double>(1.7976931348623157E308, (double)ff.valueFromString("1.7976931348623157E308"));

            Assert.AreEqual<Double>(Double.NaN, (double)ff.valueFromString("NaN"));

            Assert.AreEqual<Double>(Double.PositiveInfinity, (double)ff.valueFromString("бесконечность"));
            Assert.AreEqual<Double>(Double.NegativeInfinity, (double)ff.valueFromString("-бесконечность"));
        }

        [TestMethod]
        public void TestWritingDoubles()
        {
            var ff = new DoubleFieldFormat("testDoubleFieldFormat");



            Assert.AreEqual<String>("0", ff.valueToString(0));

            Assert.AreEqual<String>("12345.12345", ff.valueToString(12345.12345));
            Assert.AreEqual<String>("-54321.12345", ff.valueToString(-54321.12345));

            Assert.AreEqual<String>("2.2250738585072E-308", ff.valueToString(2.2250738585072014E-308));
            		


            Assert.AreEqual<String>("4.94065645841247E-324", ff.valueToString(4.9E-324));
            Assert.AreEqual<String>("1.79769313486232E+308", ff.valueToString(Double.MaxValue));

            Assert.AreEqual<String>("NaN", ff.valueToString(Double.NaN));
            Assert.IsTrue(Double.IsNaN((double) ff.valueFromString("NaN")));

            Assert.AreEqual<String>("бесконечность", ff.valueToString(Double.PositiveInfinity));
            Assert.AreEqual<String>("-бесконечность", ff.valueToString(Double.NegativeInfinity));
        }
    }
}
