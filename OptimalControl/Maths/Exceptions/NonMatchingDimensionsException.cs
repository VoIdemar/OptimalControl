using System;

namespace OptimalControl.Maths.Exceptions
{    
    public class NonMatchingDimensionsException : Exception
    {
        public NonMatchingDimensionsException()
            :base()    { }
        
        public NonMatchingDimensionsException(string msg)
            :base(msg) { }
        
        public NonMatchingDimensionsException(string msg, Exception cause)
            :base(msg, cause) { }
    }
}
