using iRLeagueDatabase.Entities.Results;
using iRLeagueManager.Enums;
using iRLeagueDatabase.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Calculation
{
    public class Accumulator
    {
        public AccumulateResultsOption AccumulateOption { get; set; }
        public IEnumerable<double> AccumulateWeights { get; set; }
        public CultureInfo Culture { get; set; }

        public Accumulator()
        {
            Culture = CultureInfo.InvariantCulture;
        }

        public T Accumulate<T>(IEnumerable<T> values, GetBestOption best = GetBestOption.MaxValue, IEnumerable<double> weights = null) where T : IConvertible
        {
            if (weights == null)
            {
                weights = AccumulateWeights;
            }
            return Accumulate(values, AccumulateOption, best, weights);
        }

        public T Accumulate<T>(IEnumerable<T> values, AccumulateResultsOption option, GetBestOption best = GetBestOption.MaxValue, IEnumerable<double> weights = null) where T : IConvertible
        {
            IEnumerable<double> dValues = values.Select(x => x.ToDouble(Culture));
            double result;

            if (weights == null)
            {
                weights = AccumulateWeights;
            }

            switch (option)
            {
                case AccumulateResultsOption.Sum:
                    result = dValues.Sum();
                    break;
                case AccumulateResultsOption.Average:
                    result = dValues.Sum() / dValues.Count();
                    break;
                case AccumulateResultsOption.Best:
                    switch (best)
                    {
                        case GetBestOption.MaxValue:
                            result = dValues.Max();
                            break;
                        case GetBestOption.MinValue:
                            result = dValues.Min();
                            break;
                        default:
                            result =  default;
                            break;
                    }
                    break;
                case AccumulateResultsOption.WeightedAverage:
                    if (weights == null || weights.Count() != dValues.Count())
                    {
                        throw new AggregateException("Failed to accumulate weighted average. Number of weights and dValues does not match");
                    }
                    result = dValues.Zip(weights, (x, y) => x * y).Sum() / weights.Sum();
                    break;
                case AccumulateResultsOption.Worst:
                    switch (best)
                    {
                        case GetBestOption.MaxValue:
                            result = dValues.Max();
                            break;
                        case GetBestOption.MinValue:
                            result = dValues.Min();
                            break;
                        default:
                            result = default;
                            break;
                    }
                    break;
                default:
                    result = default;
                    break;
            }

            return (T)((IConvertible)result).ToType(typeof(T), Culture);
        }

        public TimeSpan Accumulate(IEnumerable<TimeSpan> values, GetBestOption best, IEnumerable<double> weights = null)
        {
            return Accumulate(values, AccumulateOption, best, weights);
        }

        public TimeSpan Accumulate(IEnumerable<TimeSpan> values, AccumulateResultsOption option, GetBestOption best, IEnumerable<double> weights = null)
        {
            return new TimeSpan(Accumulate(values.Select(x => x.Ticks), option, best, weights));
        }
    }
}
