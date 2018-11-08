using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using JSB.Collections.ConcurrentTrie;

namespace UnitTestNCTrie.ConcurrentTrie
{
  [TestClass]
  public class UnitTestConcurrentTrieIterator
  {
    public void TestConcurrentTrieIterator()
    {
      for (int i = 0; i < 60 * 1000; i += 400 + new Random().Next(400))
      {
        var bt = new ConcurrentTrieDictionary<int, int>();
        for (int j = 0; j < i; j++)
        {
          TestHelper.assertEquals(null, bt.put(j, j));
        }
        int count = 0;
        var set = new HashSet<int>();
        foreach (var e in bt.EntrySet)
        {
          set.Add(e.Key);
          count++;
        }
        foreach (var j in set)
        {
          TestHelper.assertTrue(bt.containsKey(j));
        }
        foreach (var j in bt.Keys)
        {
          TestHelper.assertTrue(set.Contains(j));
        }

        TestHelper.assertEquals(i, count);
        TestHelper.assertEquals(i, bt.size());
      }       
    }
  }
}
