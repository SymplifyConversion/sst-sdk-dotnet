using System;

public class AllocationException : Exception
{
    public AllocationException() { }

    public AllocationException(string message) : base(message) { }

    public class InvalidInputException : AllocationException
    {
        public InvalidInputException(string message)
            : base(message)
        {
        }
    }

    public class NotFoundException : AllocationException
    {
        public NotFoundException(string message)
            : base(message)
        {
        }
    }

}