using System.Runtime.Serialization;

namespace Store.Domain.Exceptions
{
    public class OrderException : Exception
    {
        public OrderException(string message) : base(message) { }
    }
}