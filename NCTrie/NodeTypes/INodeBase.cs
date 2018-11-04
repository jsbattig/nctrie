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
  public abstract class INodeBase<K, V> : BasicNode
  {  
    public static object RESTART = new object();

    public volatile MainNode<K, V> mainnode = null;

    public Gen gen;
    
    public INodeBase(Gen generation)
    {
      gen = generation;
    }

    public BasicNode prev()
    {      
      return null;
    }
  }
}
