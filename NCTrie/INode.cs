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
using System.Threading;
using ScalaPorts;

namespace JSB.Collections.ConcurrentTrie
{
  class INode<K, V> : INodeBase<K, V>
  {
    static Object KEY_PRESENT = new Object();
    static Object KEY_ABSENT = new Object();

    static INode<K, V> newRootNode()
    {
      Gen gen = new Gen();
      CNode<K, V> cn = new CNode<K, V>(0, new BasicNode[] { }, gen);
      return new INode<K, V>(cn, gen);
    }

    public INode(MainNode<K, V> bn, Gen g) : base(g)
    {
      WRITE(bn);
    }

    public INode(Gen g) : this(null, g)
    {
    }

    private void WRITE(MainNode<K, V> nval)
    {
      mainnode = nval;
    }

    bool CAS(MainNode<K, V> old, MainNode<K, V> n)
    {
      Interlocked.CompareExchange(ref mainnode, n, old);
      return mainnode == n;
    }

    public MainNode<K, V> gcasRead(ConcurrentTrieDictionary<K, V> ct)
    {
      return GCAS_READ(ct);
    }

    MainNode<K, V> GCAS_READ(ConcurrentTrieDictionary<K, V> ct)
    {
      MainNode<K, V> m = /* READ */mainnode;
      MainNode<K, V> prevval = /* READ */m.prev;
      if (prevval == null)
        return m;
      else
        return GCAS_Complete(m, ct);
    }

    private MainNode<K, V> GCAS_Complete(MainNode<K, V> m, ConcurrentTrieDictionary<K, V> ct)
    {
      while (true)
      {
        if (m == null)
          return null;
        else
        {
          // complete the GCAS
          MainNode<K, V> prev = /* READ */m.prev;
          INode<K, V> ctr = ct.readRoot(true);

          if (prev == null)
            return m;

          if (prev is FailedNode<K, V>)
          {
            // try to commit to previous value
            FailedNode<K, V> fn = (FailedNode<K, V>)prev;
            if (CAS(m, fn.prev))
              return fn.prev;
            else
            {
              // Tailrec
              // return GCAS_Complete (/* READ */mainnode, ct);
              m = /* READ */mainnode;
              continue;
            }
          }
          else if (prev is MainNode<K, V>)
          {
            // Assume that you've read the root from the generation
            // G.
            // Assume that the snapshot algorithm is correct.
            // ==> you can only reach nodes in generations <= G.
            // ==> `gen` is <= G.
            // We know that `ctr.gen` is >= G.
            // ==> if `ctr.gen` = `gen` then they are both equal to
            // G.
            // ==> otherwise, we know that either `ctr.gen` > G,
            // `gen` <
            // G,
            // or both
            if ((ctr.gen == gen) && ct.nonReadOnly())
            {
              // try to commit
              if (m.CAS_PREV(prev, null))
                return m;
              else
              {
                // return GCAS_Complete (m, ct);
                // tailrec
                continue;
              }
            }
            else
            {
              // try to abort
              m.CAS_PREV(prev, new FailedNode<K, V>(prev));
              return GCAS_Complete(/* READ */mainnode, ct);
            }
          }
        }
        throw new RuntimeException("Should not happen");
      }
    }

    bool GCAS(MainNode<K, V> old, MainNode<K, V> n, ConcurrentTrieDictionary<K, V> ct)
    {
      n.WRITE_PREV(old);
      if (CAS(old, n))
      {
        GCAS_Complete(n, ct);
        return /* READ */n.prev == null;
      }
      else
        return false;
    }

    private bool equal(K k1, K k2, ConcurrentTrieDictionary<K, V> ct)
    {
      return ct.equality().equiv(k1, k2);
    }

    private INode<K, V> inode(MainNode<K, V> cn)
    {
      INode<K, V> nin = new INode<K, V>(gen);
      nin.WRITE(cn);
      return nin;
    }

    public INode<K, V> copyToGen(Gen ngen, ConcurrentTrieDictionary<K, V> ct)
    {
      INode<K, V> nin = new INode<K, V>(ngen);
      MainNode<K, V> main = GCAS_READ(ct);
      nin.WRITE(main);
      return nin;
    }

