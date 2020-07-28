﻿using System;
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

namespace iRLeagueDatabase
{
    public class LeagueDbContext : DbContext
    {
        public virtual DbSet<SeasonEntity> Seasons { get; set; }
        public virtual DbSet<LeagueUserEntity> Users { get; set; }
        public virtual DbSet<LeagueMemberEntity> Members { get; set; }

        private readonly OrphansToHandle OrphansToHandle;

        public LeagueDbContext() : this("Data Source=" + Environment.MachineName + "\\IRLEAGUEDB;Initial Catalog=TestDatabase;Integrated Security=True;Pooling=False;")
        {
        }

        public LeagueDbContext(string dbName) : base((dbName != null && dbName != "") ? "Data Source=" + Environment.MachineName + "\\IRLEAGUEDB;Initial Catalog="+dbName+ "; Integrated Security = True; Pooling=False;" : "Data Source=" + Environment.MachineName + "\\IRLEAGUEDB;Initial Catalog=LeagueDatabase;Integrated Security=True;Pooling=False;")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<LeagueDbContext, iRLeagueDatabase.Migrations.Configuration>());
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
                .HasRequired(r => r.Schedule)
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
            modelBuilder.Entity<ScoringEntity>()
                .HasMany(r => r.MultiScoringResults)
                .WithMany()
                .Map(rm =>
                {
                    rm.MapLeftKey("ScoringParentId");
                    rm.MapRightKey("ScoringChildId");
                    rm.ToTable("MultiScoringMap");
                });
            modelBuilder.Entity<ScoringEntity>()
                .HasOptional(r => r.ConnectedSchedule)
                .WithOptionalDependent(r => r.ConnectedScoring);
            //modelBuilder.Entity<ScoredResultRowEntity>()
            //    .ToTable("ScoredResultRowEntities");

            modelBuilder.Entity<LeagueUserEntity>();

            modelBuilder.Entity<ScoredResultEntity>()
                .ToTable("ScoredResultEntities");
        }

        public override int SaveChanges()
        {
            //while (!HandleOrphans()) { }
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
