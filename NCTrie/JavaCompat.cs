/**
 * Compatibility routines with Java runtime
 * Author: Jose Sebatian Battig <jsbattig@gmail.com> 
 */

namespace JSB.Collections.ConcurrentTrie
{
  public class JavaCompat
  {
    public static int SparseBitcount(int n)
    {
      int count = 0;
      while (n != 0)
      {
        count++;
        n &= (n - 1);
      }
      return count;
    }

    public static void ArrayCopy(object[] source, int sourceIndex, object[] target, int targetIndex, int length)
    {
      for(var i = sourceIndex; i < sourceIndex + length; i++)      
        target[targetIndex++] = source[i];      
    }
  }
}
