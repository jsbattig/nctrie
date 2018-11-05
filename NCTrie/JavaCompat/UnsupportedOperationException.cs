/**
 * Compatibility class with Java runtime
 * Author: Jose Sebatian Battig <jsbattig@gmail.com> 
 */

using System;

namespace JSB.Collections.ConcurrentTrie
{
  class UnsupportedOperationException : Exception
  {
    public UnsupportedOperationException() { }
    public UnsupportedOperationException(string msg) : base(msg) { }
  }
}
