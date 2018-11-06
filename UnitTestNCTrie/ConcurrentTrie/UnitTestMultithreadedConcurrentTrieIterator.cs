using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JSB.Collections.ConcurrentTrie;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace UnitTestNCTrie.ConcurrentTrie
{
  [TestClass]
  public class UnitTestMultithreadedConcurrentTrieIterator
  {
    private static int NTHREADS = 7;

    [TestMethod]
    public void testMultiThreadMapIterator()
    {
      ConcurrentTrieDictionary<Object, Object> bt = new ConcurrentTrieDictionary<Object, Object>();
      for (int j = 0; j < 50 * 1000; j++)
      {
        Object[] objects = getObjects(j);
        foreach (Object o in objects)
        {
          bt.put(o, o);
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
              //e.Value = newValue;
            }
          }
        }));

        WaitThreadPoolCompletion();
      }

      count = 0;
      foreach (KeyValuePair<Object, Object> kv in bt.EntrySet)
      {
        Object value = kv.Value;
        TestHelper.assertTrue(value is string);
        count++;
      }
      TestHelper.assertEquals(50000 + 2000 + 1000 + 100, count);

      ConcurrentDictionary<Object, Object> removed = new ConcurrentDictionary<Object, Object>();
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
            Object key = e.Key;
            if (accepts(threadNo, NTHREADS, key))
            {
              if (null == bt.get(key))
              {
                  //System.out.println (key);
                }
                //it.remove ();
                if (null != bt.get(key))
              {
                  //System.out.println (key);
                }
              removed.TryAdd(key, key);
            }
          }
        }));
      }
      WaitThreadPoolCompletion();

      count = 0;
      foreach (var value in bt.Keys)
      {
        count++;
      }
      foreach (var o in bt.Keys)
      {
        if (!removed.ContainsKey(bt.get(o)))
        {
          Console.WriteLine("Not removed: " + o);
        }
      }
      TestHelper.assertEquals(0, count);
      TestHelper.assertEquals(0, bt.size());
      //TestHelper.assertTrue (bt.isEmpty ());
    }

    private void WaitThreadPoolCompletion()
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