    /**
    * Inserts a key value pair, overwriting the old pair if the keys match.
    * 
    * @return true if successful, false otherwise
    */
    bool rec_insert(K k, V v, int hc, int lev, INode<K, V> parent, Gen startgen, ConcurrentTrieDictionary<K, V> ct)
    {
      while (true)
      {
        MainNode<K, V> m = GCAS_READ(ct); // use -Yinline!

        if (m is CNode<K, V>)
        {
          // 1) a multiway node
          CNode<K, V> cn = (CNode<K, V>)m;
          int idx = (int)((uint)hc >> lev) & 0x1f;
          int flag = 1 << idx;
          int bmp = cn.bitmap;
          int mask = flag - 1;
          int pos = JavaCompat.SparseBitcount(bmp & mask);
          if ((bmp & flag) != 0)
          {
            // 1a) insert below
            BasicNode cnAtPos = cn.array[pos];
            if (cnAtPos is INode<K, V>) {
              INode<K, V> _in = (INode<K, V>)cnAtPos;
              if (startgen == _in.gen)
                return _in.rec_insert(k, v, hc, lev + 5, this, startgen, ct);
              else
              {
                if (GCAS(cn, cn.renewed(startgen, ct), ct))
                {
                  // return rec_insert (k, v, hc, lev, parent,
                  // startgen, ct);
                  // tailrec
                  continue;
                }
                else
                  return false;
              }
            } else if (cnAtPos is SNode<K, V>)
            {
              SNode<K, V> sn = (SNode<K, V>)cnAtPos;
              if (sn.hc == hc && equal((K)sn.k, k, ct))
                return GCAS(cn, cn.updatedAt(pos, new SNode<K, V>(k, v, hc), gen), ct);
              else
              {
                CNode<K, V> rn = (cn.gen == gen) ? cn : cn.renewed(gen, ct);
                MainNode<K, V> nn = rn.updatedAt(pos, inode(CNode.dual(sn, sn.hc, new SNode<K, V>(k, v, hc), hc, lev + 5, gen)), gen);
                return GCAS(cn, nn, ct);
              }
            }
          }
          else
          {
            CNode<K, V> rn = (cn.gen == gen) ? cn : cn.renewed(gen, ct);
            MainNode<K, V> ncnode = rn.insertedAt(pos, flag, new SNode<K, V>(k, v, hc), gen);
            return GCAS(cn, ncnode, ct);
          }
        }
        else if (m is TNode<K, V>) {
          clean(parent, ct, lev - 5);
          return false;
        } else if (m is LNode<K, V>) {
          LNode<K, V> ln = (LNode<K, V>)m;
          MainNode<K, V> nn = ln.inserted(k, v);
          return GCAS(ln, nn, ct);
        }
        throw new RuntimeException("Should not happen");
      }
    }

