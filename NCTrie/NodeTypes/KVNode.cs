using System.Collections.Generic;

namespace JSB.Collections.ConcurrentTrie
{
  internal interface KVNode<K, V>
  {
    KeyValuePair<K, V> kvPair();
  }
}
