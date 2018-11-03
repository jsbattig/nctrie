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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using JSB.Collections.ConcurrentTrie;
using ScalaPorts;

namespace UnitTestNCTrie
{
  [TestClass]
  public class UnitTestListMap
  {
    private ListMap<string, int> _listMap;

    public UnitTestListMap()
    {
      _listMap = ListMap<string, int>.map("start", 0);
    }    

    [TestMethod]
    public void TestListMapCreated()
    {
      Assert.AreNotEqual(_listMap, null);
    }

    [TestMethod]
    public void TestListMapIsEmpty()
    {
      Assert.IsFalse(_listMap.isEmpty());
      var emptyListMap = new ListMap<string, int>.EmptyListMap();
      Assert.IsTrue(emptyListMap.isEmpty());
      foreach(var kvp in emptyListMap)
      {
        Assert.Fail("Code should never get here");
      }
    }

    [TestMethod]
    public void TestListMapAddAndFind()
    {
      _listMap = _listMap.add("hello", 1);
      _listMap = _listMap.add("hello 2", 2);
      Assert.IsTrue(_listMap.contains("hello"));
      Assert.IsTrue(_listMap.contains("hello 2"));
      Assert.IsFalse(_listMap.contains("hello 3"));
      Assert.IsTrue(_listMap.contains("hello", 1));
      Assert.IsFalse(_listMap.contains("hello", 2));
    }

    [TestMethod]
    public void TestListMapCheckSize()
    {
      Assert.AreEqual(1, _listMap.size());
      _listMap = _listMap.add("hello", 1);
      Assert.AreEqual(2, _listMap.size());
      _listMap = _listMap.add("hello 2", 2);
      Assert.AreEqual(3, _listMap.size());
      _listMap = _listMap.remove("hello");
      Assert.AreEqual(2, _listMap.size());
    }

    [TestMethod]
    public void TestListMapAddAndGet()
    {
      _listMap = _listMap.add("hello", 1);
      _listMap = _listMap.add("hello 2", 2);
      Option<int> v = _listMap.get("hello");
      Assert.IsTrue(v is Some<int>);
      Assert.IsTrue(v.nonEmpty());
      Assert.AreEqual(1, ((Some<int>)v).get());
    }

    [TestMethod]
    public void TestListMapAddAndEnumerate()
    {
      int i = 2;
      _listMap = _listMap.add("hello", 1);
      _listMap = _listMap.add("hello 2", 2);
      foreach(var kvp in _listMap)
      {
        Assert.AreEqual(i--, kvp.Value);
      }
      Assert.AreEqual(-1, i);
    }
  }
}
