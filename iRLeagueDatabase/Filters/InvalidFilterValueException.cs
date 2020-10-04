using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Filters
{
    public class InvalidFilterValueException : Exception
    {
        public InvalidFilterValueException(string message) : base(message)
        {
        }

        public InvalidFilterValueException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
