using System;
using System.Threading;

namespace JSB.Collections.ConcurrentTrie
{
  abstract class MainNode<K, V> : BasicNode
  {
    public volatile MainNode<K, V> prev = null;

    public abstract int cachedSize(Object ct);

    public bool CAS_PREV(MainNode<K, V> oldval, MainNode<K, V> nval)
    {
      var prev_val = Interlocked.CompareExchange(ref prev, nval, oldval);
      return prev == nval;
    }

    public void WRITE_PREV(MainNode<K, V> nval)
    {
      prev = nval;      
    }

    public MainNode<K, V> READ_PREV()
    {
      return prev;
    }
  }
}
