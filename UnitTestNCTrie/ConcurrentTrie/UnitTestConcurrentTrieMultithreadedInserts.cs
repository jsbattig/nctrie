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
    static int totalItems = 1000 * 500;

    [TestMethod]
    public void TestConcurrentTrieMultiThreadInserts()
    {      
      ThreadPool.SetMaxThreads(nThreads, nThreads);      
      var bt = new ConcurrentTrieDictionary<Object, Object>();
      for (int i = 0; i < nThreads; i++)
      {
        int threadNo = i;
        ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
        {
          for (int j = 0; j < totalItems; j++)
          {
            if (j % nThreads == threadNo)
            {
              bt.put(j, j);
            }
          }
        }));
      }

      UnitTestMultithreadedConcurrentTrieIterator.WaitThreadPoolCompletion();

      for (int j = 0; j < totalItems; j++)
      {
        Object lookup = bt.lookup(j);
        TestHelper.assertEquals(j, lookup);
      }      
    }

    [TestMethod]
    public void TestConcurrentDictionaryultiThreadInserts()
    {      
      ThreadPool.SetMaxThreads(nThreads, nThreads);
      var bt = new ConcurrentDictionary<Object, Object>();
      for (int i = 0; i < nThreads; i++)
      {
        int threadNo = i;
        ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
        {
          for (int j = 0; j < totalItems; j++)
          {
            if (j % nThreads == threadNo)
            {
              bt.TryAdd(j, j);
            }
          }
        }));
      }

      UnitTestMultithreadedConcurrentTrieIterator.WaitThreadPoolCompletion();

      for (int j = 0; j < totalItems; j++)
      {
        Object lookup;
        TestHelper.assertTrue(bt.TryGetValue(j, out lookup));
        TestHelper.assertEquals(j, lookup);
      }
    }
  }
}
