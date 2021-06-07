using System;
using System.Collections.Generic;
using System.Text;

namespace TriviaXamarinApp.Models
{
    public class AmericanQuestion
    {
        public string QText { get; set; }
        public string CorrectAnswer { get; set; }
        public string[] OtherAnswers { get; set; }
        public string CreatorNickName { get; set; }
    }
}
