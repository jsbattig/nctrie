using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JSB.Collections.ConcurrentTrie;

namespace UnitTestNCTrie.ConcurrentTrie
{
  [TestClass]
  public class UnitTestConcurrentTrieCNodeInsertionIncorrectOrder
  {
    [TestMethod]
    public void TestConcurrentTrieCNodeInsertionIncorrectOrder()
    {
      ConcurrentTrieDictionary< Object, Object > map = new ConcurrentTrieDictionary<Object, Object>();
      int z3884 = 3884;
      int z4266 = 4266;
      map.put(z3884, z3884);
      TestHelper.assertTrue(null != map.get(z3884));

      map.put(z4266, z4266);
      TestHelper.assertTrue(null != map.get(z3884));
      TestHelper.assertTrue(null != map.get(z4266));
    }
  }
}
