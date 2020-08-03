using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Markup {

    public class Exceptions {

        [Serializable]
        public class InvalidVariableException : Exception {
            public InvalidVariableException() { }
            public InvalidVariableException(string message) : base(message) { }
            public InvalidVariableException(string message, Exception inner) : base(message, inner) { }
            protected InvalidVariableException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }
    }
}
