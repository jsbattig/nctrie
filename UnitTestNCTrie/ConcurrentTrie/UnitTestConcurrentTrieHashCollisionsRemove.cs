using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JSB.Collections.ConcurrentTrie;

namespace UnitTestNCTrie.ConcurrentTrie
{
  [TestClass]
  public class UnitTestConcurrentTrieHashCollisionsRemove
  {
    [TestMethod]
    public void testHashCollisionsRemove()
    {
      ConcurrentTrieDictionary< Object, Object > bt = new ConcurrentTrieDictionary<Object, Object>();
      int count = 50000;
      for (int j = 0; j < count; j++)
      {
        Object[] objects = UnitTestMultithreadedConcurrentTrieIterator.getObjects(j);
        foreach (Object o in objects)
        {
          bt.put(o, o);
        }
      }
      
      for (int j = 0; j < count; j++)
      {
        Object[] objects = UnitTestMultithreadedConcurrentTrieIterator.getObjects(j);
        foreach (Object o in objects)
        {
          bt.remove(o);
        }
      }

      TestHelper.assertEquals(0, bt.size());
      //TestHelper.assertTrue(bt.isEmpty());
    }
  }
}
