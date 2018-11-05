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

namespace JSB.Collections.ConcurrentTrie
{
  internal class RDCSS_Descriptor<K, V>
  {
    public INode<K, V> old;
    public MainNode<K, V> expectedmain;
    public INode<K, V> nv;
    public volatile bool committed = false;

    public RDCSS_Descriptor(INode<K, V> old, MainNode<K, V> expectedmain, INode<K, V> nv)
    {
      this.old = old;
      this.expectedmain = expectedmain;
      this.nv = nv;
    }

  }
}
