namespace ScalaPorts
{
  public class Option<V>
  {
    static None<V> none = new None<V>();

    public static Option<V> makeOption(V o)
    {
      if (o != null)
        return new Some<V>(o);
      else
        return none;
    }

    public static Option<V> makeOption()
    {
      return none;
    }

    public virtual bool nonEmpty()
    {
      return false;
    }
  }

  public class None<V> : Option<V> {
  }

  public class Some<V> : Option<V>
  {
    private V value;

    public Some(V v)
    {
      value = v;
    }

    public V get()
    {
      return value;
    }

    public override bool nonEmpty()
    {
      return value != null;
    }
  }  
}
