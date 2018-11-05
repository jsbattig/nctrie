using System;

namespace JSB.Collections.ConcurrentTrie
{
  class IllegalStateException : Exception
  {
    public IllegalStateException() { }
    public IllegalStateException(string s) : base(s) {}
  }
}
