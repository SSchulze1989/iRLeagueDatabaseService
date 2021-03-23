using iRLeagueDatabase.Entities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Exceptions
{
    public class ResultsCalculationException : LeagueDatabaseException
    {
        public ScoringEntity Scoring { get; }

        public ResultsCalculationException(string message, ScoringEntity scoring, LeagueDbContext dbContext) : this(message, scoring, dbContext, null)
        {
        }

        public ResultsCalculationException(string message, ScoringEntity scoring, LeagueDbContext dbContext, Exception innerException) : base(message, dbContext, innerException)
        {
            Scoring = scoring;
        }
    }
}