    /**
    * Inserts a new key value pair, given that a specific condition is met.
    * 
    * @param cond
    *            null - don't care if the key was there
    *            KEY_ABSENT - key wasn't there
    *            KEY_PRESENT - key was there 
    *            other value `v` - key must be bound to `v`
    * @return null if unsuccessful, Option[V] otherwise (indicating
    *         previous value bound to the key)
    */
    Option<V> rec_insertif(K k, V v, int hc, Object cond, int lev, INode<K, V> parent, Gen startgen, ConcurrentTrieDictionary<K, V> ct)
    {
      while (true)
      {
        MainNode<K, V> m = GCAS_READ(ct); // use -Yinline!

        if (m is CNode<K, V>)
        {
          // 1) a multiway node
          CNode<K, V> cn = (CNode<K, V>)m;
          int idx = (hc >>> lev) & 0x1f;
          int flag = 1 << idx;
          int bmp = cn.bitmap;
          int mask = flag - 1;
          int pos = JavaCompat.SparseBitcount(bmp & mask);

          if ((bmp & flag) != 0)
          {
            // 1a) insert below
            BasicNode cnAtPos = cn.array[pos];
            if (cnAtPos is INode<K, V>) {
              INode<K, V> _in = (INode<K, V>)cnAtPos;
              if (startgen == _in.gen)
                  return _in.rec_insertif(k, v, hc, cond, lev + 5, this, startgen, ct);
                else {
                if (GCAS(cn, cn.renewed(startgen, ct), ct))
                {
                  // return rec_insertif (k, v, hc, cond, lev,
                  // parent, startgen, ct);
                  // tailrec
                  continue;
                }
                else
                  return null;
              }
            } else if (cnAtPos is SNode<K, V>) {
              SNode<K, V> sn = (SNode<K, V>)cnAtPos;
              if (cond == null)
              {
                if (sn.hc == hc && equal(sn.k, k, ct))
                {
                  if (GCAS(cn, cn.updatedAt(pos, new SNode<K, V>(k, v, hc), gen), ct))
                    return Option<V>.makeOption(sn.v);
                  else
                    return null;
                }
                else
                {
                  CNode<K, V> rn = (cn.gen == gen) ? cn : cn.renewed(gen, ct);
                  MainNode<K, V> nn = rn.updatedAt(pos, inode(CNode.dual(sn, sn.hc, new SNode<K, V>(k, v, hc), hc, lev + 5, gen)), gen);
                  if (GCAS(cn, nn, ct))
                    return Option<V>.makeOption(); // None;
                  else
                    return null;
                }
              }
              else if (cond == INode<K, V>.KEY_ABSENT)
              {
                if (sn.hc == hc && equal(sn.k, k, ct))
                  return Option<V>.makeOption(sn.v);
                else
                {
                  CNode<K, V> rn = (cn.gen == gen) ? cn : cn.renewed(gen, ct);
                  MainNode<K, V> nn = rn.updatedAt(pos, inode(CNode.dual(sn, sn.hc, new SNode<K, V>(k, v, hc), hc, lev + 5, gen)), gen);
                  if (GCAS(cn, nn, ct))
                    return Option<V>.makeOption(); // None
                  else
                    return null;
                }
              }
              else if (cond == INode<K, V>.KEY_PRESENT)
              {
                if (sn.hc == hc && equal(sn.k, k, ct))
                {
                  if (GCAS(cn, cn.updatedAt(pos, new SNode<K, V>(k, v, hc), gen), ct))
                    return Option<V>.makeOption(sn.v);
                  else
                    return null;
                }
                else
                  return Option<V>.makeOption();// None;
              }
              else
              {
                if (sn.hc == hc && equal(sn.k, k, ct) && sn.v.Equals(cond))
                {
                  if (GCAS(cn, cn.updatedAt(pos, new SNode<K, V>(k, v, hc), gen), ct))
                    return Option<V>.makeOption(sn.v);
                  else
                    return null;
                }
                else
                  return Option<V>.makeOption(); // None
              }
            }
          }
          else if (cond == null || cond == INode<K, V>.KEY_ABSENT)
          {
            CNode<K, V> rn = (cn.gen == gen) ? cn : cn.renewed(gen, ct);
            CNode<K, V> ncnode = rn.insertedAt(pos, flag, new SNode<K, V>(k, v, hc), gen);
            if (GCAS(cn, ncnode, ct))
              return Option<V>.makeOption();// None
            else
              return null;
          }
          else if (cond == INode<K, V>.KEY_PRESENT)
          {
            return Option<V>.makeOption();// None;
          }
          else
            return Option<V>.makeOption(); // None
        }
        else if (m is TNode<K, V>) {
          clean(parent, ct, lev - 5);
          return null;
        } else if (m is LNode<K, V>) {
          // 3) an l-node
          LNode<K, V> ln = (LNode<K, V>)m;
          if (cond == null)
          {
            Option<V> optv = ln.get(k);
            if (insertln(ln, k, v, ct))
              return optv;
            else
              return null;
          }
          else if (cond == INode<K, V>.KEY_ABSENT)
          {
            Option<V> t = ln.get(k);
            if (t == null)
            {
              if (insertln(ln, k, v, ct))
                return Option<V>.makeOption();// None
              else
                return null;
            }
            else
              return t;
          }
          else if (cond == INode<K, V>.KEY_PRESENT)
          {
            Option<V> t = ln.get(k);
            if (t != null)
            {
              if (insertln(ln, k, v, ct))
                return t;
              else
                return null;
            }
            else
              return null; // None
          }
          else
          {
            Option<V> t = ln.get(k);
            if (t != null)
            {
              if (((Some<V>)t).get().Equals(cond))
              {
                if (insertln(ln, k, v, ct))
                  return new Some<V>((V)cond);
                else
                  return null;
              }
              else
                return Option<V>.makeOption();
            }
          }
        }
        //                throw new RuntimeException ("Should not happen");
      }
    }

