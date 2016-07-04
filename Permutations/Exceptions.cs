using System;

namespace Permutations {

    // Used only by *.from_string methods
    public class InvalidInput: Exception
    {
        public InvalidInput() {}
        public InvalidInput(string message) : base(message) {}
        public InvalidInput(string message, Exception inner) : base(message, inner) {}
    }

    public class CycleException: Exception
    {
        public CycleException() {}
        public CycleException(string message) : base(message) {}
        public CycleException(string message, Exception inner) : base(message, inner) {}
    }

    public class PermutationException: Exception
    {
        public PermutationException() {}
        public PermutationException(string message) : base(message) {}
        public PermutationException(string message, Exception inner) : base(message, inner) {}
    }
}
