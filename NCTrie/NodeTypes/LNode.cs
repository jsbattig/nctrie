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
using System.Collections.Generic;
using ScalaPorts;

namespace JSB.Collections.ConcurrentTrie
{
  internal class LNode<K, V> : MainNode<K, V>
  {
    ListMap<K, V> listmap;

    public LNode(ListMap<K, V> listmap)
    {
      this.listmap = listmap;
    }

    public LNode(K k, V v) : this(ListMap<K, V>.map(k, v)) {}

    public LNode(K k1, V v1, K k2, V v2) : this(ListMap<K, V>.map(k1, v1, k2, v2)) {}

    public LNode<K, V> inserted(K k, V v)
    {
      return new LNode<K, V>(listmap.add(k, v));
    }

    public MainNode<K, V> removed(K k, ConcurrentTrieDictionary<K, V> ct)
    {
      ListMap<K, V> updmap = listmap.remove(k);
      if (updmap.size() > 1)
        return new LNode<K, V>(updmap);
      else
      {
        updmap.iterator().MoveNext();
        KeyValuePair<K, V> kv = updmap.iterator().Current;
        // create it tombed so that it gets compressed on subsequent
        // accesses
        return new TNode<K, V>(kv.Key, kv.Value, ct.computeHash(kv.getKey()));
      }
    }

    public Option<V> get(K k)
    {
      return listmap.get(k);
    }

    public int cachedSize(Object ct)
    {
      return listmap.size();
    }

    public override string str(int lev)
    {
      // (" " * lev) + "LNode(%s)".format(listmap.mkString(", "))
      return "LNode";
    }
  }
}
