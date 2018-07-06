using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AggreGateTests
{
    using com.tibbo.aggregate.common.command;
    using com.tibbo.aggregate.common.datatable;
    using com.tibbo.aggregate.common.datatable.field;

    [TestClass]
    public class EncodingTests
    {
        [TestMethod]
        public void TestUtfEncoding()
        {
            string s = "\uFFFF\u0000\u0123";

            DataTable st = new DataTable(new TableFormat(1, 1, FieldFormat.create("f", FieldFormat.STRING_FIELD)), s);

            string enc = st.encode();

            DataTable dt = new DataTable(enc);

            string d = dt.rec().getString("f");


            Assert.AreEqual(s, d);
        }

        [TestMethod]
        public void TestSpecialCharacterEncoding()
        {
            string s = string.Empty + DataTableUtils.ELEMENT_START + DataTableUtils.ELEMENT_END + DataTableUtils.ELEMENT_NAME_VALUE_SEPARATOR;

            s += TransferEncodingHelper.ESCAPE_CHAR + TransferEncodingHelper.SEPARATOR_CHAR;

            s += AggreGateCommand.CLIENT_COMMAND_SEPARATOR;

            s += Command.START_CHAR + Command.END_CHAR;

            DataTable st = new DataTable(new TableFormat(1, 1, FieldFormat.create("f", FieldFormat.STRING_FIELD)), s);

            string enc = st.encode();

            DataTable dt = new DataTable(enc);

            string d = dt.rec().getString("f");


            Assert.AreEqual(s, d);
        }

        [TestMethod]
        public void TestNestedTableEncoding()
        {
            string strdata = "test % %% %%% test";

            TableFormat tf = new TableFormat();

            FieldFormat ff =  FieldFormat.create("strfield", FieldFormat.STRING_FIELD);

            ff.setDefault(strdata);

            tf.addField(ff);

            DataTable table = new DataTable(tf, strdata + "value");

            DataTable wrapped = table;

            for (int i = 0; i < 2; i++)
            {
                TableFormat wtf = new TableFormat();

                FieldFormat wff = FieldFormat.create("dtfield" + i, FieldFormat.DATATABLE_FIELD);

                wff.setDefault(wrapped);

                wtf.addField(wff);

                wrapped = new DataTable(wtf, wrapped);
            }

            string encoded = wrapped.encode(false);

            DataTable restored = new DataTable(encoded, new ClassicEncodingSettings(false), true);


            Assert.AreEqual(wrapped, restored);
        }

    }
}
