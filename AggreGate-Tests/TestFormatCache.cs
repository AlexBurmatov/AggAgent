using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AggreGateTests
{
    using System.Collections.Generic;

    using com.tibbo.aggregate.common.context;
    using com.tibbo.aggregate.common.datatable;

    [TestClass]
    public class TestFormatCache
    {
        private const string format1 = "<<value><I><F=><A=0>><M=1><X=1>";
        private const string format2 = "<<value><I><F=><A=0><D=>><M=1><X=1>";


        [TestMethod]
        public void testServerFormatCache()
        { 
        TableFormat f1 = new TableFormat(format1, new ClassicEncodingSettings(true));
    
        TableFormat f2 = new TableFormat(format2, new ClassicEncodingSettings(true));

        FormatCache fc = new FormatCache();


            int id = fc.add(f1);
            
        Assert.AreEqual(0, id);

        id = fc.add(f2);
            
        Assert.AreEqual(1, id);

        TableFormat res = fc.get(0);

        Assert.AreSame(f1, res);

        res = fc.get(1);

        Assert.AreSame(f2, res);

        TableFormat newf1 = new TableFormat(format1, new ClassicEncodingSettings(true));

        res = fc.getCachedVersion(newf1);

        Assert.AreSame(f1, res);

        TableFormat newf2 = new TableFormat(format2, new ClassicEncodingSettings(true));

        res = fc.getCachedVersion(newf2);

       Assert.AreSame(f2, res);
    }

        [TestMethod]
        public void testClientFormatCache()
        {
            TableFormat f1 = new TableFormat(format1, new ClassicEncodingSettings(true));
    
            TableFormat f2 = new TableFormat(format2, new ClassicEncodingSettings(true));

            FormatCache fc = new FormatCache();

            fc.put(123, f1);
    
            TableFormat res = fc.get(123);


            Assert.AreSame(f1, res);

            int? id = fc.getId(f1);


            Assert.AreEqual(123, id);

            fc.put(456, f1);
            fc.put(456, f2);
    
            res = fc.get(456);


            Assert.AreSame(f2, res);
    }
}
}
