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
  public class Equiv<K>
  {
    private static long serialVersionUID = 1L;

    public bool equiv(K k1, K k2)
    {
      return k1.Equals(k2);
    }
    static public Equiv<K> universal = new Equiv<K>();
  }
}
