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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using ScalaPorts;

namespace JSB.Collections.ConcurrentTrie
{
  public class ConcurrentTrieDictionary<K, V> :  IDictionary<K, V> {
    private static long serialVersionUID = 1L;
    //private static Field READONLY_FIELD;

    /*static {
         Field f;
        try {
            f = ConcurrentTrieDictionary.class.getDeclaredField("readOnly");
        } catch (NoSuchFieldException e) {
            throw new ExceptionInInitializerError(e);
        } catch (SecurityException e) {
            throw new ExceptionInInitializerError(e);
        }
        f.setAccessible(true);
        READONLY_FIELD = f;
    }*/       

    public V this[K key]
    {
      get
      {
        throw new NotImplementedException();
      }

      set
      {
        throw new NotImplementedException();
      }
    }

    public int Count
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public bool IsReadOnly
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public ICollection<K> Keys
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public ICollection<V> Values
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public static ConcurrentTrieDictionary<K, V> empty()
    {
      return new ConcurrentTrieDictionary<K, V>();
    }

    public void Add(KeyValuePair<K, V> item)
    {
      throw new NotImplementedException();
    }

    public void Add(K key, V value)
    {
      throw new NotImplementedException();
    }

    public void Clear()
    {
      throw new NotImplementedException();
    }

    public bool Contains(KeyValuePair<K, V> item)
    {
      throw new NotImplementedException();
    }

    public bool ContainsKey(K key)
    {
      throw new NotImplementedException();
    }

    public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
    {
      throw new NotImplementedException();
    }

    public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
    {
      throw new NotImplementedException();
    }

    public bool Remove(KeyValuePair<K, V> item)
    {
      throw new NotImplementedException();
    }

    public bool Remove(K key)
    {
      throw new NotImplementedException();
    }

