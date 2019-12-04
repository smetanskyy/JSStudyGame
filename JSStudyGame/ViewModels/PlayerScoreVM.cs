using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSStudyGame.ViewModels
{
    public class PlayerScoreVM
    {
        public int IdPlayerScore { get; set; }
        public int CorrectAnswers { get; set; }
        public int IncorrectAnswers { get; set; }
        public int SkippedAnswers { get; set; }
        public int TotalScore { get; set; }
        public double TimeGameInSeconds { get; set; }
        public int ProgressInGame { get; set; }
        public int CurrentQuestionNoAnswer { get; set; }
        public string AnswersRight { get; set; }
        public string AnswersWrong { get; set; }
    }
}
