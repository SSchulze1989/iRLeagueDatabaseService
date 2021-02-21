// MIT License

// Copyright (c) 2020 Simon Schulze

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using iRLeagueDatabase.DataTransfer.Reviews;

namespace iRLeagueDatabase.DataTransfer.Results
{
    [DataContract]
    public class ScoredResultRowDataDTO : ResultRowDataDTO
    {
        [DataMember(Name = "ScrResRowId")]
        public long? ScoredResultRowId { get; set; }
        [DataMember]
        public long? ScoringId { get; set; }
        [DataMember(Name = "Pts")]
        public int RacePoints { get; set; }
        [DataMember(Name = "Bonus")]
        public int BonusPoints { get; set; }
        [DataMember(Name = "PenPts")]
        public int PenaltyPoints { get; set; }
        [DataMember(Name = "TotPts")]
        public int TotalPoints { get; set; }
        [DataMember(Name = "Pos")]
        public int FinalPosition { get; set; }
        [DataMember(Name = "PosChg")]
        public int FinalPositionChange { get; set; }
        [DataMember(Name = "RevPen")]
        public ReviewPenaltyDTO[] ReviewPenalties { get; set; }

        public override object MappingId => ScoredResultRowId;

        public override object[] Keys => new object[] { ScoredResultRowId };
    }
}
