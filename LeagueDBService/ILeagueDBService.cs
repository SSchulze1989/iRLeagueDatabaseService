using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using iRLeagueDatabase.DataTransfer;
using iRLeagueDatabase.DataTransfer.Sessions;
using iRLeagueDatabase.DataTransfer.Members;
using iRLeagueDatabase.DataTransfer.Reviews;
using iRLeagueDatabase.DataTransfer.Results;
using iRLeagueDatabase.DataTransfer.Messages;

namespace LeagueDBService
{
    // HINWEIS: Mit dem Befehl "Umbenennen" im Menü "Umgestalten" können Sie den Schnittstellennamen "IService1" sowohl im Code als auch in der Konfigurationsdatei ändern.
    [ServiceContract,
        ServiceKnownType(typeof(RaceSessionDataDTO)),
        ServiceKnownType(typeof(ReviewCommentDataDTO)),
        ServiceKnownType(typeof(IncidentReviewDataDTO)),
        ServiceKnownType(typeof(LeagueMemberDataDTO)),
        ServiceKnownType(typeof(ResultDataDTO)),
        ServiceKnownType(typeof(RequestMessage)),
        ServiceKnownType(typeof(ScheduleDataDTO)),
        ServiceKnownType(typeof(ScoredResultDataDTO)), 
        ServiceKnownType(typeof(ScoredResultRowDataDTO)), 
        ServiceKnownType(typeof(ScoringDataDTO)),
        ServiceKnownType(typeof(ResultRowDataDTO)),
        ServiceKnownType(typeof(StandingsDataDTO)),
        ServiceKnownType(typeof(StandingsRowDataDTO)),
        ServiceKnownType(typeof(AddPenaltyDTO))]
    public interface ILeagueDBService
    {
        [OperationContract]
        ResponseMessage MessageTest(RequestMessage request);

        [OperationContract]
        POSTItemsResponseMessage DatabasePOST(POSTItemsRequestMessage requestMsg);

        [OperationContract]
        GETItemsResponseMessage DatabaseGET(GETItemsRequestMessage requestMsg);

        [OperationContract]
        PUTItemsResponseMessage DatabasePUT(PUTItemsRequestMessage requestMsg);

        [OperationContract]
        DELItemsResponseMessage DatabaseDEL(DELItemsRequestMessage requestMsg);

        [OperationContract]
        void SetDatabaseName(string databaseName);
        [OperationContract]
        string TestDB();

        [OperationContract]
        string Test(string name);

        //[OperationContract]
        //SeasonDataDTO GetSeason(long seasonId);

        //[OperationContract]
        //List<SeasonDataDTO> GetSeasons(long[] seasonIds = null);

        //[OperationContract]
        //SeasonDataDTO PutSeason(SeasonDataDTO season);

        //[OperationContract]
        //LeagueMemberDataDTO GetMember(long memberId);

        //[OperationContract]
        //List<LeagueMemberDataDTO> GetMembers(long[] memberId = null);

        //[OperationContract]
        //LeagueMemberDataDTO[] UpdateMemberList(LeagueMemberDataDTO[] members);

        //[OperationContract]
        //LeagueMemberDataDTO GetLastMember();

        //[OperationContract]
        //LeagueMemberDataDTO PutMember(LeagueMemberDataDTO member);

        //[OperationContract]
        //IncidentReviewDataDTO GetReview(long reviewId);

        //[OperationContract]
        //IncidentReviewDataDTO PutReview(IncidentReviewDataDTO review);

        //[OperationContract]
        //SessionDataDTO GetSession(long sessionId);

        //[OperationContract]
        //SessionDataDTO PutSession(SessionDataDTO session);

        //[OperationContract]
        //CommentDataDTO GetComment(long commentId);

        //[OperationContract]
        //CommentDataDTO PutComment(ReviewCommentDataDTO comment);

        //[OperationContract]
        //ScheduleDataDTO GetSchedule(long scheduleId);

        //[OperationContract]
        //List<ScheduleDataDTO> GetSchedules(long[] scheduleIds = null);

        //[OperationContract]
        //ScheduleDataDTO PutSchedule(ScheduleDataDTO schedule);

        //[OperationContract]
        //ResultDataDTO GetResult(long resultId);

        //[OperationContract]
        //ResultDataDTO PutResult(ResultDataDTO result);

        //[OperationContract]
        //ScoringDataDTO GetScoring(long scoringId);

        //[OperationContract]
        //ScoringDataDTO PutScoring(ScoringDataDTO scoring);

        //[OperationContract]
        //StandingsRowDTO[] GetSeasonStandings(long seasonId, long? lastSessionId);

        //[OperationContract]
        //StandingsRowDTO[] GetTeamStandings(long seasonId, long? lastSessionId);

        //[OperationContract]
        //ScoredResultDataDTO GetScoredResult(long sessionId, long scoringId);

        [OperationContract]
        void CalculateScoredResults(long sessionId);

        [OperationContract]
        void CleanUpSessions();

        // TODO: Hier Dienstvorgänge hinzufügen
    }

    // Verwenden Sie einen Datenvertrag, wie im folgenden Beispiel dargestellt, um Dienstvorgängen zusammengesetzte Typen hinzuzufügen.
    // Sie können im Projekt XSD-Dateien hinzufügen. Sie können nach dem Erstellen des Projekts dort definierte Datentypen über den Namespace "LeagueDBService.ContractType" direkt verwenden.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
