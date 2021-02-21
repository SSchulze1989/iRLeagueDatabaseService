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
using iRLeagueManager.Enums;
using iRLeagueDatabase.DataTransfer.Members;

namespace iRLeagueDatabase.DataTransfer.Results
{
    [DataContract]
    [KnownType(typeof(ResultInfoDTO))]
    public class ResultRowDataDTO : MappableDTO, IMappableDTO
    {
        [DataMember]
        public long? ResultRowId { get; set; }
        [DataMember]
        public long ResultId { get; set; }
        [DataMember(Name = "SesType")]
        public SimSessionTypeEnum SimSessionType { get; set; }
        [DataMember(Name = "Start")]
        public int StartPosition { get; set; }
        [DataMember(Name = "Fin")]
        public int FinishPosition { get; set; }

        [DataMember]
        public long MemberId { get; set; }
        [DataMember]
        public long? TeamId { get; set; }

        [DataMember(Name = "OldIR")]
        public int OldIRating { get; set; }
        [DataMember(Name = "NewIR")]
        public int NewIRating { get; set; }
        [DataMember(Name = "StartIR")]
        public int SeasonStartIRating { get; set; }
        [DataMember(Name = "Lic")]
        public string License { get; set; }
        [DataMember(Name = "OldSR")]
        public double OldSafetyRating { get; set; }
        [DataMember(Name = "NewSR")]
        public double NewSafetyRating { get; set; }
        [DataMember]
        public int OldCpi { get; set; }
        [DataMember]
        public int NewCpi { get; set; }
        [DataMember]
        public int ClubId { get; set; }
        [DataMember]
        public string ClubName { get; set; }
        [DataMember]
        public string TeamName { get; set; }
        [DataMember(Name = "CarNr")]
        public int CarNumber { get; set; }
        [DataMember]
        public int ClassId { get; set; }
        [DataMember]
        public string Car { get; set; }
        [DataMember]
        public int CarId { get; set; }
        [DataMember]
        public string CarClass { get; set; }
        [DataMember(Name = "ComplL")]
        public int CompletedLaps { get; set; }
        [DataMember(Name = "ComplPct")]
        public double CompletedPct { get; set; }
        [DataMember(Name = "LeadL")]
        public int LeadLaps { get; set; }
        [DataMember(Name = "FastL")]
        public int FastLapNr { get; set; }
        [DataMember(Name = "Incs")]
        public int Incidents { get; set; }
        [DataMember]
        public RaceStatusEnum Status { get; set; }
        [DataMember(Name = "QTime")]
        public TimeSpan QualifyingTime { get; set; }
        [DataMember(Name = "Intv")]
        public TimeSpan Interval { get; set; }
        [DataMember(Name = "AvgTime")]
        public TimeSpan AvgLapTime { get; set; }
        [DataMember(Name = "FlapTime")]
        public TimeSpan FastestLapTime { get; set; }
        [DataMember(Name = "FinChg")]
        public int PositionChange { get; set; }
        [DataMember(Name = "LocId")]
        public string LocationId { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
        [DataMember(Name = "Div")]
        public int Division { get; set; }
        [DataMember(Name = "OldLic")]
        public int OldLicenseLevel { get; set; }
        [DataMember(Name = "NewLic")]
        public int NewLicenseLevel { get; set; }


        public override object MappingId => ResultRowId;

        //public override object[] Keys => new object[] { ResultRowId, ResultId };
        public override object[] Keys => new object[] { ResultRowId };

        public ResultRowDataDTO() { }
    }
}
