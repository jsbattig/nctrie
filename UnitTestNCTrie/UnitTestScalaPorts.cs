using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScalaPorts;

namespace UnitTestCTrie
{
  [TestClass]
  public class UnitTestScalaPorts
  {
    [TestMethod]
    public void TestOptionEmpty()
    {
      var o = Option<int>.makeOption();
      Assert.IsFalse(o.nonEmpty());
    }

    [TestMethod]
    public void TestOptionNonEmpty()
    {
      var o = Option<int>.makeOption(1);
      Assert.IsTrue(o.nonEmpty());
    }

    [TestMethod]
    public void TestSomeEmpty()
    {
      var o = Some<object>.makeOption(null);
      Assert.IsFalse(o.nonEmpty());
    }

    [TestMethod]
    public void TestSomeNonEmpty()
    {
      var o = Some<object>.makeOption(1);
      Assert.IsTrue(o.nonEmpty());
    }

    [TestMethod]
    public void TestSomeGet()
    {
      var o = new Some<object>(1);      
      Assert.AreEqual(1, (int)o.get());
    }

    [TestMethod]
    public void TestMethodNone()
    {
      var n = new None<int>();
      Assert.AreNotEqual(n, null);
    }
  }
}
