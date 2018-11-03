using System.Threading;

namespace JSB.Collections.ConcurrentTrie
{
  abstract class CNodeBase<K, V> : MainNode<K, V> {  
    public volatile int csize = -1;

    public bool CAS_SIZE(int oldval, int nval)
    {
      Interlocked.CompareExchange(ref csize, oldval, nval);
      return csize == nval;
    }

    public void WRITE_SIZE(int nval)
    {
      csize = nval;      
    }

    public int READ_SIZE()
    {
      return csize;
    }
  }
}
