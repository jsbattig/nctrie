using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JSB.Collections.ConcurrentTrie;

namespace UnitTestNCTrie
{
  [TestClass]
  public class UnitTestConcurrentTrieInsert
  {
    [TestMethod]
    public void TestConcurrentTrieInsert()
    {
      ConcurrentTrieDictionary< Object, Object > bt = new ConcurrentTrieDictionary<Object, Object>();
      TestHelper.assertEquals(null, bt.put("a", "a"));
      TestHelper.assertEquals(null, bt.put("b", "b"));
      TestHelper.assertEquals(null, bt.put("c", "b"));
      TestHelper.assertEquals(null, bt.put("d", "b"));
      TestHelper.assertEquals(null, bt.put("e", "b"));

      for (int i = 0; i < 10000; i++)
      {
        TestHelper.assertEquals(null, bt.put(i, i));
        Object lookup = bt.lookup(i);
        TestHelper.assertEquals(i, lookup);
      }
    }
  }
}
