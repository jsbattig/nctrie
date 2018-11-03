using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JSB.Collections.ConcurrentTrie;

namespace UnitTestNCTrie
{
  [TestClass]
  public class UnitTestListMap
  {
    private ListMap<string, int> _listMap;

    public UnitTestListMap()
    {
      _listMap = ListMap<string, int>.map("hello", 1);
    }    

    [TestMethod]
    public void TestListMapCreated()
    {
      Assert.AreNotEqual(_listMap, null);
    }
  }
}
