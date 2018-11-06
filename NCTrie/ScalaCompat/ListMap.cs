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
 * Mimic immutable ListMap in Scala
 */

using System.Collections;
using System.Collections.Generic;
using ScalaPorts;

namespace JSB.Collections.ConcurrentTrie
{
  public interface ScalaIterator
  {
    bool hasNext();
  }

  abstract public class ListMap<K, V> : IEnumerable<KeyValuePair<K,V>>
  {

    protected ListMap<K, V> _next;

    static public ListMap<K, V> map(K k, V v, ListMap<K, V> tail)
    {
      return new Node(k, v, tail);
    }

    static public ListMap<K, V> map(K k, V v)
    {
      return new Node(k, v, null);
    }

    static public ListMap<K, V> map(K k1, V v1, K k2, V v2)
    {
      return new Node(k1, v1, new Node(k2, v2, null));
    }

    public abstract int size();

    public abstract bool isEmpty();

    abstract public bool contains(K k, V v);

    abstract public bool contains(K key);

    abstract public Option<V> get(K key);

    abstract public ListMap<K, V> add(K key, V value);

    abstract public ListMap<K, V> remove(K key);

    abstract public IEnumerator<KeyValuePair<K, V>> iterator();

    public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
    {
      return iterator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return iterator();
    }

    public class EmptyListMap : ListMap<K, V> {
      public override ListMap<K, V> add(K key, V value)
      {
        return map(key, value, null);
      }

      public override bool contains(K k, V v)
      {
        return false;
      }

      public override bool contains(K k)
      {
        return false;
      }

      public override ListMap<K, V> remove(K key)
      {
        return this;
      }

      public override int size()
      {
        return 0;
      }

      public override bool isEmpty()
      {
        return true;
      }

      public override Option<V> get(K key)
      {
        return Option<V>.makeOption();
      }

      public override IEnumerator<KeyValuePair<K, V>> iterator()
      {
        return new EmptyListMapIterator();
      }
    }

    private class EmptyListMapIterator : IEnumerator<KeyValuePair<K, V>>, ScalaIterator
    {
      static private KeyValuePair<K, V> _emptyKVP = new KeyValuePair<K, V>();

      public object Current { get { return null; } }

      public bool hasNext()
      {
        return false;
      }

      public bool MoveNext() {
        return false;
      }

      public void Reset() { }

      KeyValuePair<K, V> IEnumerator<KeyValuePair<K, V>>.Current
      {
        get { return _emptyKVP; }
      }

      #region IDisposable Support
      protected virtual void Dispose(bool disposing) { }

      public void Dispose()
      {
        Dispose(true);
      }
      #endregion      
    }

    private class Node : ListMap<K, V>
    {
      private K k;
      private V v;

      public Node(K k, V v, ListMap<K, V> next)
      {
        this.k = k;
        this.v = v;
        _next = next;
      }

      public override ListMap<K, V> add(K key, V value)
      {
        return map(key, value, remove(key));
      }

      public override bool contains(K k, V v)
      {
        if (k.Equals(this.k) && v.Equals(this.v))
          return true;
        else if (_next != null)
          return _next.contains(k, v);
        return false;
      }

      public override bool contains(K k)
      {
        if (k.Equals(this.k))
          return true;
        else if (_next != null)
          return _next.contains(k);
        return false;
      }

      public override ListMap<K, V> remove(K key)
      {
        if (!contains(key))
          return this;
        else
          return remove0(key);
      }

      private ListMap<K, V> remove0(K key)
      {
        ListMap<K, V> n = this;
        ListMap<K, V> newN = null;
        ListMap<K, V> lastN = null;
        while (n != null)
        {
          if (n is EmptyListMap)
          {
            newN._next = n;
            break;
          }
          Node nn = (Node)n;
          if (key.Equals(nn.k))
          {
            n = n._next;
            continue;
          }
          else
          {
            if (newN != null)
            {
              lastN._next = map(nn.k, nn.v, null);
              lastN = lastN._next;
            }
            else
            {
              newN = map(nn.k, nn.v, null);
              lastN = newN;
            }
          }
          n = n._next;
        }
        return newN;
      }

      public override int size()
      {
        if (_next == null)
          return 1;
        else
          return 1 + _next.size();
      }

      public override bool isEmpty()
      {
        return false;
      }

      public override Option<V> get(K key)
      {
        if (key.Equals(k))
          return Option<V>.makeOption(v);
        if (_next != null)
          return _next.get(key);
        return Option<V>.makeOption();
      }


      public override IEnumerator<KeyValuePair<K, V>> iterator()
      {
        return new NodeIterator(this);
      }

      private class NodeIterator : IEnumerator<KeyValuePair<K, V>>, ScalaIterator
      {
        static private KeyValuePair<K, V> _emptyKVP = new KeyValuePair<K, V>();
        ListMap<K, V> n;

        public NodeIterator(Node n)
        {
          this.n = n;
        }

        public object Current { get { return null; } }

        public bool hasNext()
        {
          return n != null && !(n is EmptyListMap);
        }

        public bool MoveNext()
        {
          return n != null && !(n is EmptyListMap);
        }

        public void Reset() { }

        KeyValuePair<K, V> IEnumerator<KeyValuePair<K, V>>.Current
        {
          get
          {
            if (n is Node)
            {
              Node nn = (Node)n;
              KeyValuePair<K, V> res = new KeyValuePair<K, V>(nn.k, nn.v);
              n = n._next;
              return res;
            }
            else
            {
              return _emptyKVP;
            }
          }
        }

        #region IDisposable Support
        protected virtual void Dispose(bool disposing) { }

        public void Dispose()
        {
          Dispose(true);
        }
        #endregion
      }
    }
  }
}