    bool insertln(LNode<K, V> ln, K k, V v, ConcurrentTrieDictionary<K, V> ct)
    {
      LNode<K, V> nn = ln.inserted(k, v);
      return GCAS(ln, nn, ct);
    }

    /**
    * Looks up the value associated with the key.
    * 
    * @return null if no value has been found, RESTART if the operation
    *         wasn't successful, or any other value otherwise
    */
    Object rec_lookup(K k, int hc, int lev, INode<K, V> parent, Gen startgen, ConcurrentTrieDictionary<K, V> ct)
    {
      while (true)
      {
        MainNode<K, V> m = GCAS_READ(ct); // use -Yinline!

        if (m is CNode<K, V>) {
          // 1) a multinode
          CNode<K, V> cn = (CNode<K, V>)m;
          int idx = (int)((uint)hc >> lev) & 0x1f;
          int flag = 1 << idx;
          int bmp = cn.bitmap;
          if ((bmp & flag) == 0)
            return null; // 1a) bitmap shows no binding
          else
          { // 1b) bitmap contains a value - descend
            int pos = ((uint)bmp == 0xffffffff) ? idx : JavaCompat.SparseBitcount(bmp & (flag - 1));
            BasicNode sub = cn.array[pos];
            if (sub is INode<K, V>) {
              INode< K, V > _in = (INode<K, V>)sub;
              if (ct.isReadOnly() || (startgen == ((INodeBase<K, V>)sub).gen))
                return _in.rec_lookup(k, hc, lev + 5, this, startgen, ct);
                else {
                if (GCAS(cn, cn.renewed(startgen, ct), ct))
                {
                  // return rec_lookup (k, hc, lev, parent,
                  // startgen, ct);
                  // Tailrec
                  continue;
                }
                else
                  return RESTART; // used to be throw
                                  // RestartException
              }
            } else if (sub is SNode<K, V>) {
              // 2) singleton node
              SNode<K, V> sn = (SNode<K, V>)sub;
              if (((SNode<K, V>)sub).hc == hc && equal(sn.k, k, ct))
                return sn.v;
              else
                return null;
            }
          }
        } else if (m is TNode<K, V>) {
          // 3) non-live node
          return cleanReadOnly((TNode<K, V>)m, lev, parent, ct, k, hc);
        } else if (m is LNode<K, V>) {
          // 5) an l-node
          Option<V> tmp = ((LNode<K, V>)m).get(k);
          return (tmp is Option<V>) ? tmp : null;
        }
        throw new RuntimeException("Should not happen");
      }
    }

    private Object cleanReadOnly(TNode<K, V> tn, int lev, INode<K, V> parent, ConcurrentTrieDictionary<K, V> ct, K k, int hc)
    {
      if (ct.nonReadOnly())
      {
        clean(parent, ct, lev - 5);
        return RESTART; // used to be throw RestartException
      }
      else
      {
        if (tn.hc == hc && tn.k.Equals(k))
          return tn.v;
        else
          return null;
      }
    }

