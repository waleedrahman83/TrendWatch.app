using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendWatch.App.DataModels
{
    public class TrendData
    {
      

        public Guid ID { get; set; } = Guid.NewGuid();

        public decimal Senti_Positive { get; set; }
        public decimal Senti_Negative { get; set; }
        public decimal Senti_Neutral { get; set; }
        public string? Senti_Response { get; set; }

        public string TrendKeywords { get; set; }
        public string TrendTopics { get; set; }

        public string AnalysisText { get; set; }
        public string NewsSources { get; set; }
        public string NewsCategories { get; set; }




    }
}
