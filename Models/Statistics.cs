using System;
using System.ComponentModel.DataAnnotations;

namespace MoodMapper.Models
{

        public class Statistics
        {
        public User User { get; set; }

        public int Id { get; set; }

            public string Emotion { get; set; }

            public int Count { get; set; }

            public DateTime PeriodStart { get; set; }

            public DateTime PeriodEnd { get; set; }

            public int UserId { get; set; }
        }
    }
