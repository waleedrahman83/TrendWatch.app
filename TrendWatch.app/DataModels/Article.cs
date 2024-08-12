using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendWatch.App.DataModels
{
    public class Article : INotifyPropertyChanged
    {
        public Guid ID { get; set; }

        public string Title { get; set; }
        public string Body { get; set; }
        public string Summary { get; set; }
        public string Author { get; set; }
        public Guid NewsSourceID { get; set; }

        public DateTime ArticleDate { get; set; }
        public string ImageURL { get; set; }
        public string ImageDescription { get; set; }
        public int EstimatedReadingSeconds { get; set; }

        public string ArticleURL { get; set; }
        public string Tags { get; set; }

        public string SourceName { get; set; }
        public string SourceLogoURL { get; set; }
        public string ArticleId { get; set; }
        public bool IsFake { get; set; } = false;
        private bool isPlaying = false;
        public string CategoryName { get; set; }

        public bool IsFeatured { get; set; } = false;
        public string IsFakeText { get; set; }

        public bool IsPlaying
    {
            get { return isPlaying; }
            set { isPlaying= value;
                RaisePropertyChanged("IsPlayingColor");
                RaisePropertyChanged("IsPlaying");
            }
        }
        public Color IsPlayingColor
        {
            get {
                if(IsPlaying)
                  return Color.FromHex("#FF6F61");
                else return Color.FromHex("#ffffff");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
