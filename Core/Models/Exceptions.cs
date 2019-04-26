using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Models
{
    public class RoleNotFoundError : Exception
    {
        public RoleNotFoundError()
        {
        }

        public RoleNotFoundError(string message) : base(message)
        {
        }

        public RoleNotFoundError(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RoleNotFoundError(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class PlayerCountException : Exception
    {
        public PlayerCountException()
        {
        }

        public PlayerCountException(string message) : base(message)
        {
        }

        public PlayerCountException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PlayerCountException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