    /**
    * Removes the key associated with the given value.
    * 
    * @param v
    *            if null, will remove the key irregardless of the value;
    *            otherwise removes only if binding contains that exact key
    *            and value
    * @return null if not successful, an Option[V] indicating the previous
    *         value otherwise
    */
    Option<V> rec_remove(K k, V v, int hc, int lev, INode<K, V> parent, Gen startgen, ConcurrentTrieDictionary<K, V> ct)
    {
      MainNode<K, V> m = GCAS_READ(ct); // use -Yinline!

      if (m is CNode<K, V>) {
        CNode<K, V> cn = (CNode<K, V>)m;
        int idx = (hc >>> lev) & 0x1f;
        int bmp = cn.bitmap;
        int flag = 1 << idx;
        if ((bmp & flag) == 0)
          return Option<V>.makeOption();
        else
        {
          int pos = JavaCompat.SparseBitcount(bmp & (flag - 1));
          BasicNode sub = cn.array[pos];
          Option<V> res = null;
          if (sub is INode<K, V>) {
            INode< K, V > _in = (INode<K, V>)sub;
            if (startgen == _in.gen)
                res = _in.rec_remove(k, v, hc, lev + 5, this, startgen, ct);
              else {
              if (GCAS(cn, cn.renewed(startgen, ct), ct))
                res = rec_remove(k, v, hc, lev, parent, startgen, ct);
              else
                res = null;
            }
          } else if (sub is SNode<K, V>) {
            SNode<K, V> sn = (SNode<K, V>)sub;
            if (sn.hc == hc && equal(sn.k, k, ct) && (v == null || v.Equals(sn.v)))
            {
              MainNode<K, V> ncn = cn.removedAt(pos, flag, gen).toContracted(lev);
              if (GCAS(cn, ncn, ct))
                res = Option<V>.makeOption(sn.v);
              else
                res = null;
            }
            else
              res = Option<V>.makeOption();
          }

          if (res is None<V> || (res == null))
              return res;
            else {
            if (parent != null)
            { // never tomb at root
              MainNode<K, V> n = GCAS_READ(ct);
              if (n is TNode<K, V>)
                  cleanParent(n, parent, ct, hc, lev, startgen);
            }

            return res;
          }
        }
      } else if (m is TNode<K, V>) {
        clean(parent, ct, lev - 5);
        return null;
      } else if (m is LNode<K, V>) {
        LNode<K, V> ln = (LNode<K, V>)m;
        if (v == null)
        {
          Option<V> optv = ln.get(k);
          MainNode<K, V> nn = ln.removed(k, ct);
          if (GCAS(ln, nn, ct))
            return optv;
          else
            return null;
        }
        else
        {
          Option<V> tmp = ln.get(k);
          if (tmp is Some<V>) {
            Some<V> tmp1 = (Some<V>)tmp;
            if (tmp1.Equals(v))
            {
              MainNode<K, V> nn = ln.removed(k, ct);
              if (GCAS(ln, nn, ct))
                return tmp;
              else
                return null;
            }
          }
        }
      }
      throw new RuntimeException("Should not happen");
    }

    void cleanParent(Object nonlive, INode<K, V> parent, ConcurrentTrieDictionary<K, V> ct, int hc, int lev, Gen startgen)
    {
      while (true)
      {
        MainNode<K, V> pm = parent.GCAS_READ(ct);
        if (pm is CNode<K, V>) {
          CNode<K, V> cn = (CNode<K, V>)pm;
          int idx = (int)((uint)hc >> (lev - 5)) & 0x1f;
          int bmp = cn.bitmap;
          int flag = 1 << idx;
          if ((bmp & flag) == 0)
          {
          } // somebody already removed this i-node, we're done
          else
          {
            int pos = JavaCompat.SparseBitcount(bmp & (flag - 1));
            BasicNode sub = cn.array[pos];
            if (sub == this)
            {
              if (nonlive is TNode<K, V>) {
                TNode<K, V> tn = (TNode<K, V>)nonlive;
                MainNode<K, V> ncn = cn.updatedAt(pos, tn.copyUntombed(), gen).toContracted(lev - 5);
                if (!parent.GCAS(cn, ncn, ct))
                  if (ct.readRoot().gen == startgen)
                  {
                    // cleanParent (nonlive, parent, ct, hc,
                    // lev, startgen);
                    // tailrec
                    continue;
                  }
              }
            }
          }
        } else {
          // parent is no longer a cnode, we're done
        }
        break;
      }
    }

    private void clean(INode<K, V> nd, ConcurrentTrieDictionary<K, V> ct, int lev)
    {
      MainNode<K, V> m = nd.GCAS_READ(ct);
      if (m is CNode<K, V>) {
        CNode<K, V> cn = (CNode<K, V>)m;
        nd.GCAS(cn, cn.toCompressed(ct, lev, gen), ct);
      }
    }

    bool isNullInode(ConcurrentTrieDictionary<K, V> ct)
    {
      return GCAS_READ(ct) == null;
    }

    public int cachedSize(ConcurrentTrieDictionary<K, V> ct)
    {
      MainNode<K, V> m = GCAS_READ(ct);
      return m.cachedSize(ct);
    }

    // /* this is a quiescent method! */
    // def str(lev: Int) = "%sINode -> %s".format("  " * lev, mainnode
    // match {
    // case null => "<null>"
    // case tn: TNode[_, _] => "TNode(%s, %s, %d, !)".format(tn.k, tn.v,
    // tn.hc)
    // case cn: CNode[_, _] => cn.str(lev)
    // case ln: LNode[_, _] => ln.str(lev)
    // case x => "<elem: %s>".format(x)
    // })

    public override string str(int lev)
    {
      return "INode";
    }
  }
}
