using System;
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
  class EntrySet<K, V> {
    public IEnumerator<KeyValuePair<K, V>> iterator()
    {
      return ConcurrentTrieDictionary.this.iterator();
    }

  public boolean contains(Object o)
  {
    if (!(o instanceof Map.Entry)) {
      return false;
    }
    Map.Entry<K, V> e = (Map.Entry<K, V>)o;
    K k = e.getKey();
    V v = lookup(k);
    return v != null;
  }

  public boolean remove(Object o)
  {
    if (!(o instanceof Map.Entry)) {
      return false;
    }
    Map.Entry<K, V> e = (Map.Entry<K, V>)o;
    K k = e.getKey();
    return null != ConcurrentTrieDictionary.this.remove(k);
  }

  public int size()
  {
    int size = 0;
    for (Iterator <?> i = iterator(); i.hasNext(); i.next())
    {
      size++;
    }
    return size;
  }

  public void clear()
  {
    ConcurrentTrieDictionary.this.clear();
  }
}
}
