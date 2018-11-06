using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JSB.Collections.ConcurrentTrie;

namespace UnitTestNCTrie.ConcurrentTrie
{
  [TestClass]
  public class UnitTestConcurrentTrieRemove
  {
    private static int COUNT = 50 * 1000;

    [TestMethod]
    public void TestConcurrentTrieRemove()
    {
      ConcurrentTrieDictionary< Object, Object > map = new ConcurrentTrieDictionary<Object, Object>();

      for (int i = 128; i < COUNT; i++)
      {
        TestHelper.assertFalse(map.remove(i, i));
        TestHelper.assertTrue(null == map.put(i, i));
        TestHelper.assertFalse(map.remove(i, "lol"));
        TestHelper.assertTrue(map.containsKey(i));
        TestHelper.assertTrue(map.remove(i, i));
        TestHelper.assertFalse(map.containsKey(i));
        TestHelper.assertTrue(null == map.put(i, i));
      }
    }
  }
}
