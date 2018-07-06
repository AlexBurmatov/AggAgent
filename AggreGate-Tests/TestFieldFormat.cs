using System;
using com.tibbo.aggregate.common;
using com.tibbo.aggregate.common.datatable.validator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AggreGateTests
{
    using System.Collections.Generic;

    using com.tibbo.aggregate.common.context;
    using com.tibbo.aggregate.common.datatable;

    [TestClass]
    public class TestFieldFormat
    {
        [TestMethod]
        public void testFieldFormat()
        {
            FieldFormat ff1 = FieldFormat.create("<s1><S><F=N><D=Test>");
            FieldFormat ff2 = FieldFormat.create("<s1><S><D=Test>");
            Assert.IsTrue(ff1.extend(ff2), ff1.extendMessage(ff2));
        }

        [TestMethod]
        public void testDefaultValue()
        {
            FieldFormat ff1 = FieldFormat.create("<s1><F><A=123.456>");
            Assert.IsTrue(Math.Abs(123.456f - (float) ff1.getDefaultValue()) < 0.0000000000001f);
        }
        
        [TestMethod]
        public void testClone()
        {
            string format = "<s1><S><F=N><A=default><D=Test><S=<desc=default><desc2=val2>><V=<L=1 10>>";
            FieldFormat ff = FieldFormat.create(format);
            FieldFormat cl = (FieldFormat) ff.Clone();
            Assert.AreEqual(format, cl.encode(new ClassicEncodingSettings(true)));
         }

        [TestMethod]
        public void testDefaultDescription()
        {
            FieldFormat ff = FieldFormat.create("<theBigValue><S>");

            Assert.AreEqual("The Big Value", ff.getDescription());
        }

        [TestMethod]
        public void testFloatStorage()
        {
            FieldFormat ff = FieldFormat.create("<s1><F>");
            float f = 12345.12f;

            Assert.AreEqual(f, (float) ff.valueFromString(ff.valueToString(f)));
        }

        [TestMethod]
        public void testDoubleStorage()
        {
            FieldFormat ff = FieldFormat.create("<s1><E>");
            double d = 123456789.123456D;

            Assert.AreEqual(d, ff.valueFromString(ff.valueToString(d)));
        }

        [TestMethod]
        public void testHashCodesAreEqual()
        {
            FieldFormat ff1 = FieldFormat.create("<value><E><A=0.0>");
            FieldFormat ff2 = FieldFormat.create("<value><E><A=0.0>");

            Assert.AreEqual(ff1.getDefaultValue().GetHashCode(), ff2.getDefaultValue().GetHashCode());
            //Assert.AreEqual(ff1.description == null) ? 0 : description.GetHashCode());
            //Assert.AreEqual(ff1.editor == null) ? 0 : editor.GetHashCode());
            //Assert.AreEqual(ff1.editorOptions == null) ? 0 : editorOptions.GetHashCode());
            //Assert.AreEqual(ff1.getIcon().GetHashCode(), );
            //result = prime * result + ((group == null) ? 0 : group.GetHashCode());
            //result = prime * result + (extendableSelectionValues ? 1231 : 1237);
            //result = prime * result + ((help == null) ? 0 : help.GetHashCode());
            //result = prime * result + (hidden ? 1231 : 1237);
            //result = prime * result + (inlineData ? 1231 : 1237);
            //result = prime * result + (keyField ? 1231 : 1237);
            //result = prime * result + ((name == null) ? 0 : name.GetHashCode());
            //result = prime * result + (notReplicated ? 1231 : 1237);
            //result = prime * result + (nullable ? 1231 : 1237);
            //result = prime * result + (optional ? 1231 : 1237);
            ////            result = prime * result + (readonly ? 1231 : 1237);
            ////            result = prime * result + (advanced ? 1231 : 1237);
            //Assert.AreEqual(ff1.getSelectionValues().GetHashCode(), ff2.getSelectionValues().GetHashCode());
            //result = prime * result + (transferEncode ? 1231 : 1237);
            Assert.AreEqual(ff1.getValidators().GetHashCode(), ff2.getValidators().GetHashCode());


            Assert.AreEqual(ff1.GetHashCode(), ff2.GetHashCode());
        }

        [TestMethod]
        public void testHashCodesAreEqual2()
        {
            var ff1 = FieldFormat.create("dt", FieldFormat.DATATABLE_FIELD, "dt", new DataTable());
            var ff2 = FieldFormat.create("dt", FieldFormat.DATATABLE_FIELD, "dt", new DataTable());

            //Assert.AreEqual(ff1.getSelectionValues().GetHashCode(), ff2.getSelectionValues().GetHashCode());
            Assert.AreEqual(ff1.getDefaultValue().GetHashCode(), ff2.getDefaultValue().GetHashCode());

            Assert.AreEqual(ff1.GetHashCode(), ff2.GetHashCode());
        }

        [TestMethod]
        public void testHashCodesDiffer()
        {
            FieldFormat ff1 = FieldFormat.create("<value><E><A=0.0>");

            FieldFormat ff2 = FieldFormat.create("<value><I><A=0>");

            Assert.IsFalse(ff1.GetHashCode() == ff2.GetHashCode());
        }

        [TestMethod]
        public void testEquals()
        {
            FieldFormat ff1 = FieldFormat.create("<value><I><A=0>");
            ff1.addValidator(new LimitsValidator(5, 10));
    
            TableFormat tf1 = ff1.wrap();
            tf1.addRecordValidator(new KeyFieldsValidator());
            tf1.addTableValidator(new TableKeyFieldsValidator());
    
            FieldFormat ff2 = FieldFormat.create("<value><I><A=0>");
            ff2.addValidator(new LimitsValidator(5, 10));
    
            TableFormat tf2 = ff2.wrap();
            tf2.addRecordValidator(new KeyFieldsValidator());
            tf2.addTableValidator(new TableKeyFieldsValidator());


            Assert.AreEqual(tf1, tf2);
            Assert.AreEqual(tf1.GetHashCode(), tf2.GetHashCode());
        }
   }
}
