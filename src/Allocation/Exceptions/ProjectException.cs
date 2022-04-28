using System;

namespace Allocation.Exceptions
{
    public class ProjectException : Exception
    {
        public ProjectException() { }

        public ProjectException(string message) : base(message) { }

        public class InvalidInputException : ProjectException
        {
            public InvalidInputException(string message)
                : base(message)
            {
            }
        }

        public class NotFoundException : ProjectException
        {
            public NotFoundException(string message)
                : base(message)
            {
            }
        }

    }
}