    public bool TryGetValue(K key, out V value)
    {
      throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    // static class MangledHashing<K> extends Hashing<K> {
    // int hash(K k) {
    // return util.hashing.byteswap32(k);
    // }
    // }           
    
    private Hashing<K> hashingobj;
    private Equiv<K> equalityobj;

    Hashing<K> hashing()
    {
      return hashingobj;
    }

    public Equiv<K> equality()
    {
      return equalityobj;
    }

    private volatile Object root;
    private bool readOnly;

    ConcurrentTrieDictionary( Hashing<K> hashf,  Equiv<K> ef,  bool readOnly)
    {
      hashingobj = hashf;
      equalityobj = ef;
      this.readOnly = readOnly;
    }

    ConcurrentTrieDictionary( Object r,  Hashing<K> hashf,  Equiv<K> ef, bool readOnly) : this(hashf, ef, readOnly)
    {      
      root = r;
    }

    public ConcurrentTrieDictionary(Hashing<K> hashf,  Equiv<K> ef) : this(INode<K, V>.newRootNode(), hashf, ef, false) {}

    public ConcurrentTrieDictionary() : this(Default<K>.instance, Equiv<K>.universal) {}

    /* internal methods */

    // private void writeObject(java.io.ObjectOutputStream out) {
    // out.writeObject(hashf);
    // out.writeObject(ef);
    //
    // Iterator it = iterator();
    // while (it.hasNext) {
    // val (k, v) = it.next();
    // out.writeObject(k);
    // out.writeObject(v);
    // }
    // out.writeObject(ConcurrentTrieDictionarySerializationEnd);
    // }
    //
    // private ConcurrentTrieDictionary readObject(java.io.ObjectInputStream in) {
    // root = INode.newRootNode();
    // rootupdater = AtomicReferenceFieldUpdater.newUpdater(ConcurrentTrieDictionary.class,
    // Object.class, "root");
    //
    // hashingobj = in.readObject();
    // equalityobj = in.readObject();
    //
    // Object obj = null;
    // do {
    // obj = in.readObject();
    // if (obj != ConcurrentTrieDictionarySerializationEnd) {
    // K k = (K)obj;
    // V = (V)in.readObject();
    // update(k, v);
    // }
    // } while (obj != ConcurrentTrieDictionarySerializationEnd);
    // }

    bool CAS_ROOT(Object ov, Object nv)
    {
      if (isReadOnly())
      {
        throw new IllegalStateException("Attempted to modify a read-only snapshot");
      }
      Interlocked.CompareExchange(ref root, nv, ov);
      return root == nv;
    }

    // FIXME: abort = false by default
    public INode<K, V> readRoot (bool abort)
    {
      return RDCSS_READ_ROOT(abort);
    }

    public INode<K, V> readRoot ()
    {
      return RDCSS_READ_ROOT(false);
    }

    public INode<K, V> RDCSS_READ_ROOT ()
    {
      return RDCSS_READ_ROOT(false);
    }

    INode<K, V> RDCSS_READ_ROOT (bool abort)
    {
      Object r = /* READ */root;
      if (r is INode<K, V>)
        return (INode<K, V>)r;
      else if (r is RDCSS_Descriptor<K, V>) {
        return RDCSS_Complete(abort);
      }
      throw new RuntimeException("Should not happen");
    }

    private INode<K, V> RDCSS_Complete ( bool abort)
    {
      while (true)
      {
        Object v = /* READ */root;
        if (v is INode<K, V>)
          return (INode<K, V>)v;
        else if (v is RDCSS_Descriptor<K, V>) {
          RDCSS_Descriptor<K, V> desc = (RDCSS_Descriptor<K, V>)v;
          INode<K, V> ov = desc.old;
          MainNode<K, V> exp = desc.expectedmain;
          INode<K, V> nv = desc.nv;

          if (abort)
          {
            if (CAS_ROOT(desc, ov))
              return ov;
            else
            {
              // return RDCSS_Complete (abort);
              // tailrec
              continue;
            }
          }
          else
          {
            MainNode<K, V> oldmain = ov.gcasRead(this);
            if (oldmain == exp)
            {
              if (CAS_ROOT(desc, nv))
              {
                desc.committed = true;
                return nv;
              }
              else
              {
                // return RDCSS_Complete (abort);
                // tailrec
                continue;
              }
            }
            else
            {
              if (CAS_ROOT(desc, ov))
                return ov;
              else
              {
                // return RDCSS_Complete (abort);
                // tailrec
                continue;
              }
            }
          }
        }

        throw new RuntimeException("Should not happen");
      }
    }

    private bool RDCSS_ROOT( INode<K, V> ov,  MainNode<K, V> expectedmain,  INode<K, V> nv)
    {
      RDCSS_Descriptor<K, V> desc = new RDCSS_Descriptor<K, V>(ov, expectedmain, nv);
      if (CAS_ROOT(ov, desc))
      {
        RDCSS_Complete(false);
        return /* READ */desc.committed;
      }
      else
        return false;
    }

    private void inserthc( K k,  int hc,  V v)
    {
      while (true)
      {
        INode<K, V> r = RDCSS_READ_ROOT();
        if (!r.rec_insert(k, v, hc, 0, null, r.gen, this))
        {
          // inserthc (k, hc, v);
          // tailrec
          continue;
        }
        break;
      }
    }

    private Option<V> insertifhc( K k,  int hc,  V v,  Object cond)
    {
      while (true)
      {
        INode<K, V> r = RDCSS_READ_ROOT();

        Option<V> ret = r.rec_insertif(k, v, hc, cond, 0, null, r.gen, this);
        if (ret == null)
        {
          // return insertifhc (k, hc, v, cond);
          // tailrec
          continue;
        }
        else
          return ret;
      }
    }

    private Object lookuphc( K k,  int hc)
    {
      while (true)
      {
        INode<K, V> r = RDCSS_READ_ROOT();
        Object res = r.rec_lookup(k, hc, 0, null, r.gen, this);
        if (res == INodeBase<K, V>.RESTART)
        {
          // return lookuphc (k, hc);
          // tailrec
          continue;
        }
        else
          return res;
      }
    }

    private Option<V> removehc( K k,  V v,  int hc)
    {
      while (true)
      {
        INode<K, V> r = RDCSS_READ_ROOT();
        Option<V> res = r.rec_remove(k, v, hc, 0, null, r.gen, this);
        if (res != null)
          return res;
        else
        {
          // return removehc (k, v, hc);
          // tailrec
          continue;
        }
      }
    }

    /**
    * Ensure this instance is read-write, throw UnsupportedOperationException
    * otherwise. Used by Map-type methods for quick check.
    */
    private void ensureReadWrite()
    {
      if (isReadOnly())
      {
        throw new UnsupportedOperationException("Attempted to modify a read-only view");
      }
    }

    public string str () {
        // RDCSS_READ_ROOT().string(0);
        return "Root";
    }

    /* public methods */

    // public Seq<V> seq() {
    // return this;
    // }

    // override def par = new ParConcurrentTrieDictionary(this)

    // static ConcurrentTrieDictionary empty() {
    // return new ConcurrentTrieDictionary();
    // }

    public bool isReadOnly()
    {
      return readOnly;
    }

    public bool nonReadOnly()
    {
      return !readOnly;
    }

    /**
    * Returns a snapshot of this ConcurrentTrieDictionary. This operation is lock-free and
    * linearizable.
    * 
    * The snapshot is lazily updated - the first time some branch in the
    * snapshot or this ConcurrentTrieDictionary are accessed, they are rewritten. This means
    * that the work of rebuilding both the snapshot and this ConcurrentTrieDictionary is
    * distributed across all the threads doing updates or accesses subsequent
    * to the snapshot creation.
    */

    public ConcurrentTrieDictionary<K, V> snapshot()
    {
      while (true)
      {
        INode<K, V> r = RDCSS_READ_ROOT();
        MainNode< K, V > expmain = r.gcasRead(this);
        if (RDCSS_ROOT(r, expmain, r.copyToGen(new Gen(), this)))
          return new ConcurrentTrieDictionary<K, V>(r.copyToGen(new Gen(), this), hashing(), equality(), readOnly);
        else
        {
          // return snapshot ();
          // tailrec
          continue;
        }
      }
    }

    /**
    * Returns a read-only snapshot of this ConcurrentTrieDictionary. This operation is lock-free
    * and linearizable.
    * 
    * The snapshot is lazily updated - the first time some branch of this
    * ConcurrentTrieDictionary are accessed, it is rewritten. The work of creating the snapshot
    * is thus distributed across subsequent updates and accesses on this
    * ConcurrentTrieDictionary by all threads. Note that the snapshot itself is never rewritten
    * unlike when calling the `snapshot` method, but the obtained snapshot
    * cannot be modified.
    * 
    * This method is used by other methods such as `size` and `iterator`.
    */
    public ConcurrentTrieDictionary<K, V> readOnlySnapshot()
    {
      // Is it a snapshot of a read-only snapshot?
      if (!nonReadOnly())
        return this;

      while (true)
      {
        INode<K, V> r = RDCSS_READ_ROOT();
        MainNode<K, V> expmain = r.gcasRead(this);
        if (RDCSS_ROOT(r, expmain, r.copyToGen(new Gen(), this)))
          return new ConcurrentTrieDictionary<K, V>(r, hashing(), equality(), true);
        else
        {
          // return readOnlySnapshot ();
          continue;
        }
      }
    }

    public void clear()
    {
      while (true)
      {
        INode<K, V> r = RDCSS_READ_ROOT();
        if (!RDCSS_ROOT(r, r.gcasRead(this), INode< K, V >.newRootNode()))
        {
          continue;
        }
        else
        {
          return;
        }
      }
    }

    public int computeHash(K k)
    {
      return hashingobj.hash(k);
    }

    V lookup(K k)
    {
      int hc = computeHash(k);
      //        return (V) lookuphc (k, hc);
      Object o = lookuphc(k, hc);
      if (o is Some<V>) {
        return ((Some<V>)o).get();
      } else if (o is None<V>) 
        return default(V);
      else
        return (V)o;
    }

    V apply(K k)
    {
      int hc = computeHash(k);
      Object res = lookuphc(k, hc);
      if (res == null)
        throw new NoSuchElementException();
      else
        return (V)res;
    }

    //     public Option<V> get (K k) {
    //        int hc = computeHash (k);
    //        return Option.makeOption ((V)lookuphc (k, hc));
    //    }

    public V get(Object k)
    {
      return lookup((K)k);
    }

    public Option<V> putOpt(Object key, Object value)
    {
      int hc = computeHash((K)key);
      return insertifhc((K)key, hc, (V)value, null);
    }

    public V put(Object key, Object value)
    {
      ensureReadWrite();
      int hc = computeHash((K)key);
      Option<V> ov = insertifhc((K)key, hc, (V)value, null);
      if (ov is Some<V>) {
        Some<V> sv = (Some<V>)ov;
        return sv.get();
      } else 
        return default(V);
    }

    public void update(K k, V v)
    {
      int hc = computeHash(k);
      inserthc(k, hc, v);
    }

    public ConcurrentTrieDictionary<K, V> add(K k, V v)
    {
      update(k, v);
      return this;
    }

    Option<V> removeOpt (K k)
    {
      int hc = computeHash(k);
      return removehc(k, default(V), hc);
    }

    public V remove(Object k)
    {
      ensureReadWrite();
      int hc = computeHash((K)k);
      Option<V> ov = removehc((K)k, default(V), hc);
      if (ov is Some<V>) {
        Some<V> sv = (Some<V>)ov;
        return sv.get();
      } else 
        return default(V);
    }

    //     public ConcurrentTrieDictionary<K, V> remove (Object k) {
    //        removeOpt ((K)k);
    //        return this;
    //    }

    public Option<V> putIfAbsentOpt(K k, V v)
    {
      int hc = computeHash(k);
      return insertifhc(k, hc, v, INode<K, V>.KEY_ABSENT);
    }

    public V putIfAbsent(Object k, Object v)
    {
      ensureReadWrite();
      int hc = computeHash((K)k);
      Option<V> ov = insertifhc((K)k, hc, (V)v, INode<K, V>.KEY_ABSENT);
      if (ov is Some<V>) {
        Some<V> sv = (Some<V>)ov;
        return sv.get();
      } else 
        return default(V);
    }

    public bool remove(Object k, Object v)
    {
      ensureReadWrite();
      int hc = computeHash((K)k);
      return removehc((K)k, (V)v, hc).nonEmpty();
    }

    public bool replace(K k, V oldvalue, V newvalue)
    {
      ensureReadWrite();
      int hc = computeHash(k);
      return insertifhc(k, hc, newvalue, (Object)oldvalue).nonEmpty();
    }

    public Option<V> replaceOpt(K k, V v)
    {
      int hc = computeHash(k);
      return insertifhc(k, hc, v, INode<K, V>.KEY_PRESENT);
    }

    public V replace(Object k, Object v)
    {
      ensureReadWrite();
      int hc = computeHash((K)k);
      Option<V> ov = insertifhc((K)k, hc, (V)v, INode<K, V>.KEY_PRESENT);
      if (ov is Some<V>) {
        Some<V> sv = (Some<V>)ov;
        return sv.get();
      } else 
        return default(V);
    }

    /***
    * Return an iterator over a ConcurrentTrieDictionary.
    * 
    * If this is a read-only snapshot, it would return a read-only iterator.
    * 
    * If it is the original ConcurrentTrieDictionary or a non-readonly snapshot, it would return
    * an iterator that would allow for updates.
    * 
    * @return
    */
    public IEnumerator<KeyValuePair<K, V>> iterator()
    {
      if (!nonReadOnly())
        return readOnlySnapshot().readOnlyIterator();
      else
        return new ConcurrentTrieDictionaryIterator<K, V>(0, this);
    }

    /***
    * Return an iterator over a ConcurrentTrieDictionary.
    * This is a read-only iterator.
    * 
    * @return
    */
    public IEnumerator<KeyValuePair<K, V>> readOnlyIterator()
    {
      if (nonReadOnly())
        return readOnlySnapshot().readOnlyIterator();
      else
        return new ConcurrentTrieDictionaryReadOnlyIterator<K, V>(0, this);
    }

    private int cachedSize()
    {
      INode<K, V> r = RDCSS_READ_ROOT();
      return r.cachedSize(this);
    }

    public int size()
    {
      if (nonReadOnly())
        return readOnlySnapshot().size();
      else
        return cachedSize();
    }

    string stringPrefix()
    {
      return "ConcurrentTrieDictionary";
    }

    /***
    * This iterator is a read-only one and does not allow for any update
    * operations on the underlying data structure.
    * 
    * @param <K>
    * @param <V>
    */
        
    

    /** Only used for ctrie serialization. */
    // @SerialVersionUID(0L - 7237891413820527142L)
    private static long ConcurrentTrieDictionarySerializationEnd = 0L - 7237891413820527142L;

    public bool containsKey(Object key)
    {
      return lookup((K)key) != null;
    }    
  }

}