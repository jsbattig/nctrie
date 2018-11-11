using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JSB.Collections.ConcurrentTrie;
using System.Threading;

namespace UnitTestNCTrie.ConcurrentTrie
{
  [TestClass]
  public class UnitTestConcurrentTrieMultiThreadedAddDelete
  {
    private static int RETRIES = 1;
    private static int N_THREADS = 7;
    private static int COUNT = 50 * 1000;

    [TestMethod]
    public void TestConcurrentHashTrieMultiThreadAddDelete()
    {
      for (int j = 0; j < RETRIES; j++)
      {

        var bt = new ConcurrentTrieDictionary<Object, Object>();
        {
          ThreadPool.SetMaxThreads(N_THREADS, N_THREADS);
          int counter = N_THREADS;
          for (int i = 0; i < N_THREADS; i++)
          {
            int threadNo = i;
            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
            {
              for (int jj = 0; jj < COUNT; jj++)
              {
                if (jj % N_THREADS == threadNo)
                {
                  bt.put(jj, jj);
                }
              }
              Interlocked.Decrement(ref counter);
            }));            
          }        
          UnitTestMultithreadedConcurrentTrieIterator.WaitThreadPoolCompletion(ref counter);

          TestHelper.assertEquals(COUNT, bt.size());
          counter = N_THREADS;

          for (int i = 0; i < N_THREADS; i++) {
            int threadNo = i;
            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
            {
              for (int jj = 0; jj < COUNT; jj++)
              {
                if (jj % N_THREADS == threadNo)
                {
                  bt.remove(jj);
                }
              }
              Interlocked.Decrement(ref counter);
            }));
          }
          UnitTestMultithreadedConcurrentTrieIterator.WaitThreadPoolCompletion(ref counter);

          counter = N_THREADS;
          TestHelper.assertEquals(0, bt.size());
          for (int i = 0; i < N_THREADS; i++) {
            int threadNo = i;
            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
            {
              try
              {
                for (int jj = 0; jj < COUNT; jj++)
                {
                  if (jj % N_THREADS == threadNo)
                  {
                    try
                    {
                      bt.put(jj, jj);
                      if (!bt.containsKey(jj))
                      {
                        Assert.Fail("Key j not found");
                      }
                      bt.remove(jj);
                      if (bt.containsKey(jj))
                      {
                        Assert.Fail("Key jj found and should have been removed");
                      }
                    }
                    catch (Exception e)
                    {
                      Assert.Fail(e.Message);
                    }
                  }
                }
              }
              finally
              {
                Interlocked.Decrement(ref counter);
              }
            }));        

            TestHelper.assertEquals(0, bt.size());
          }
          UnitTestMultithreadedConcurrentTrieIterator.WaitThreadPoolCompletion(ref counter);
        }
      }
    }
  }
}
