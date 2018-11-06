using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JSB.Collections.ConcurrentTrie;

namespace UnitTestNCTrie.ConcurrentTrie
{
  [TestClass]
  public class UnitTestConcurrentTrieReplace
  {    
    private static int COUNT = 50 * 1000;

    [TestMethod]
    public void TestConcurrentTrieReplace()
    {
      ConcurrentTrieDictionary< Object, Object > map = new ConcurrentTrieDictionary<Object, Object>();

      for (int i = 0; i < COUNT; i++)
      {
        TestHelper.assertTrue(null == map.replace(i, "lol"));
        TestHelper.assertFalse(map.replace(i, i, "lol2"));
        TestHelper.assertTrue(null == map.put(i, i));
        TestHelper.assertTrue(i.Equals(map.replace(i, "lol")));
        TestHelper.assertFalse(map.replace(i, i, "lol2"));
        TestHelper.assertTrue(map.replace(i, "lol", i));
      }
    }
  }
}
