﻿using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iRLeagueDatabase.Entities.Results;
using iRLeagueManager.Enums;
using iRLeagueDatabase.Entities.Reviews;

namespace iRLeagueDatabase.Entities.Sessions
{
    /// <summary>
    /// Base type for league sessions.
    /// </summary>
    public class SessionBaseEntity : Revision
    {
        //[Key]
        //[ForeignKey(nameof(Schedule))]
        //public int? ScheduleId { get; set; }
        //[Key, ForeignKey(nameof(Schedule)), Column(Order = 2)]
        //public int ScheduleId { get; set; }
        [ForeignKey(nameof(Schedule))]
        public long? ScheduleId { get; set; }
        public virtual ScheduleEntity Schedule { get; set; }

        public override object MappingId => SessionId;

        /// <summary>
        /// Unique Session Id for League Session
        /// </summary>
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SessionId { get; set; }

        public string SessionTitle { get; set; }

        /// <summary>
        /// Type of this session. (Practice, Qualifying or Race)
        /// </summary>
        //[XmlIgnore]
        public SessionType SessionType { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Date of the session.
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Id of the track and track-config for the session.
        /// </summary>
        public string LocationId { get; set; }

        /// <summary>
        /// Duration of the session. In case of a race with attached qualy, this also includes the times of free practice and qualifiying.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Result of the session
        /// </summary>
        public virtual ResultEntity SessionResult { get; set; }

        [InverseProperty(nameof(IncidentReviewEntity.Session))]
        public virtual List<IncidentReviewEntity> Reviews { get; set; }

        //[InverseProperty(nameof(IncidentReview.Session))]
        //public virtual List<IncidentReview> Reviews { get; set; }

        //public XmlTimeSpan Duration { get; set; }C:\Users\simon\Documents\VisualStudio\DatabaseTest\DatabaseTest\Entities\Sessions\SessionBase.cs

        public virtual List<ScoringEntity> Scorings { get; set; }

        [InverseProperty(nameof(SessionBaseEntity.ParentSession))]
        public virtual List<SessionBaseEntity> SubSessions { get; set; }

        [ForeignKey(nameof(ParentSession))]
        public long? ParentSessionId { get; set; }

        public virtual SessionBaseEntity ParentSession { get; set; }

        public int SubSessionNr { get; set; }

        /// <summary>
        /// Create a new Session object
        /// </summary>
        public SessionBaseEntity()
        {
            SessionType = SessionType.Undefined;
            Date = DateTime.Now;
        }

        public override void Delete(LeagueDbContext dbContext)
        {
            SessionResult?.Delete(dbContext);
            Scorings?.ToList().ForEach(x => x.Sessions.Remove(this));
            Reviews?.ToList().ForEach(x => x.Delete(dbContext));
            SubSessions?.ToList().ForEach(x => x.Delete(dbContext));
            base.Delete(dbContext);
        }
    }
}
