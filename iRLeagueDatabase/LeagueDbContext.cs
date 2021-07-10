using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;

using iRLeagueDatabase.Entities;
using iRLeagueDatabase.Entities.Members;
using iRLeagueDatabase.Entities.Results;
using iRLeagueDatabase.Entities.Reviews;
using iRLeagueDatabase.Entities.Sessions;
using iRLeagueDatabase.Entities.Statistics;

namespace iRLeagueDatabase
{
    [DbConfigurationType(typeof(iRLeagueDatabase.MyContextConfiguration))]
    public class LeagueDbContext : DbContext
    {
        public virtual DbSet<LeagueEntity> Leagues { get; set; }
        public virtual DbSet<SeasonEntity> Seasons { get; set; }
        //public virtual DbSet<LeagueUserEntity> Users { get; set; }
        public virtual DbSet<LeagueMemberEntity> Members { get; set; }
        public virtual DbSet<CustomIncidentEntity> CustomIncidentKinds { get; set; }
        public virtual DbSet<VoteCategoryEntity> CustomVoteCategories { get; set; }
        public virtual DbSet<LeagueStatisticSetEntity> LeagueStatistics { get; set; }

        public string CurrentLeagueName { get; set; }

        public long CurrentLeagueId { get; set; }
        public LeagueEntity CurrentLeague { get; set; }

        private readonly OrphansToHandle OrphansToHandle;

        private static bool AllowMultipleResultSets = true;

        private const string databaseName = "iRLM_LeagueDatabase";

        /// <summary>
        /// Indicates that SaveChanges() operation performed a change while working with the current dbContext
        /// </summary>
        public bool DbChanged { get; private set; } = false;

        public LeagueDbContext() : this("")
        {
            CurrentLeagueName = "";
            CurrentLeagueId = 0;
        }

        public LeagueDbContext(string leagueName, bool createDb = false) : base(GetConnectionString(databaseName))
        {
            if (createDb)
                Database.SetInitializer(new CreateDatabaseIfNotExists<LeagueDbContext>());
            else
                Database.SetInitializer(new MigrateDatabaseToLatestVersion<LeagueDbContext, iRLeagueDatabase.Migrations.Configuration>(useSuppliedContext: true));

            OrphansToHandle = new OrphansToHandle();
            OrphansToHandle.Add<ScheduleEntity, SeasonEntity>(x => x.Season);
            OrphansToHandle.Add<SessionBaseEntity, ScheduleEntity>(x => x.Schedule);
            OrphansToHandle.Add<RaceSessionEntity, ScheduleEntity>(x => x.Schedule);
            OrphansToHandle.Add<ResultEntity, SessionBaseEntity>(x => x.Session);
            OrphansToHandle.Add<ResultEntity, RaceSessionEntity>(x => x.Session as RaceSessionEntity);
            OrphansToHandle.Add<ResultRowEntity, ResultEntity>(x => x.Result);
            OrphansToHandle.Add<ScoredResultEntity, ResultEntity>(x => x.Result);
            OrphansToHandle.Add<ScoredResultEntity, ScoringEntity>(x => x.Scoring);
            OrphansToHandle.Add<ScoredResultRowEntity, ScoredResultEntity>(x => x.ScoredResult);
            OrphansToHandle.Add<ScoredResultRowEntity, ResultRowEntity>(x => x.ResultRow);

            CurrentLeagueName = leagueName;
            LeagueEntity leagueEntity = null;
            try
            {
                leagueEntity = Leagues.SingleOrDefault(x => x.LeagueName == CurrentLeagueName);
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException) { }

            //if (leagueEntity == null)
            //{
            //    throw new ArgumentException("League name not found!", nameof(leagueName));
            //}
            if (leagueEntity != null)
            {
                CurrentLeagueId = leagueEntity.LeagueId;
                CurrentLeague = leagueEntity;
            }
        }

