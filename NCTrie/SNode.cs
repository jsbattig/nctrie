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

using System.Collections.Generic;

namespace JSB.Collections.ConcurrentTrie
{
  internal class SNode<K, V> : BasicNode, KVNode<K, V>
  {
    public K k;
    public V v;
    public int hc;

    public SNode(K k, V v, int hc)
    {
      this.k = k;
      this.v = v;
      this.hc = hc;
    }

    SNode<K, V> copy()
    {
      return new SNode<K, V>(k, v, hc);
    }

    public TNode<K, V> copyTombed()
    {
      return new TNode<K, V>(k, v, hc);
    }

    SNode<K, V> copyUntombed()
    {
      return new SNode<K, V>(k, v, hc);
    }

    public KeyValuePair<K, V> kvPair()
    {
      return new KeyValuePair<K, V>(k, v);
    }

    public override string str(int lev)
    {
      // ("  " * lev) + "SNode(%s, %s, %x)".format(k, v, hc);
      return "SNode";
    }
  }
}
