using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRLeagueManager.Enums;

namespace iRLeagueDatabase.Entities.Members
{
    public interface IClientUser
    {
        string UserName { get; }
        bool Authorize(AdminRights rights);
    }
}
