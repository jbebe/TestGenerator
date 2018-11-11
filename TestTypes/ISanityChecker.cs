using System;
using System.Diagnostics;

namespace TestCommon
{
  public interface ISanityChecker
  {
    void Assert(bool expression, string message = null);
  }

  public class SanityChecker : ISanityChecker
  {
    public void Assert(bool expression, string message = null)
    {
      Debug.Assert(expression, message);
    }
  }
}