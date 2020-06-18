using System;

namespace MiFloraGateway.Database
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException()
        {
        }

        public EntityNotFoundException(Type type, int id) : base(string.Format("No entity of {0} was found with the id {1}", type, id))
        {
        }

        public EntityNotFoundException(string? message) : base(message)
        {
        }

        public EntityNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }

    public class EntityNotFoundException<T> : EntityNotFoundException
    {
        public EntityNotFoundException()
        {
        }

        public EntityNotFoundException(int id) : base(typeof(T), id)
        {
        }
    }
}
