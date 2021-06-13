using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRLeagueRESTService.Exceptions
{
    /// <summary>
    /// When this exception is thrown the user does not have sufficient privileges to access to the data set
    /// or the provided league identifier is incorrect
    /// </summary>
    [Serializable]
    public class DataAccessException : Exception
    {
        public long UserLeagueId { get; set; }
        public long DataLeagueId { get; set; }

        public DataAccessException(DataAccessType accessType, DataAccessViolation accessViolation, long userLeagueId, long dataLeagueId) : 
            this(accessType, accessViolation, userLeagueId, dataLeagueId, $"Access to performing \"{accessType}\" action on data source denied. Reason: {GetAccessViolationString(accessViolation, userLeagueId, dataLeagueId)}")
        {
        }

        public DataAccessException(DataAccessType accessType, DataAccessViolation accessViolation, long userLeagueId, long dataLeagueId, string message) :
            this(accessType, accessViolation, userLeagueId, dataLeagueId, message, null)
        {
            
        }

        public DataAccessException(DataAccessType accessType, DataAccessViolation accessViolation, long userLeagueId, long dataLeagueId, string message, Exception innerException) : base(message, innerException)
        {
            UserLeagueId = userLeagueId;
            DataLeagueId = dataLeagueId;
        }

        private static string GetAccessViolationString(DataAccessViolation accessViolation, long userLeagueId = 0, long dataLeagueId = 0)
        {
            switch (accessViolation)
            {
                case DataAccessViolation.MissingPrivileges:
                    return $"{nameof(DataAccessViolation.MissingPrivileges)}";
                case DataAccessViolation.WrongLeague:
                    return $"{nameof(DataAccessViolation.WrongLeague)} - userLeagueId: {userLeagueId} | dataLeagueId: {dataLeagueId}";
                default:
                    return "Unknown Violation";
            }
        }

        public enum DataAccessType
        {
            Read,
            Write,
            Modify,
            Delete
        }

        public enum DataAccessViolation
        {
            Unknown,
            MissingPrivileges,
            WrongLeague
        }
    }
}