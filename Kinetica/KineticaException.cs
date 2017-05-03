using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kinetica
{
    class KineticaException : System.Exception
    {
        private string message;

        public KineticaException(string msg)
        {
            message = msg;
        }

        public string what() { return message; }

        public override string ToString() { return "KineticaException: " + message; }
    }
}
