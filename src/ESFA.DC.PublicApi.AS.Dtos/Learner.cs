using System;
using System.Collections.Generic;

namespace ESFA.DC.PublicApi.AS.Dtos
{
    public class Learner
    {
        public int Ukprn { get; set; }
        public string LearnRefNumber { get; set; }
        public long Uln { get; set; }
        public string FamilyName { get; set; }
        public string GivenNames { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string NINumber { get; set; }
       
        public IReadOnlyList<LearningDelivery> LearningDeliveries { get; set; }
    }
}
