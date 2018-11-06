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
  public interface Hashing<K>
  {
    int hash(K k);
  }

  internal class Default<K> : Hashing<K> {
    public int hash(K k)
    {
      int h = k.GetHashCode();
      // This function ensures that hashCodes that differ only by
      // constant multiples at each bit position have a bounded
      // number of collisions (approximately 8 at default load factor).
      h ^= (int)(((uint)h >> 20) ^ ((uint)h >> 12));
      h ^= (int)(((uint)h >> 7) ^ ((uint)h >> 4));
      return h;
    }
    static public Default<K> instance = new Default<K>();
   }
}
