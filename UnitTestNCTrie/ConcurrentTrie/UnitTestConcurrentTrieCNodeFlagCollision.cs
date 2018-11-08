using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JSB.Collections.ConcurrentTrie;

namespace UnitTestNCTrie.ConcurrentTrie
{
  [TestClass]
  public class UnitTestConcurrentTrieCNodeFlagCollision
  {
    [TestMethod]
    public void TestConcurrentTrieCNodeFlagCollision()
    {
      ConcurrentTrieDictionary< Object, Object > map = new ConcurrentTrieDictionary<Object, Object>();
      int z15169 = 15169;
      int z28336 = 28336;

      TestHelper.assertTrue(null == map.get(z15169));
      TestHelper.assertTrue(null == map.get(z28336));

      map.put(z15169, z15169);
      TestHelper.assertTrue(null != map.get(z15169));
      TestHelper.assertTrue(null == map.get(z28336));

      map.put(z28336, z28336);
      TestHelper.assertTrue(null != map.get(z15169));
      TestHelper.assertTrue(null != map.get(z28336));

      map.remove(z15169);

      TestHelper.assertTrue(null == map.get(z15169));
      TestHelper.assertTrue(null != map.get(z28336));

      map.remove(z28336);

      TestHelper.assertTrue(null == map.get(z15169));
      TestHelper.assertTrue(null == map.get(z28336));
    }
  }
}