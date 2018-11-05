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
 * 
 * Mimic Option, None and Some in Scala
 */

namespace ScalaPorts
{
  public class Option<V>
  {
    static None<V> none = new None<V>();

    public static Option<V> makeOption(V o)
    {
      if (o != null)
        return new Some<V>(o);
      else
        return none;
    }

    public static Option<V> makeOption()
    {
      return none;
    }

    public virtual bool nonEmpty()
    {
      return false;
    }
  }

  public class None<V> : Option<V> {
  }

  public class Some<V> : Option<V>
  {
    private V value;

    public Some(V v)
    {
      value = v;
    }

    public V get()
    {
      return value;
    }

    public override bool nonEmpty()
    {
      return value != null;
    }
  }  
}