        private static string GetConnectionString(string dbName)
        {
            return $"Data Source={Environment.MachineName}\\IRLEAGUEDB;Initial Catalog={dbName}; Integrated Security = True; " +
                   $"Pooling=False; MultipleActiveResultSets={AllowMultipleResultSets}; Connect Timeout=30;";
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SeasonEntity>()
                .HasMany(r => r.Schedules)
                .WithRequired(r => r.Season);
            //modelBuilder.Entity<SeasonEntity>()
            //    .HasMany(r => r.Results)
            //    .WithRequired(r => r.Season);

            modelBuilder.Entity<IncidentReviewEntity>()
                .HasMany(r => r.InvolvedMembers)
                .WithMany()
                .Map(rm =>
                {
                    rm.MapLeftKey("ReviewRefId");
                    rm.MapRightKey("MemberRefId");
                    rm.ToTable("IncidentReview_InvolvedLeagueMember");
                });
                //.WithMany(r => r.Reviews);
            modelBuilder.Entity<IncidentReviewEntity>()
                .HasRequired(r => r.Session)
                .WithMany(r => r.Reviews);

            modelBuilder.Entity<SessionBaseEntity>()
                .HasOptional(r => r.SessionResult)
                .WithRequired(r => r.Session)
                .WillCascadeOnDelete();
            modelBuilder.Entity<SessionBaseEntity>()
                .HasOptional(r => r.Schedule)
                .WithMany(r => r.Sessions)
                .WillCascadeOnDelete();

            modelBuilder.Entity<ScoringEntity>()
                .HasMany(r => r.Sessions)
                .WithMany(m => m.Scorings)
                .Map(rm =>
                {
                    rm.MapLeftKey("ScoringRefId");
                    rm.MapRightKey("SessionRefId");
                    rm.ToTable("Scoring_Session");
                });
            modelBuilder.Entity<ScoringEntity>()
                .HasRequired(r => r.Season)
                .WithMany(m => m.Scorings);
            //modelBuilder.Entity<ScoringEntity>()
            //    .HasMany(r => r.Results)
            //    .WithMany(m => m.Session.Scorings);
            //modelBuilder.Entity<ScoringEntity>()
            //    .HasMany(r => r.MultiScoringResults)
            //    .WithMany()
            //    .Map(rm =>
            //    {
            //        rm.MapLeftKey("ScoringParentId");
            //        rm.MapRightKey("ScoringChildId");
            //        rm.ToTable("MultiScoringMap");
            //    });
            modelBuilder.Entity<ScoringEntity>()
                .HasOptional(r => r.ConnectedSchedule)
                .WithMany(m => m.ConnectedScorings);
            //modelBuilder.Entity<ScoredResultRowEntity>()
            //    .ToTable("ScoredResultRowEntities");
            modelBuilder.Entity<ScoringTableEntity>()
                .HasMany(r => r.Scorings)
                .WithMany(m => m.ScoringTables)
                .Map(rm =>
                {
                    rm.MapLeftKey("ScoringTableRefId");
                    rm.MapRightKey("ScoringRefId");
                    rm.ToTable("ScoringTableMap");
                });

            modelBuilder.Entity<ScoredResultEntity>()
                .ToTable("ScoredResultEntities");
            modelBuilder.Entity<ScoredResultEntity>()
                .HasMany(r => r.HardChargers)
                .WithMany()
                .Map(rm =>
                {
                    rm.MapLeftKey("ResultRefId", "ScoringRefId");
                    rm.MapRightKey("LeagueMemberRefId");
                    rm.ToTable("ScoredResult_HardChargers");
                });
            modelBuilder.Entity<ScoredResultEntity>()
                .HasMany(r => r.CleanestDrivers)
                .WithMany()
                .Map(rm =>
                {
                    rm.MapLeftKey("ResultRefId", "ScoringRefId");
                    rm.MapRightKey("LeagueMemberRefId");
                    rm.ToTable("ScoredResult_CleanestDrivers");
                });

            modelBuilder.Entity<ScoredTeamResultRowEntity>()
                .HasMany(r => r.ScoredResultRows)
                .WithMany(m => m.ScoredTeamResultRows)
                .Map(rm =>
                {
                    rm.MapLeftKey("ScoredTeamResultRowRefId");
                    rm.MapRightKey("ScoredResultRowRefId");
                    rm.ToTable("ScoredTeamResultRowsGroup");
                });

            modelBuilder.Entity<LeagueStatisticSetEntity>()
                .HasMany(r => r.StatisticSets)
                .WithMany()
                .Map(rm =>
                {
                    rm.MapLeftKey("LeagueStatisticSetRefId");
                    rm.MapRightKey("SeasonStatisticSetRefId");
                    rm.ToTable("LeagueStatisticSet_SeasonStatisticSet");
                });
        }

