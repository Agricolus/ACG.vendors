using System;

namespace FIWARE.ContextBroker.Serializer.Exceptions
{
  public class DeserializationException : Exception
  {
    public DeserializationException()
    {
    }

    public DeserializationException(string message) : base(message)
    {
    }

    public DeserializationException(string message, Exception inner) : base(message, inner)
    {
    }
  }
}