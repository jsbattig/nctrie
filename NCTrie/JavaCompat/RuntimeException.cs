/**
 * Compatibility class with Java runtime
 * Author: Jose Sebatian Battig <jsbattig@gmail.com> 
 */

using System;

namespace JSB.Collections.ConcurrentTrie
{
  class RuntimeException : Exception
  {
    public RuntimeException(string s) : base(s) { }
  }
}
