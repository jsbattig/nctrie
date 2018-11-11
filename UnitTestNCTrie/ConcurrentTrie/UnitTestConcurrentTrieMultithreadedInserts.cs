using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JSB.Collections.ConcurrentTrie;
using System.Threading;
using System.Collections.Concurrent;

namespace UnitTestNCTrie.ConcurrentTrie
{
	[TestClass]
	public class UnitTestConcurrentTrieMultithreadedInserts
	{
    static int nThreads = 12;
    static int totalItems = 1000 * 1000 * 30;

    [TestMethod]
    public void TestConcurrentTrieMultiThreadInserts()
    {     
      int counter = nThreads;
      ThreadPool.SetMaxThreads(nThreads, nThreads);
      long mem = GC.GetTotalMemory(true);
      var bt = new ConcurrentTrieDictionary<string, string>();
      for (int i = 0; i < nThreads; i++)
      {
        int threadNo = i;
        ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
        {
          for (int j = 0; j < totalItems; j++)
          {
            if (j % nThreads == threadNo)
            {
              bt.put(j.ToString(), j.ToString());
            }
          }
          Interlocked.Decrement(ref counter);
        }));
      }     

      UnitTestMultithreadedConcurrentTrieIterator.WaitThreadPoolCompletion(ref counter);
      var diff = GC.GetTotalMemory(true) - mem;

      for (int j = 0; j < totalItems; j++)
      {
        string lookup = bt.lookup(j.ToString());
        TestHelper.assertEquals(j, int.Parse(lookup));
      }      
    }

    [TestMethod]
    public void TestConcurrentDictionaryultiThreadInserts()
    {
      int counter = nThreads;
      ThreadPool.SetMaxThreads(nThreads, nThreads);
      long mem = GC.GetTotalMemory(true);
      var bt = new ConcurrentDictionary<string, string>();
      for (int i = 0; i < nThreads; i++)
      {
        int threadNo = i;
        ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
        {
          for (int j = 0; j < totalItems; j++)
          {
            if (j % nThreads == threadNo)
            {
              bt.TryAdd(j.ToString(), j.ToString());
            }
          }
          Interlocked.Decrement(ref counter);
        }));
      }

      UnitTestMultithreadedConcurrentTrieIterator.WaitThreadPoolCompletion(ref counter);
      var diff = GC.GetTotalMemory(true) - mem;

      for (int j = 0; j < totalItems; j++)
      {
        string lookup;
        TestHelper.assertTrue(bt.TryGetValue(j.ToString(), out lookup));
        TestHelper.assertEquals(j, int.Parse(lookup));
      }
    }
  }
}
