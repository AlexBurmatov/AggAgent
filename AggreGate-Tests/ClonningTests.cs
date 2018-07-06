using System;
using System.Collections.Generic;
using com.tibbo.aggregate.common.util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AggreGateTests
{
    [TestClass]
    public class ClonningTests
    {
        #region Running

        [TestInitialize]
        public void SetUp()
        {
        }

        [TestCleanup]
        public void TearDown()
        {
        }

        #endregion

        private static void AssertAreEqualButNotSame(Object obj1, Object obj2)
        {
            Assert.AreEqual(obj1, obj2);
            Assert.AreNotSame(obj1, obj2);
        }

        [TestMethod]
        public void TestObject()
        {
            CloneUtils.genericClone(new Object());
        }

        public struct SimpleStruct : IEquatable<SimpleStruct>
        {
            public int intValue;
            public string stringValue;

            public SimpleStruct(int i, string s)
            {
                intValue = i;
                stringValue = s;
            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public bool Equals(SimpleStruct other)
            {
                return other.intValue == intValue && Equals(other.stringValue, stringValue);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (intValue*397) ^ (stringValue != null ? stringValue.GetHashCode() : 0);
                }
            }
        }

        [TestMethod]
        public void TestStruct()
        {
            var struct1 = new SimpleStruct(101, "abc");
            var struct2 = (SimpleStruct) CloneUtils.genericClone(struct1);

            AssertAreEqualButNotSame(struct1, struct2);

            struct1.intValue = 303;
            Assert.AreEqual(101, struct2.intValue);
            Assert.AreEqual("abc", struct1.stringValue);
            struct2.stringValue = "def";
            Assert.AreEqual("abc", struct1.stringValue);
        }

        public class SimpleClass : ICloneable, IEquatable<SimpleClass>
        {
            public int intValue;
            public string stringValue;
            public SimpleStruct structValue;

            public SimpleClass()
            {
            }

            public SimpleClass(int i, string s)
            {
                intValue = i;
                stringValue = s;
                structValue = new SimpleStruct(i, s);
            }

            public object Clone()
            {
                return MemberwiseClone();
            }

            public bool Equals(SimpleClass other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return other.intValue == intValue && Equals(other.stringValue, stringValue) &&
                       other.structValue.Equals(structValue);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((SimpleClass) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var result = intValue;
                    result = (result*397) ^ (stringValue != null ? stringValue.GetHashCode() : 0);
                    result = (result*397) ^ structValue.GetHashCode();
                    return result;
                }
            }
        }

        [TestMethod]
        public void TestSimpleClass()
        {
            var obj1 = new SimpleClass(101, "abc");
            var obj2 = (SimpleClass) CloneUtils.genericClone(obj1);

            AssertAreEqualButNotSame(obj1, obj2);

            obj1.intValue = 303;
            Assert.AreEqual(101, obj2.intValue);
            obj2.stringValue = "def";
            Assert.AreEqual("abc", obj1.stringValue);
            obj2.structValue.intValue = 505;
            Assert.AreEqual(101, obj1.structValue.intValue);
        }

        public class IncorrectComplexClass : ICloneable, IEquatable<IncorrectComplexClass>
        {
            public SimpleClass objectField;

            public IncorrectComplexClass()
            {
            }

            public IncorrectComplexClass(int i, string s)
            {
                objectField = new SimpleClass(i, s);
            }

            public object Clone()
            {
                return MemberwiseClone();
            }

            public bool Equals(IncorrectComplexClass other)
            {
                if (ReferenceEquals(null, other)) return false;
                return ReferenceEquals(this, other) || Equals(other.objectField, objectField);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == GetType() && Equals((IncorrectComplexClass) obj);
            }


            public override int GetHashCode()
            {
                return (objectField != null ? objectField.GetHashCode() : 0);
            }
        }

        [TestMethod]
        public void TestIncorrectComplexClass()
        {
            var obj1 = new IncorrectComplexClass(101, "abc");
            var obj2 = (IncorrectComplexClass) CloneUtils.genericClone(obj1);

            AssertAreEqualButNotSame(obj1, obj2);

            AssertAreEqualButNotSame(obj1.objectField, obj2.objectField);

            obj1.objectField.intValue = 303;
            Assert.AreEqual(101, obj2.objectField.intValue);
        }

        public class CorrectComplexClass : ICloneable
        {
            public SimpleClass objectField;

            public CorrectComplexClass()
            {
            }

            public CorrectComplexClass(int i, string s)
            {
                objectField = new SimpleClass(i, s);
            }

            public object Clone()
            {
                var copy = (CorrectComplexClass) MemberwiseClone();
                copy.objectField = (SimpleClass) CloneUtils.deepClone(objectField);
                return copy;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == GetType() && Equals((CorrectComplexClass) obj);
            }

            public bool Equals(CorrectComplexClass other)
            {
                if (ReferenceEquals(null, other)) return false;
                return ReferenceEquals(this, other) || Equals(other.objectField, objectField);
            }

            public override int GetHashCode()
            {
                return (objectField != null ? objectField.GetHashCode() : 0);
            }
        }

        [TestMethod]
        public void TestCorrectComplexClass()
        {
            var obj1 = new CorrectComplexClass(101, "abc");
            var obj2 = (CorrectComplexClass) CloneUtils.genericClone(obj1);

            AssertAreEqualButNotSame(obj1, obj2);
            AssertAreEqualButNotSame(obj1.objectField, obj2.objectField);

            obj1.objectField.intValue = 303;
            Assert.AreEqual(101, obj2.objectField.intValue);
        }

        public class CorrectSuperComplexClass : ICloneable
        {
            public CorrectComplexClass objectField;

            public CorrectSuperComplexClass()
            {
            }

            public CorrectSuperComplexClass(int i, string s)
            {
                objectField = new CorrectComplexClass(i, s);
            }

            public object Clone()
            {
                var copy = (CorrectSuperComplexClass) MemberwiseClone();
                copy.objectField = (CorrectComplexClass) CloneUtils.deepClone(objectField);
                return copy;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == GetType() && Equals((CorrectSuperComplexClass) obj);
            }

            public bool Equals(CorrectSuperComplexClass other)
            {
                if (ReferenceEquals(null, other)) return false;
                return ReferenceEquals(this, other) || Equals(other.objectField, objectField);
            }

            public override int GetHashCode()
            {
                return (objectField != null ? objectField.GetHashCode() : 0);
            }
        }

        [TestMethod]
        public void TestCorrectSuperComplexClass()
        {
            var obj1 = new CorrectSuperComplexClass(101, "abc");
            var obj2 = (CorrectSuperComplexClass) CloneUtils.genericClone(obj1);

            AssertAreEqualButNotSame(obj1, obj2);
            AssertAreEqualButNotSame(obj1.objectField, obj2.objectField);
            AssertAreEqualButNotSame(obj1.objectField.objectField.structValue, obj2.objectField.objectField.structValue);

            obj1.objectField.objectField.intValue = 303;
            Assert.AreEqual(101, obj2.objectField.objectField.intValue);
        }

        [TestMethod]
        public void TestList()
        {
            var l1 = new List<CorrectSuperComplexClass>
                         {new CorrectSuperComplexClass(101, "abc"), new CorrectSuperComplexClass(202, "def")};
            var l2 = (List<CorrectSuperComplexClass>) CloneUtils.genericClone(l1);

            Assert.AreNotSame(l1, l2);
            AssertAreEqualButNotSame(l1[0], l2[0]);
            AssertAreEqualButNotSame(l1[1], l2[1]);
        }
    }
}