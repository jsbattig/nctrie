using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JSB.Collections.ConcurrentTrie;
using System.Threading;

namespace UnitTestNCTrie.ConcurrentTrie
{
	[TestClass]
	public class UnitTestConcurrentTrieMultithreadedInserts
	{
		[TestMethod]
    public void TestConcurrentTrieMultiThreadInserts()
    {
      int nThreads = 2;
      ThreadPool.SetMaxThreads(nThreads, nThreads);      
      var bt = new ConcurrentTrieDictionary<Object, Object>();
      for (int i = 0; i < nThreads; i++)
      {
        int threadNo = i;
        ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
        {
          for (int j = 0; j < 500 * 1000; j++)
          {
            if (j % nThreads == threadNo)
            {
              bt.put(j, j);
            }
          }
        }));
      }

      UnitTestMultithreadedConcurrentTrieIterator.WaitThreadPoolCompletion();

      for (int j = 0; j < 500 * 1000; j++)
      {
        Object lookup = bt.lookup(j);
        TestHelper.assertEquals(j, lookup);
      }      
    }
	}
}
