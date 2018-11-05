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

namespace JSB.Collections.ConcurrentTrie
{
  public class CNode<K, V> : CNodeBase<K, V>
  {

    public int bitmap;
    public BasicNode[] array;
    public Gen gen;

    public CNode(int bitmap, BasicNode[] array, Gen gen)
    {
      this.bitmap = bitmap;
      this.array = array;
      this.gen = gen;
    }

    // this should only be called from within read-only snapshots
    public override int cachedSize(Object ct)
    {
      int currsz = READ_SIZE();
      if (currsz != -1)
        return currsz;
      else
      {
        int sz = computeSize((ConcurrentTrieDictionary<K, V>)ct);
        while (READ_SIZE() == -1)
          CAS_SIZE(-1, sz);
        return READ_SIZE();
      }
    }

    // lends itself towards being parallelizable by choosing
    // a random starting offset in the array
    // => if there are concurrent size computations, they start
    // at different positions, so they are more likely to
    // to be independent
    private int computeSize(ConcurrentTrieDictionary<K, V> ct)
    {
      int i = 0;
      int sz = 0;
      //  int offset = (array.length > 0) ?
      // // util.Random.nextInt(array.length) /* <-- benchmarks show that
      // // this causes observable contention */
      // scala.concurrent.forkjoin.ThreadLocalRandom.current.nextInt (0,
      // array.length)
      // : 0;

      int offset = 0;
      while (i < array.Length)
      {
        int pos = (i + offset) % array.Length;
        BasicNode elem = array[pos];
        if (elem is SNode<K, V>)
                    sz += 1;
                else if (elem is INode<K, V>)
                    sz += ((INode<K, V>)elem).cachedSize(ct);
        i += 1;
      }
      return sz;
    }

    public CNode<K, V> updatedAt(int pos, BasicNode nn, Gen gen)
    {
      int len = array.Length;
      BasicNode[] narr = new BasicNode[len];
      JavaCompat.ArrayCopy(array, 0, narr, 0, len);
      array.CopyTo(narr, 0);      
      narr[pos] = nn;
      return new CNode<K, V>(bitmap, narr, gen);
    }

    public CNode<K, V> removedAt(int pos, int flag, Gen gen)
    {
      BasicNode[] arr = array;
      int len = arr.Length;
      BasicNode[] narr = new BasicNode[len - 1];
      JavaCompat.ArrayCopy(arr, 0, narr, 0, pos);
      JavaCompat.ArrayCopy(arr, pos + 1, narr, pos, len - pos - 1);
      return new CNode<K, V>(bitmap ^ flag, narr, gen);
    }

    public CNode<K, V> insertedAt(int pos, int flag, BasicNode nn, Gen gen)
    {
      int len = array.Length;
      int bmp = bitmap;
      BasicNode[] narr = new BasicNode[len + 1];
      JavaCompat.ArrayCopy(array, 0, narr, 0, pos);
      narr[pos] = nn;
      JavaCompat.ArrayCopy(array, pos, narr, pos + 1, len - pos);
      return new CNode<K, V>(bmp | flag, narr, gen);
    }

    /**
     * Returns a copy of this cnode such that all the i-nodes below it are
     * copied to the specified generation `ngen`.
     */
    public CNode<K, V> renewed(Gen ngen, ConcurrentTrieDictionary<K, V> ct)
    {
      int i = 0;
      BasicNode[] arr = array;
      int len = arr.Length;
      BasicNode[] narr = new BasicNode[len];
      while (i < len)
      {
        BasicNode elem = arr[i];
        if (elem is INode<K, V>) {
          INode < K, V > _in = (INode<K, V>)elem;
          narr[i] = _in.copyToGen(ngen, ct);
        } else if (elem is BasicNode)
                    narr[i] = elem;
        i += 1;
      }
      return new CNode<K, V>(bitmap, narr, ngen);
    }

