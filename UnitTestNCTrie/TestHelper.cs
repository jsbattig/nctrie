using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestNCTrie
{
  public class TestHelper
  {

    public static void assertEquals(long expected, long found)
    {
      Assert.AreEqual(expected, found);
    }

    public static void assertEquals(int expected, int found)
    {
      Assert.AreEqual(expected, found);
    }

    public static void assertEquals(Object expected, Object found)
    {
      Assert.AreEqual(expected, found);
    }

    public static void assertTrue(bool found)
    {
      Assert.IsTrue(found);
    }

    public static void assertFalse(bool found)
    {
      Assert.IsFalse(found);
    }
  }
}
