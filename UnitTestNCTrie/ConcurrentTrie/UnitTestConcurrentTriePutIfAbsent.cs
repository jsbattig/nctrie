using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JSB.Collections.ConcurrentTrie;

namespace UnitTestNCTrie.ConcurrentTrie
{
  [TestClass]
  public class UnitTestConcurrentTriePutIfAbsent
  {
    private static int COUNT = 50 * 1000;

    [TestMethod]
    public void TestConcurrentTriePutIfAbsent()
    {
      ConcurrentTrieDictionary< Object, Object > map = new ConcurrentTrieDictionary<Object, Object>();

      for (int i = 0; i < COUNT; i++)
      {
        TestHelper.assertTrue(null == map.putIfAbsent(i, i));
        TestHelper.assertTrue(i.Equals(map.putIfAbsent(i, i)));
      }
    }
  }
}
