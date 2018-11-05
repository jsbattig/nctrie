using System.Collections.Generic;

namespace JSB.Collections.ConcurrentTrie
{
  public class ConcurrentTrieDictionaryReadOnlyIterator<K, V> : ConcurrentTrieDictionaryIterator<K, V> {
    public ConcurrentTrieDictionaryReadOnlyIterator (int level,  ConcurrentTrieDictionary<K, V> ct, bool mustInit) : base(level, ct, mustInit) {}
    public ConcurrentTrieDictionaryReadOnlyIterator(int level, ConcurrentTrieDictionary<K, V> ct) : this(level, ct, true) { }
    public override void initialize () {
      //assert(ct.isReadOnly());
      base.initialize();
    }

    public override void remove()
    {
     throw new UnsupportedOperationException("Operation not supported for read-only iterators");
    }

    KeyValuePair<K, V> nextEntry(KeyValuePair<K, V> rr)
    {
      // Return non-updatable entry
      return rr;
    }
  }
}
