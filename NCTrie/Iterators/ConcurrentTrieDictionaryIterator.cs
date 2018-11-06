using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSB.Collections.ConcurrentTrie
{
  public class ConcurrentTrieDictionaryIterator<K, V> : IEnumerator<KeyValuePair<K, V>>, ScalaIterator
  {
    private int level;
    protected ConcurrentTrieDictionary<K, V> ct;
    private bool mustInit;
    private BasicNode[][] stack = new BasicNode[7][];
    private int[] stackpos = new int[7];
    private int depth = -1;
    private IEnumerator<KeyValuePair<K, V>> subiter = null;
    private KVNode<K, V> current = null;
    private KeyValuePair<K, V> lastReturned = default(KeyValuePair<K, V>);

    public KeyValuePair<K, V> Current
    {
      get
      {
        return lastReturned;
      }
    }

    object IEnumerator.Current
    {
      get
      {
        return lastReturned;
      }
    }

    public ConcurrentTrieDictionaryIterator(int level, ConcurrentTrieDictionary<K, V> ct, bool mustInit)
    {
      this.level = level;
      this.ct = ct;
      this.mustInit = mustInit;
      if (this.mustInit)
        initialize();
    }

    public ConcurrentTrieDictionaryIterator(int level, ConcurrentTrieDictionary<K, V> ct) : this(level, ct, true) {}

    public bool hasNext()
    {
      return (current != null) || (subiter != null);
    }

    public KeyValuePair<K, V> next()
    {
      if (hasNext())
      {
        KeyValuePair<K, V> r = default(KeyValuePair<K, V>);
        if (subiter != null)
        {
          subiter.MoveNext();
          r = subiter.Current;
          checkSubiter();
        }
        else
        {
          r = current.kvPair();
          advance();
        }

        lastReturned = r;
        return r;
      }
      else
      {
        // return Iterator.empty ().next ();
        return default(KeyValuePair<K, V>);
      }
    }

    KeyValuePair<K, V> nextEntry(KeyValuePair<K, V> rr)
    {
      return new KeyValuePair<K, V>(rr.Key, rr.Value);
    }

    public void Dispose() {}

    public bool MoveNext()
    {
      bool _hasNext = hasNext();
      next();
      return _hasNext;
    }

    public void Reset()
    {
      throw new NotImplementedException();
    }

    private void readin(INode<K, V> _in)
    {
      MainNode<K, V> m = _in.gcasRead(ct);
      if (m is CNode<K, V>) {
        CNode<K, V> cn = (CNode<K, V>)m;
        depth += 1;
        stack[depth] = cn.array;
        stackpos[depth] = -1;
        advance();
      } else if (m is TNode<K, V>) {
        current = (TNode<K, V>)m;
      } else if (m is LNode<K, V>) {
        subiter = ((LNode<K, V>)m).listmap.iterator();
        checkSubiter();
      } else if (m == null)
      {
        current = null;
      }
    }
    
    private void checkSubiter()
    {
      if (!((ScalaIterator)subiter).hasNext())
      {
        subiter = null;
        advance();
      }
    }

    public virtual void initialize()
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
        if (npos < stack[depth].Length)
        {
          stackpos[depth] = npos;
          BasicNode elem = stack[depth][npos];
          if (elem is SNode<K, V>) {
            current = (SNode<K, V>)elem;
          } else if (elem is INode<K, V>) {
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

    protected ConcurrentTrieDictionaryIterator<K, V> newIterator(int _lev, ConcurrentTrieDictionary<K, V> _ct, bool _mustInit)
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
      JavaCompat.ArrayCopy(this.stack, 0, it.stack, 0, 7);
      JavaCompat.ArrayCopy(this.stackpos, 0, it.stackpos, 0, 7);

      // this one needs to be evaluated
      if (this.subiter == null)
        it.subiter = null;
      else
      {
        List<KeyValuePair<K, V>> lst = toList(this.subiter);
        this.subiter = lst.GetEnumerator();
        it.subiter = lst.GetEnumerator();
      }
    }

    private List<KeyValuePair<K, V>> toList(IEnumerator<KeyValuePair<K, V>> it)
    {
      List<KeyValuePair<K, V>> list = new List<KeyValuePair<K, V>>();
      while (it.MoveNext())
      {
        list.Add(it.Current);
      }
      return list;
    }

    void printDebug()
    {
      Console.WriteLine("ctrie iterator");
      Console.WriteLine(stackpos.ToString());
      Console.WriteLine("depth: " + depth);
      Console.WriteLine("curr.: " + current);
      // System.out.println(stack.mkString("\n"));
    }

    public virtual void remove()
    {
      if (!lastReturned.Equals(default(KeyValuePair<K, V>)))
      {
        ct.remove(lastReturned.Key);
        lastReturned = default(KeyValuePair<K, V>);
      }
      else
        throw new IllegalStateException();
    }
  }
}
