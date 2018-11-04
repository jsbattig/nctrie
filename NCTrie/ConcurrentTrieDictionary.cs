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

    /**
     * EntrySet
     */
    private EntrySet entrySet = new EntrySet();

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
        
private static class Equiv<K> implements Serializable
{
        private static  long serialVersionUID = 1L;

public boolean equiv(K k1, K k2)
{
  return k1.equals(k2);
}

static  Equiv universal = new Equiv();
    }

    private static interface Hashing<K> extends Serializable
{
        public int hash(K k);
    }

    static class Default<K> implements Hashing<K> {
        private static  long serialVersionUID = 1L;

public int hash(K k)
{
  int h = k.hashCode();
  // This function ensures that hashCodes that differ only by
  // constant multiples at each bit position have a bounded
  // number of collisions (approximately 8 at default load factor).
  h ^= (h >>> 20) ^ (h >>> 12);
  h ^= (h >>> 7) ^ (h >>> 4);
  return h;
}

static  Default instance = new Default();
    }

    private  Hashing<K> hashingobj;
private  Equiv<K> equalityobj;

Hashing<K> hashing()
{
  return hashingobj;
}

Equiv<K> equality()
{
  return equalityobj;
}

private transient volatile Object root;
private  transient boolean readOnly;

    ConcurrentTrieDictionary( Hashing<K> hashf,  Equiv<K> ef,  boolean readOnly)
{
  this.hashingobj = hashf;
  this.equalityobj = ef;
  this.readOnly = readOnly;
}

    ConcurrentTrieDictionary( Object r,  Hashing<K> hashf,  Equiv<K> ef, boolean readOnly)
{
  this(hashf, ef, readOnly);
  this.root = r;
}

public ConcurrentTrieDictionary( Hashing<K> hashf,  Equiv<K> ef)
{
  this(INode.newRootNode(), hashf, ef, false);
}

public ConcurrentTrieDictionary()
{
  this(Default.instance, Equiv.universal);
}

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

 boolean CAS_ROOT(Object ov, Object nv)
{
  if (isReadOnly())
  {
    throw new IllegalStateException("Attempted to modify a read-only snapshot");
  }
  return ROOT_UPDATER.compareAndSet(this, ov, nv);
}

// FIXME: abort = false by default
 INode<K, V> readRoot (boolean abort)
{
  return RDCSS_READ_ROOT(abort);
}

 INode<K, V> readRoot ()
{
  return RDCSS_READ_ROOT(false);
}

 INode<K, V> RDCSS_READ_ROOT ()
{
  return RDCSS_READ_ROOT(false);
}

 INode<K, V> RDCSS_READ_ROOT (boolean abort)
{
  Object r = /* READ */root;
  if (r instanceof INode)
            return (INode<K, V>)r;
        else if (r instanceof RDCSS_Descriptor) {
    return RDCSS_Complete(abort);
  }
  throw new RuntimeException("Should not happen");
}

