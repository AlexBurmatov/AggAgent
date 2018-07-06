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
    public class TestTableFormat
    {
        [TestMethod]
        public void testEqualsHashCode()
        {
            TableFormat tf1 = createTestTableFormat();
            TableFormat tf2 = createTestTableFormat();

            var fields1 = tf1.getFields();
            var fields2 = tf2.getFields();
            Assert.AreEqual(fields1[0].GetHashCode(), fields2[0].GetHashCode());
            Assert.AreEqual(fields1[1].GetHashCode(), fields2[1].GetHashCode());
            Assert.AreEqual(fields1[2].GetHashCode(), fields2[2].GetHashCode());

            Assert.AreEqual(fields1.GetHashCode(), fields2.GetHashCode());

            Assert.AreEqual(tf1, tf2);
            Assert.AreEqual(tf1.GetHashCode(), tf2.GetHashCode());
        }

        private FieldValidator NAME_SYNTAX_VALIDATOR = new RegexValidator("\\w+", Cres.get().getString("dtInvalidName"));
        protected TableFormat createTestTableFormat()
        {
            TableFormat format = new TableFormat(1, 1);
            FieldFormat ff = FieldFormat.create("name", FieldFormat.STRING_FIELD, "name", "default name");
            ff.addValidator(NAME_SYNTAX_VALIDATOR);
            format.addField(ff);
            format.addField(FieldFormat.create("dt", FieldFormat.DATATABLE_FIELD, "dt", new DataTable()));
            format.addField(FieldFormat.create("float", FieldFormat.FLOAT_FIELD, "float", 1.5f));
            format.setReorderable(true);
            return format;
        }
    }
}
