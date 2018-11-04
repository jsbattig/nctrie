using System;

namespace JSB.Collections.ConcurrentTrie
{
  internal class FailedNode<K, V> : MainNode<K, V>
  {
    MainNode<K, V> p;

    public FailedNode(MainNode<K, V> p)
    {
      this.p = p;
      WRITE_PREV(p);
    }

    public override string str(int lev)
    {
      throw new UnsupportedOperationException();
    }

    public override int cachedSize(Object ct)
    {
      throw new UnsupportedOperationException();
    }

    public override string ToString()
    {
      return string.Format("FailedNode(%s)", p);
    }
  }
}
