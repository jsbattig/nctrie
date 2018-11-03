namespace JSB.Collections.ConcurrentTrie
{
  abstract class INodeBase<K, V> : BasicNode
  {  
    public static object RESTART = new object();

    public volatile MainNode<K, V> mainnode = null;

    public Gen gen;
    
    public INodeBase(Gen generation)
    {
      gen = generation;
    }

    public BasicNode prev()
    {      
      return null;
    }
  }
}