    private BasicNode resurrect(INode<K, V> inode, Object inodemain)
    {
      if (inodemain is TNode<K, V>) {
        TNode<K, V> tn = (TNode<K, V>)inodemain;
        return tn.copyUntombed();
      } else
                return inode;
    }

    public MainNode<K, V> toContracted(int lev)
    {
      if (array.Length == 1 && lev > 0)
      {
        if (array[0] is SNode<K, V>) {
          SNode<K, V> sn = (SNode<K, V>)array[0];
          return sn.copyTombed();
        } else
                    return this;

      }
      else
        return this;
    }

    // - if the branching factor is 1 for this CNode, and the child
    // is a tombed SNode, returns its tombed version
    // - otherwise, if there is at least one non-null node below,
    // returns the version of this node with at least some null-inodes
    // removed (those existing when the op began)
    // - if there are only null-i-nodes below, returns null
    public MainNode<K, V> toCompressed(ConcurrentTrieDictionary<K, V> ct, int lev, Gen gen)
    {
      int bmp = bitmap;
      int i = 0;
      BasicNode[] arr = array;
      BasicNode[] tmparray = new BasicNode[arr.Length];
      while (i < arr.Length)
      { // construct new bitmap
        BasicNode sub = arr[i];
        if (sub is INode<K, V>) {
          INode < K, V > _in = (INode<K, V>)sub;
          MainNode<K, V> inodemain = _in.gcasRead(ct);
          // assert(inodemain != null);
          tmparray[i] = resurrect(_in, inodemain);
        } else if (sub is SNode<K, V>) {
          tmparray[i] = sub;
        }
        i += 1;
      }

      return new CNode<K, V>(bmp, tmparray, gen).toContracted(lev);
    }

    public override string str(int lev)
    {
      // "CNode %x\n%s".format(bitmap, array.map(_.str(lev +
      // 1)).mkString("\n"));
      return "CNode";
    }

    /*
     * quiescently consistent - don't call concurrently to anything
     * involving a GCAS!!
     */
    // protected Seq<K,V> collectElems() {
    // array flatMap {
    // case sn: SNode[K, V] => Some(sn.kvPair)
    // case in: INode[K, V] => in.mainnode match {
    // case tn: TNode[K, V] => Some(tn.kvPair)
    // case ln: LNode[K, V] => ln.listmap.toList
    // case cn: CNode[K, V] => cn.collectElems
    // }
    // }
    // }

    // protected Seq<String> collectLocalElems() {
    // // array flatMap {
    // // case sn: SNode[K, V] => Some(sn.kvPair._2.toString)
    // // case in: INode[K, V] => Some(in.toString.drop(14) + "(" + in.gen +
    // ")")
    // // }
    // return null;
    // }

    public override string ToString()
    {
      // val elems = collectLocalElems
      // "CNode(sz: %d; %s)".format(elems.size,
      // elems.sorted.mkString(", "))
      return "CNode";
    }

    public static MainNode<K, V> dual(SNode<K, V> x, int xhc, SNode<K, V> y, int yhc, int lev, Gen gen)
    {
      if (lev < 35)
      {
        int xidx = (int)((uint)xhc >> lev) & 0x1f;
        int yidx = (int)((uint)yhc >> lev) & 0x1f;
        int bmp = (1 << xidx) | (1 << yidx);

        if (xidx == yidx)
        {
          INode<K, V> subinode = new INode<K, V>(gen);// (ConcurrentTrieDictionary.inodeupdater)
          subinode.mainnode = dual(x, xhc, y, yhc, lev + 5, gen);
          return new CNode<K, V>(bmp, new BasicNode[] { subinode }, gen);
        }
        else
        {
          if (xidx < yidx)
            return new CNode<K, V>(bmp, new BasicNode[] { x, y }, gen);
          else
            return new CNode<K, V>(bmp, new BasicNode[] { y, x }, gen);
        }
      }
      else
      {
        return new LNode<K, V>(x.k, x.v, y.k, y.v);
      }
    }
  }
}