        public override int SaveChanges()
        {
            //while (!HandleOrphans()) { }
            if (ChangeTracker.HasChanges())
            {
                DbChanged = true;
            }
            HandleOrphans();
            return base.SaveChanges();
        }

        private bool HandleOrphans()
        {
            bool allOrphansHandled = true;

            var objectContext = ((IObjectContextAdapter)this).ObjectContext;

            objectContext.DetectChanges();

            var deletedThings = objectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Deleted).ToList();

            foreach (var deletedThing in deletedThings)
            {
                if (deletedThing.IsRelationship)
                {
                    var entityToDelete = IdentifyEntityToDelete(objectContext, deletedThing);

                    if (entityToDelete != null)
                    {
                        if (!(this.Entry(entityToDelete).State == EntityState.Deleted))
                        {
                            objectContext.DeleteObject(entityToDelete);
                            allOrphansHandled = false;
                        }
                    }
                }
            }

            return allOrphansHandled;
        }

        private object IdentifyEntityToDelete(ObjectContext objectContext, ObjectStateEntry deletedThing)
        {
            // The order is not guaranteed, we have to find which one has to be deleted
            var entityKeyOne = objectContext.GetObjectByKey((EntityKey)deletedThing.OriginalValues[0]);
            var entityKeyTwo = objectContext.GetObjectByKey((EntityKey)deletedThing.OriginalValues[1]);

            foreach (var item in OrphansToHandle.List)
            {
                if (IsInstanceOf(entityKeyOne, item.ChildToDelete) && IsInstanceOf(entityKeyTwo, item.Parent))
                {
                    if (item.NavigationProperty(entityKeyOne) == null)
                        return entityKeyOne;
                }
                if (IsInstanceOf(entityKeyOne, item.Parent) && IsInstanceOf(entityKeyTwo, item.ChildToDelete))
                {
                    if (item.NavigationProperty(entityKeyTwo) == null)
                        return entityKeyTwo;
                }
            }

            return null;
        }

        private bool IsInstanceOf(object obj, Type type)
        {
            // Sometimes it's a plain class, sometimes it's a DynamicProxy, we check for both.
            return
                type == obj.GetType() ||
                (
                    obj.GetType().Namespace == "System.Data.Entity.DynamicProxies" &&
                    type == obj.GetType().BaseType
                );
        }
    }

    public class OrphansToHandle
    {
        public IList<EntityPairDto> List { get; private set; }

        public OrphansToHandle()
        {
            List = new List<EntityPairDto>();
        }

        public void Add<TChildObjectToDelete, TParentObject>(Func<TChildObjectToDelete, TParentObject> navigationProperty)
        {
            List.Add(new EntityPairDto<TChildObjectToDelete, TParentObject>(navigationProperty));
        }
    }

    public class EntityPairDto
    {
        public virtual Type ChildToDelete { get; set; }
        public virtual Type Parent { get; set; }
        public Func<object, object> NavigationProperty { get; set; }
    }

    public class EntityPairDto<TChild, TParent> : EntityPairDto
    {
        public EntityPairDto(Func<TChild, TParent> navigationProperty)
        {
            ChildToDelete = typeof(TChild);
            Parent = typeof(TParent);
            NavigationProperty = new Func<object, object>(x => navigationProperty((TChild)x));
        }
    }
}
