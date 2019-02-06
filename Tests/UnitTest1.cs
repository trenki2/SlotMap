using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlotMap;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestGet()
        {
            var m = new SlotMap<object>();
            var obj = new object();

            var key = m.Add(obj);
            Assert.AreEqual(obj, m.Get(key));

            m.TryGet(key, out object result);
            Assert.AreEqual(obj, result);
        }

        [TestMethod]
        public void TestGetInvalidKey()
        {
            var m = new SlotMap<object>();
            var ok = m.TryGet(0, out object result);

            Assert.IsFalse(ok);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestRemove()
        {
            var m = new SlotMap<object>();
            var obj = new object();

            var key = m.Add(obj);
            m.Remove(key);
            var ok = m.TryGet(key, out object result);

            Assert.IsFalse(ok);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestRemoveInvalidKey()
        {
            var m = new SlotMap<object>();
            var ok = m.TryRemove(0);

            Assert.IsFalse(ok);
        }

        [TestMethod]
        public void TestReuseKey()
        {
            var m = new SlotMap<object>();
            var obj = new object();

            var key1 = m.Add(obj);
            m.Remove(key1);
            var key2 = m.Add(obj);

            var ok = m.TryGet(key1, out object result);

            Assert.IsFalse(ok);
            Assert.IsNull(result);

            ok = m.TryGet(key2, out result);

            Assert.IsTrue(ok);
            Assert.AreEqual(obj, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectNotFoundException))]
        public void TestGetException()
        {
            var m = new SlotMap<object>();
            m.Get(0);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectNotFoundException))]
        public void TestRemoveException()
        {
            var m = new SlotMap<object>();
            m.Remove(0);
        }
    }
}
