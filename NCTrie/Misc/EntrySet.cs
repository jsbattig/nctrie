using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSB.Collections.ConcurrentTrie
{
  /***
   * Support for EntrySet operations required by the Map interface
   *
   */
  public class EntrySet<K, V> : IEnumerable<KeyValuePair<K, V>> {
    private ConcurrentTrieDictionary<K, V> t;

    public EntrySet(ConcurrentTrieDictionary<K, V> t) {
      this.t = t;
    }

    public IEnumerator<KeyValuePair<K, V>> iterator()
    {
      return t.iterator();
    }

    public bool contains(Object o)
    {
      if (!(o is KeyValuePair<K, V>)) {
        return false;
      }
      KeyValuePair<K, V> e = (KeyValuePair<K, V>)o;
      K k = e.Key;
      V v = t.lookup(k);
      return v != null;
    }

    public bool remove(Object o)
    {
      if (!(o is KeyValuePair<K, V>)) {
        return false;
      }
      KeyValuePair<K, V> e = (KeyValuePair<K, V>)o;
      K k = e.Key;
      return null != t.remove(k);
    }

    public int size()
    {
      int size = 0;
      for (IEnumerator<KeyValuePair<K, V>> i = iterator(); true;)
      {
        if (!i.MoveNext())
          break;
        size++;
      }
      return size;
    }

    public void clear()
    {
      t.clear();
    }

    public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
    {
      return ((IEnumerable<KeyValuePair<K, V>>)t).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable<KeyValuePair<K, V>>)t).GetEnumerator();
    }
  }
}
