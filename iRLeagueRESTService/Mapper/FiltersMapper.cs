using iRLeagueDatabase.DataTransfer.Filters;
using iRLeagueDatabase.DataTransfer.Members;
using iRLeagueDatabase.DataTransfer.Results;
using iRLeagueDatabase.Entities.Filters;
using iRLeagueDatabase.Entities.Members;
using iRLeagueDatabase.Entities.Results;
using iRLeagueDatabase.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Results;

namespace iRLeagueDatabase.Mapper
{
    public partial class DTOMapper
    {
        public void RegisterFiltersTypeMaps()
        {
            RegisterTypeMap<ResultsFilterOptionEntity, ResultsFilterOptionDTO>(MapToResultsFilterOptionDTO);
        }

        public ResultsFilterOptionDTO MapToResultsFilterOptionDTO(ResultsFilterOptionEntity source, ResultsFilterOptionDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new ResultsFilterOptionDTO();

            target.ColumnPropertyName = source.ColumnPropertyName;
            target.Comparator = source.Comparator;

            switch (source.ColumnPropertyName)
            {
                case nameof(ResultRowEntity.MemberId):
                    target.ColumnPropertyName = nameof(ResultRowDataDTO.Member);
                    break;
                case nameof(ResultRowEntity.Member)+"."+nameof(LeagueMemberEntity.Team)+"."+nameof(TeamEntity.Name):
                    target.ColumnPropertyName = nameof(ResultRowDataDTO.TeamName);
                    break;
                default:
                    target.ColumnPropertyName = source.ColumnPropertyName;
                    break;
            }
            var targetColumnProperty = typeof(ResultRowDataDTO).GetNestedPropertyInfo(target.ColumnPropertyName);
            var sourceColumnProperty = typeof(ResultRowEntity).GetNestedPropertyInfo(source.ColumnPropertyName);
            target.FilterValues = source.FilterValues.Split(';').Select(x => ConvertToResultsValueObject(sourceColumnProperty.PropertyType, x, targetColumnProperty.PropertyType)).ToArray();
            target.ResultsFilterId = source.ResultsFilterId;
            target.ResultsFilterType = source.ResultsFilterType;
            target.Exclude = source.Exclude;
            target.ScoringId = source.ScoringId;

            return target;
        }

        private object ConvertToResultsValueObject(Type sourceType, string source, Type targetType)
        {
            if (source == null)
                return null;

            if (sourceType == null)
            {
                throw new ArgumentException("Error converting results value string to object - sourceType was null");
            }
            if (targetType == null)
            {
                throw new ArgumentException("Error converting results value string to object - targetType was null");
            }

            // convert values
            if (string.IsNullOrEmpty(source))
            {
                if (targetType.IsValueType)
                {
                    return Activator.CreateInstance(targetType);
                }
                else
                {
                    return null;
                }
            }
            object sourceObject = Convert.ChangeType(source, sourceType);
            object target;
            if (sourceType.Equals(typeof(long)) && targetType.Equals(typeof(TimeSpan)))
            {
                target = TimeSpanConverter.Convert((long)sourceObject);
            }
            else if (sourceType.Equals(typeof(long)) && targetType.Equals(typeof(LeagueMemberInfoDTO)))
            {
                target = new LeagueMemberInfoDTO() { MemberId = (long)sourceObject };
            }
            else if (sourceType.Equals(targetType))
            {
                target = sourceObject;
            }
            else
            {
                target = Convert.ChangeType(sourceObject, targetType);
            }

            return target;
        }
    }

    public partial class EntityMapper
    {
        public void RegisterFiltersTypeMaps()
        {
            RegisterTypeMap<ResultsFilterOptionDTO, ResultsFilterOptionEntity>(MapToResultsFilterOptionEntity);
        }

        public ResultsFilterOptionEntity MapToResultsFilterOptionEntity(ResultsFilterOptionDTO source, ResultsFilterOptionEntity target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = DefaultGet<ResultsFilterOptionDTO, ResultsFilterOptionEntity>(source);

            switch (source.ColumnPropertyName)
            {
                case nameof(ResultRowDataDTO.Member):
                    target.ColumnPropertyName = nameof(ResultRowEntity.MemberId);
                    break;
                case nameof(ResultRowDataDTO.TeamName):
                    target.ColumnPropertyName = $"{nameof(ResultRowEntity.Member)}.{nameof(LeagueMemberEntity.Team)}.{nameof(TeamEntity.Name)}";
                    break;
                default:
                    target.ColumnPropertyName = source.ColumnPropertyName;
                    break;
            }
            target.Comparator = source.Comparator;
            // get target and source columnproperty
            var targetColumnProperty = typeof(ResultRowEntity).GetNestedPropertyInfo(target.ColumnPropertyName);
            var sourceColumnProperty = typeof(ResultRowDataDTO).GetNestedPropertyInfo(source.ColumnPropertyName);
            target.FilterValues = String.Join(";", source.FilterValues.Select(x => ConvertToResultsValueString(sourceColumnProperty.PropertyType, x, targetColumnProperty.PropertyType)));
            target.ResultsFilterType = source.ResultsFilterType;
            target.Exclude = source.Exclude;
            if (target.Scoring == null && target.ScoringId == 0)
            {
                target.Scoring = DefaultGet<ScoringEntity>(new object[] { source.ScoringId });
            }

            return target;
        }

        private string ConvertToResultsValueString(Type sourceType, object source, Type targetType)
        {
            if (source == null)
                return null;

            if (sourceType == null)
            {
                throw new ArgumentException("Error converting results value to string - sourceType was null");
            }
            if (targetType == null)
            {
                throw new ArgumentException("Error converting results value to string - targetType was null");
            }

            // convert values
            object target;
            if (sourceType.Equals(typeof(TimeSpan)) && targetType.Equals(typeof(long)))
            {
                target = TimeSpanConverter.Convert((TimeSpan)source);
            }
            else if (sourceType.Equals(typeof(LeagueMemberInfoDTO)) && targetType.Equals(typeof(long)))
            {
                target = ((LeagueMemberInfoDTO)source).MemberId;
            }
            else
            {
                target = Convert.ChangeType(source, targetType);
            }

            return Convert.ToString(target);
        }
    }
}