private  INode<K, V> RDCSS_Complete ( boolean abort)
{
  while (true)
  {
    Object v = /* READ */root;
    if (v instanceof INode)
                return (INode<K, V>)v;
            else if (v instanceof RDCSS_Descriptor) {
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

private boolean RDCSS_ROOT( INode<K, V> ov,  MainNode<K, V> expectedmain,  INode<K, V> nv)
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
    if (res == INodeBase.RESTART)
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

public String string () {
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

     boolean isReadOnly()
{
  return readOnly;
}

 boolean nonReadOnly()
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
    if (!RDCSS_ROOT(r, r.gcasRead(this), INode.< K, V > newRootNode()))
    {
      continue;
    }
    else
    {
      return;
    }
  }
}

// @inline
int computeHash(K k)
{
  return hashingobj.hash(k);
}

 V lookup(K k)
{
  int hc = computeHash(k);
  //        return (V) lookuphc (k, hc);
  Object o = lookuphc(k, hc);
  if (o instanceof Some) {
    return ((Some<V>)o).get();
  } else if (o instanceof None) 
            return null;
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

@Override
 public V put(Object key, Object value)
{
  ensureReadWrite();
  int hc = computeHash((K)key);
  Option<V> ov = insertifhc((K)key, hc, (V)value, null);
  if (ov instanceof Some) {
    Some<V> sv = (Some<V>)ov;
    return sv.get();
  } else 
            return null;
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
  return removehc(k, (V)null, hc);
}

@Override
 public V remove(Object k)
{
  ensureReadWrite();
  int hc = computeHash((K)k);
  Option<V> ov = removehc((K)k, (V)null, hc);
  if (ov instanceof Some) {
    Some<V> sv = (Some<V>)ov;
    return sv.get();
  } else 
            return null;
}

//     public ConcurrentTrieDictionary<K, V> remove (Object k) {
//        removeOpt ((K)k);
//        return this;
//    }

 public Option<V> putIfAbsentOpt(K k, V v)
{
  int hc = computeHash(k);
  return insertifhc(k, hc, v, INode.KEY_ABSENT);
}

@Override
 public V putIfAbsent(Object k, Object v)
{
  ensureReadWrite();
  int hc = computeHash((K)k);
  Option<V> ov = insertifhc((K)k, hc, (V)v, INode.KEY_ABSENT);
  if (ov instanceof Some) {
    Some<V> sv = (Some<V>)ov;
    return sv.get();
  } else 
            return null;
}

@Override
    public boolean remove(Object k, Object v)
{
  ensureReadWrite();
  int hc = computeHash((K)k);
  return removehc((K)k, (V)v, hc).nonEmpty();
}

@Override
    public boolean replace(K k, V oldvalue, V newvalue)
{
  ensureReadWrite();
  int hc = computeHash(k);
  return insertifhc(k, hc, newvalue, (Object)oldvalue).nonEmpty();
}

public Option<V> replaceOpt(K k, V v)
{
  int hc = computeHash(k);
  return insertifhc(k, hc, v, INode.KEY_PRESENT);
}

@Override
    public V replace(Object k, Object v)
{
  ensureReadWrite();
  int hc = computeHash((K)k);
  Option<V> ov = insertifhc((K)k, hc, (V)v, INode.KEY_PRESENT);
  if (ov instanceof Some) {
    Some<V> sv = (Some<V>)ov;
    return sv.get();
  } else 
            return null;
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
public Iterator<Map.Entry<K, V>> iterator()
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
public Iterator<Map.Entry<K, V>> readOnlyIterator()
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

String stringPrefix()
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
private static class ConcurrentTrieDictionaryReadOnlyIterator<K, V> extends ConcurrentTrieDictionaryIterator<K, V> {
  ConcurrentTrieDictionaryReadOnlyIterator (int level,  ConcurrentTrieDictionary<K, V> ct, boolean mustInit) {
    super(level, ct, mustInit);
  }

  ConcurrentTrieDictionaryReadOnlyIterator (int level, ConcurrentTrieDictionary<K, V> ct) {
    this(level, ct, true);
  }        
        void initialize () {
    assert(ct.isReadOnly());
    super.initialize();
  }


        public void remove()
{
  throw new UnsupportedOperationException("Operation not supported for read-only iterators");
}

Map.Entry<K, V> nextEntry( Map.Entry<K, V> rr)
{
  // Return non-updatable entry
  return rr;
}
    }
    
    private static class ConcurrentTrieDictionaryIterator<K, V> implements java.util.Iterator<Map.Entry<K, V>> {
        private int level;
protected ConcurrentTrieDictionary<K, V> ct;
private  boolean mustInit;
        private BasicNode[][] stack = new BasicNode[7][];
private int[] stackpos = new int[7];
private int depth = -1;
private Iterator<Map.Entry<K, V>> subiter = null;
private KVNode<K, V> current = null;
private Map.Entry<K, V> lastReturned = null;

        ConcurrentTrieDictionaryIterator(int level,  ConcurrentTrieDictionary<K, V> ct, boolean mustInit)
{
  this.level = level;
  this.ct = ct;
  this.mustInit = mustInit;
  if (this.mustInit)
    initialize();
}

        ConcurrentTrieDictionaryIterator(int level, ConcurrentTrieDictionary<K, V> ct)
{
  this(level, ct, true);
}


public boolean hasNext()
{
  return (current != null) || (subiter != null);
}

public Map.Entry<K, V> next()
{
  if (hasNext())
  {
    Map.Entry<K, V> r = null;
    if (subiter != null)
    {
      r = subiter.next();
      checkSubiter();
    }
    else
    {
      r = current.kvPair();
      advance();
    }

    lastReturned = r;
    if (r instanceof Map.Entry) {
       Map.Entry<K, V> rr = (Map.Entry<K, V>)r;
      return nextEntry(rr);
    }
    return r;
  }
  else
  {
    // return Iterator.empty ().next ();
    return null;
  }
}

Map.Entry<K, V> nextEntry( Map.Entry<K, V> rr)
{
  return new Map.Entry<K, V>()
  {
                private V updated = null;

@Override
                public K getKey()
{
  return rr.getKey();
}

@Override
                public V getValue()
{
  return (updated == null) ? rr.getValue() : updated;
}

@Override
                public V setValue(V value)
{
  updated = value;
  return ct.replace(getKey(), value);
}
            };            
        }

        private void readin(INode<K, V> in)
{
  MainNode<K, V> m = in.gcasRead(ct);
  if (m instanceof CNode) {
    CNode<K, V> cn = (CNode<K, V>)m;
    depth += 1;
    stack[depth] = cn.array;
    stackpos[depth] = -1;
    advance();
  } else if (m instanceof TNode) {
    current = (TNode<K, V>)m;
  } else if (m instanceof LNode) {
    subiter = ((LNode<K, V>)m).listmap.iterator();
    checkSubiter();
  } else if (m == null)
  {
    current = null;
  }
}

// @inline
private void checkSubiter()
{
  if (!subiter.hasNext())
  {
    subiter = null;
    advance();
  }
}

// @inline
void initialize()
{
  //            assert (ct.isReadOnly ());
  INode<K, V> r = ct.RDCSS_READ_ROOT();
  readin(r);
}

void advance()
{
  if (depth >= 0)
  {
    int npos = stackpos[depth] + 1;
    if (npos < stack[depth].length)
    {
      stackpos[depth] = npos;
      BasicNode elem = stack[depth][npos];
      if (elem instanceof SNode) {
        current = (SNode<K, V>)elem;
      } else if (elem instanceof INode) {
        readin((INode<K, V>)elem);
      }
    }
    else
    {
      depth -= 1;
      advance();
    }
  }
  else
    current = null;
}

protected ConcurrentTrieDictionaryIterator<K, V> newIterator(int _lev, ConcurrentTrieDictionary<K, V> _ct, boolean _mustInit)
{
  return new ConcurrentTrieDictionaryIterator<K, V>(_lev, _ct, _mustInit);
}

protected void dupTo(ConcurrentTrieDictionaryIterator<K, V> it)
{
  it.level = this.level;
  it.ct = this.ct;
  it.depth = this.depth;
  it.current = this.current;

  // these need a deep copy
  System.arraycopy(this.stack, 0, it.stack, 0, 7);
  System.arraycopy(this.stackpos, 0, it.stackpos, 0, 7);

  // this one needs to be evaluated
  if (this.subiter == null)
    it.subiter = null;
  else
  {
    List<Map.Entry<K, V>> lst = toList(this.subiter);
    this.subiter = lst.iterator();
    it.subiter = lst.iterator();
  }
}

// /** Returns a sequence of iterators over subsets of this iterator.
// * It's used to ease the implementation of splitters for a parallel
// version of the ConcurrentTrieDictionary.
// */
// protected def subdivide(): Seq[Iterator[(K, V)]] = if (subiter ne
// null) {
// // the case where an LNode is being iterated
// val it = subiter
// subiter = null
// advance()
// this.level += 1
// Seq(it, this)
// } else if (depth == -1) {
// this.level += 1
// Seq(this)
// } else {
// var d = 0
// while (d <= depth) {
// val rem = stack(d).length - 1 - stackpos(d)
// if (rem > 0) {
// val (arr1, arr2) = stack(d).drop(stackpos(d) + 1).splitAt(rem / 2)
// stack(d) = arr1
// stackpos(d) = -1
// val it = newIterator(level + 1, ct, false)
// it.stack(0) = arr2
// it.stackpos(0) = -1
// it.depth = 0
// it.advance() // <-- fix it
// this.level += 1
// return Seq(this, it)
// }
// d += 1
// }
// this.level += 1
// Seq(this)
// }

private List<Entry<K, V>> toList(Iterator<Entry<K, V>> it)
{
  ArrayList<Entry<K, V>> list = new ArrayList<Map.Entry<K, V>>();
  while (it.hasNext())
  {
    list.add(it.next());
  }
  return list;
}

void printDebug()
{
  System.out.println("ctrie iterator");
  System.out.println(Arrays.toString(stackpos));
  System.out.println("depth: " + depth);
  System.out.println("curr.: " + current);
  // System.out.println(stack.mkString("\n"));
}

@Override
        public void remove()
{
  if (lastReturned != null)
  {
    ct.remove(lastReturned.getKey());
    lastReturned = null;
  }
  else
    throw new IllegalStateException();
}

    }

    /** Only used for ctrie serialization. */
    // @SerialVersionUID(0L - 7237891413820527142L)
    private static long ConcurrentTrieDictionarySerializationEnd = 0L - 7237891413820527142L;


public boolean containsKey(Object key)
{
  return lookup((K)key) != null;
}


@Override
    public Set<Map.Entry<K, V>> entrySet()
{
  return entrySet;
}

/***
 * Support for EntrySet operations required by the Map interface
 *
 */
 class EntrySet extends AbstractSet<Map.Entry<K, V>> {

  @Override
        public Iterator<Map.Entry<K, V>> iterator()
{
  return ConcurrentTrieDictionary.this.iterator();
}

@Override
        public  boolean contains( Object o)
{
  if (!(o instanceof Map.Entry)) {
    return false;
  }
   Map.Entry<K, V> e = (Map.Entry<K, V>)o;
   K k = e.getKey();
   V v = lookup(k);
  return v != null;
}

@Override
        public  boolean remove( Object o)
{
  if (!(o instanceof Map.Entry)) {
    return false;
  }
   Map.Entry<K, V> e = (Map.Entry<K, V>)o;
   K k = e.getKey();
  return null != ConcurrentTrieDictionary.this.remove(k);
}

@Override
        public  int size()
{
  int size = 0;
  for ( Iterator<?> i = iterator(); i.hasNext(); i.next()) {
    size++;
  }
  return size;
}

@Override
        public  void clear()
{
  ConcurrentTrieDictionary.this.clear();
}
    }

    private void readObject(ObjectInputStream inputStream) throws IOException, ClassNotFoundException {
        inputStream.defaultReadObject();
        this.root = INode.newRootNode();

         boolean ro = inputStream.readBoolean();
         int size = inputStream.readInt();
        for (int i = 0; i<size; ++i) {
   K key = (K)inputStream.readObject();
   V value = (V)inputStream.readObject();
  add(key, value);
}

        // Propagate the read-only bit
        try {
  READONLY_FIELD.setBoolean(this, ro);
} catch (IllegalAccessException e) {
  throw new IOException("Failed to set read-only flag", e);
}
}

private void writeObject(ObjectOutputStream outputStream) throws IOException
{
  outputStream.defaultWriteObject();

   Map<K, V> ro = readOnlySnapshot();
  outputStream.writeBoolean(isReadOnly());
  outputStream.writeInt(ro.size());

        for (Entry<K, V> e : ro.entrySet()) {
    outputStream.writeObject(e.getKey());
    outputStream.writeObject(e.getValue());
  }
}
}

}