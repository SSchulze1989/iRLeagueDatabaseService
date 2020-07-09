using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRLeagueDatabase.Entities.Members;

namespace iRLeagueDatabase.User
{
    public interface ICustomAuthenticationResult
    {
        bool IsAuthenticated { get; }
        IClientUser AuthenticatedUser { get; }
    }
}
