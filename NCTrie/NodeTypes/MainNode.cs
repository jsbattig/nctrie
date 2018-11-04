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

using System;
using System.Threading;

namespace JSB.Collections.ConcurrentTrie
{
  public abstract class MainNode<K, V> : BasicNode
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
