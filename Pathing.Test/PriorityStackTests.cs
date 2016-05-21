using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pathing.Test
{
    using System.Diagnostics;
    using CapitalStaging;

    [TestClass]
    public class PriorityStackTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var stack = new PriorityNodeStack<TestItem>();

            stack.Push(1, new TestItem());

            var top = stack.Pop();

            Assert.AreEqual(stack.KeyIndexLength, 0);
            Assert.AreEqual(stack.PriorityIndexLength, 0);
        }

        public class TestItem
        {
            
        }
    }
}
