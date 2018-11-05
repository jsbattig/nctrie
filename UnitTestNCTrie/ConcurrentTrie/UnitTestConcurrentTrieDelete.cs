using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JSB.Collections.ConcurrentTrie;

namespace UnitTestNCTrie
{
  [TestClass]
  public class UnitTestConcurrentTrieDelete
  {
    [TestMethod]
    public void TestConcurrentTrieDelete()
    {
      ConcurrentTrieDictionary< Object, Object > bt = new ConcurrentTrieDictionary<Object, Object>();

      for (int i = 0; i < 10000; i++)
      {
        TestHelper.assertEquals(null, bt.put(i, i));
        Object lookup = bt.lookup(i);
        TestHelper.assertEquals(i, lookup);
      }

      checkAddInsert(bt, 536);
      checkAddInsert(bt, 4341);
      checkAddInsert(bt, 8437);

      for (int i = 0; i < 10000; i++)
      {
        bool removed = null != bt.remove(i);
        TestHelper.assertEquals(true, removed);
        Object lookup = bt.lookup(i);
        TestHelper.assertEquals(null, lookup);
      }
    }

    private static void checkAddInsert(ConcurrentTrieDictionary<Object, Object> bt, int k)
    {
      int v = k;
      bt.remove(v);
      Object foundV = bt.lookup(v);
      TestHelper.assertEquals(null, foundV);
      TestHelper.assertEquals(null, bt.put(v, v));
      foundV = bt.lookup(v);
      TestHelper.assertEquals(v, foundV);

      TestHelper.assertEquals(v, bt.put(v, -1));
      TestHelper.assertEquals(-1, bt.put(v, v));
    }
  }
}
