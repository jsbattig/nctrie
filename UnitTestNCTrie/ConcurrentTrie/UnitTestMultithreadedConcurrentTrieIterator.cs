using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JSB.Collections.ConcurrentTrie;
using System.Threading;
using System.Collections.Generic;

namespace UnitTestNCTrie.ConcurrentTrie
{
  public class ObjectHolder
  {
    public object obj;  
  }

  [TestClass]
  public class UnitTestMultithreadedConcurrentTrieIterator
  {
    private static int NTHREADS = 7;

    [TestMethod]
    public void TestMultiThreadConcurrentTrieIterator()
    {
      ConcurrentTrieDictionary<Object, Object> bt = new ConcurrentTrieDictionary<Object, Object>();
      for (int j = 0; j < 50 * 1000; j++)
      {
        Object[] objects = getObjects(j);
        foreach (Object o in objects)
        {
          bt.put(o, new ObjectHolder() { obj = o });
        }
      }

      int count = 0;
      ThreadPool.SetMaxThreads(NTHREADS, NTHREADS);
      for (int i = 0; i < NTHREADS; i++)
      {
        int threadNo = i;
        ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
        {
          for (IEnumerator<KeyValuePair<Object, Object>> it = bt.EntrySet.iterator(); true;)
          {
            if (!it.MoveNext())
              break;
            KeyValuePair<Object, Object> e = it.Current;
            if (accepts(threadNo, NTHREADS, e.Key))
            {
              String newValue = "TEST:" + threadNo;
              ((ObjectHolder)e.Value).obj = newValue;
            }
          }
        }));

        WaitThreadPoolCompletion();
      }

      count = 0;
      foreach (KeyValuePair<Object, Object> kv in bt.EntrySet)
      {
        Object value = ((ObjectHolder)kv.Value).obj;
        TestHelper.assertTrue(value is string);
        count++;
      }
      TestHelper.assertEquals(50000 + 2000 + 1000 + 100, count);      
    }

    public static void WaitThreadPoolCompletion()
    {
      int wt, mwt;
      int cpwt, mcpwt;
      do
      {
        ThreadPool.GetAvailableThreads(out wt, out cpwt);
        ThreadPool.GetMaxThreads(out mwt, out mcpwt);
        Thread.Sleep(1);
      } while (wt < mwt);
    }

    protected static bool accepts(int threadNo, int nThreads, Object key)
    {
      int val = getKeyValue(key);
      if (val >= 0)
        return val % nThreads == threadNo;
      else
        return false;
    }

    private static int getKeyValue(Object key)
    {
      int val = 0;
      if (key is int)
      {
        val = (int)key;
      }
      else if (key is char) {
        val = (int)char.GetNumericValue((char)key) + 1;
      }
        else if (key is short) {
        val = ((short)key) + 2;
      }
        else if (key is Byte) {
        val = ((Byte)key) + 3;
      }
      else
        return -1;
      return val;
    }

    public static Object[] getObjects(int j)
    {
      List<Object> results = new List<Object>();
      results.Add(j);
      if (j < 2000)
      {
        results.Add((char)j);
      }
      if (j < 1000)
      {
        results.Add((short)j);
      }
      if (j < 100)
      {
        results.Add((byte)j);
      }
      return results.ToArray();
    }
  }
}
