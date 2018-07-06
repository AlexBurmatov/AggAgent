using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AggreGateTests
{
    using System.Collections.Generic;
    using System.Text;

    using com.tibbo.aggregate.common.context;
    using com.tibbo.aggregate.common.datatable;

    [TestClass]
    public class DataTableEncodingTests
    {
        [TestMethod]
        public void EventDatatableEncoding()
        {
            TableFormat tf = new TableFormat(1, 1);
            tf.addField(FieldFormat.FLOAT_FIELD, "value");
            tf.addField(FieldFormat.INTEGER_FIELD, "quality");

            DataTable nested = new DataTable(tf, 12345.67f, 123);

            ClassicEncodingSettings ces = new ClassicEncodingSettings(false);

            DataTable table = new DataTable(AbstractContext.EF_UPDATED, "test", nested, null);


            string encodedTable = table.encode(ces);
            //  .encode(this.createClassicEncodingSettings(true));

            //string expected = "<F=<<variable><S><A=>><<value><T><A=<F=>>><<user><S><F=N><A=<NULL>>>"
            //    + "<M=1><X=1>><R=<test>"
            //    + "<<F=<<value><F><A=0>>"
            //    + "<<quality><I><A=0>><M=1><X=1>>"
            //    + "<R=<12345.67><123>>><<NULL>>>";
            string expected = "\\u001c\\u0046\\u001e\\u001c\\u001c\\u0076\\u0061\\u0072\\u0069\\u0061\\u0062\\u006c\\u0065\\u001d\\u001c\\u0053\\u001d\\u001c\\u0041\\u001e\\u001d\\u001d\\u001c\\u001c\\u0076\\u0061\\u006c\\u0075\\u0065\\u001d\\u001c\\u0054\\u001d\\u001c\\u0041\\u001e\\u001c\\u0046\\u001e\\u001d\\u001d\\u001d\\u001c\\u001c\\u0075\\u0073\\u0065\\u0072\\u001d\\u001c\\u0053\\u001d\\u001c\\u0046\\u001e\\u004e\\u001d\\u001c\\u0041\\u001e\\u001a\\u001d\\u001d\\u001c\\u004d\\u001e\\u0031\\u001d\\u001c\\u0058\\u001e\\u0031\\u001d\\u001d\\u001c\\u0052\\u001e\\u001c\\u0074\\u0065\\u0073\\u0074\\u001d\\u001c\\u001c\\u0046\\u001e\\u001c\\u001c\\u0076\\u0061\\u006c\\u0075\\u0065\\u001d\\u001c\\u0046\\u001d\\u001c\\u0041\\u001e\\u0030\\u001d\\u001d\\u001c\\u001c\\u0071\\u0075\\u0061\\u006c\\u0069\\u0074\\u0079\\u001d\\u001c\\u0049\\u001d\\u001c\\u0041\\u001e\\u0030\\u001d\\u001d\\u001c\\u004d\\u001e\\u0031\\u001d\\u001c\\u0058\\u001e\\u0031\\u001d\\u001d\\u001c\\u0052\\u001e\\u001c\\u0031\\u0032\\u0033\\u0034\\u0035\\u002e\\u0036\\u0037\\u001d\\u001c\\u0031\\u0032\\u0033\\u001d\\u001d\\u001d\\u001c\\u001a\\u001d\\u001d";

            StringBuilder sb = new StringBuilder();
            foreach (char c in encodedTable)
            {
                sb.Append("\\u");
                sb.Append(string.Format("{0:x4}", (int)c));
            }

            Assert.AreEqual(expected, sb.ToString());

        }

        [TestMethod]
        public void TestFirstEncoding()
        {
            TableFormat tf = new TableFormat(1, 1);
            tf.addField(FieldFormat.FLOAT_FIELD, "value");
            tf.addField(FieldFormat.INTEGER_FIELD, "quality");

            DataTable nested = new DataTable(tf, 12345.67f, 123);

            ClassicEncodingSettings ces = new ClassicEncodingSettings(true);
            ces.setFormatCache(new FormatCache());

            ces.setKnownFormatCollector(new KnownFormatCollector());

            DataTable table = new DataTable(AbstractContext.EF_UPDATED, "test", nested, null);

            String encodedTable = table.encode(ces);
            String actual = "<F=<<variable><S><A=>><<value><T><A=<F=>>><<user><S><F=N><A=<NULL>>>"
                + "<M=1><X=1>><D=0><R=<test>"
                + "<<F=<<value><F><A=0>>"
                + "<<quality><I><A=0>><M=1><X=1>><D=1>"
                + "<R=<12345.67><123>>><<NULL>>>";

            Assert.AreEqual(encodedTable, actual);
        }

        [TestMethod]
        public void TestCachedEncoding()
        {
            TableFormat tf = new TableFormat(1, 1);
            tf.addField(FieldFormat.FLOAT_FIELD, "value");
            tf.addField(FieldFormat.INTEGER_FIELD, "quality");

            DataTable nested = new DataTable(tf, 12345.67f, 123);

            ClassicEncodingSettings ces = new ClassicEncodingSettings(true);
            ces.setFormatCache(new FormatCache());

            ces.setKnownFormatCollector(new KnownFormatCollector());

            DataTable table = new DataTable(AbstractContext.EF_UPDATED, "test", nested, null);

            table.encode(ces);
            String encodedTable = table.encode(ces);
            String actual = "<D=0><R=<test><<D=0><R=<12345.67><123>>><<NULL>>>";
            Assert.AreEqual(encodedTable, actual);
        }

    }
}
