/**
 * Credits for whitepaper publishing concurrent trie implementation:
 * Aleksandar Prokopec EPFL <aleksandar.prokopec@epfl.ch>
 * Nathan G. Bronson Stanford <ngbronson@gmail.com>
 * Phil Bagwell Typesafe <phil.bagwell@typesafe.com>
 * Martin Odersky EPFL <martin.odersky@epfl.ch>
 * -- 
 * This code is port of C-Trie implementation by Roman Levenstein <romixlev@gmail.com>
 * https://github.com/romix/java-concurrent-hash-trie-map 
 * Author: Jose Sebatian Battig <jsbattig@gmail.com> 
 */

using System.Threading;

namespace JSB.Collections.ConcurrentTrie
{
  public abstract class CNodeBase<K, V> : MainNode<K, V> {  
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
