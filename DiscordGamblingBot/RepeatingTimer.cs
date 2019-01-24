using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DiscordGamblingBot
{
    internal static class RepeatingTimer
    {
        private static Timer LoopingTimer;
        private static SocketTextChannel channel;
        internal static Task StartTimer()
        {
            channel = Global.Client.GetGuild(503998446584201236).GetTextChannel(535381579992793089);
            LoopingTimer = new Timer()
            {
                Interval = 15 * 1000,
                AutoReset = true,
                Enabled = true
            };
            LoopingTimer.Elapsed += OnTimerTicked;

            return Task.CompletedTask;
        }

        private static async void OnTimerTicked(object sender, ElapsedEventArgs e)
        {
            //This is what the bot sends to the channel every Tick.
            string MathProblem = RenderMathProblem();
            string MathString = $"Answer the math problem |`{MathProblem}`| within 60 seconds to get 50 credits.";
            int answer = MathProblemSolver();
            await channel.SendMessageAsync(MathString);
            await channel.SendMessageAsync($"{answer}");
        }

        public static MathProblem MathProblemRandomizer()
        { 
            Random r = new Random();
            MathProblem mathProblem = new MathProblem();
            char[] mathSign = new char[] { '+', '-', '*', '/' };
            char MathSign = mathSign[r.Next(0, 4)];
            mathProblem.Operator = MathSign;

            if (MathSign == '+' || MathSign == '-')
            {
                mathProblem.Number1 = r.Next(1, 10000);
                mathProblem.Number2 = r.Next(1, 10000);
                return mathProblem;
            }
            else if (MathSign == '/')
            {
                mathProblem.Number2 = r.Next(1, 11);
                mathProblem.Number1 = r.Next(1, 101) * mathProblem.Number2;
                return mathProblem;
            }
            else
            {
                mathProblem.Number1 = r.Next(1, 1001);
                mathProblem.Number2 = r.Next(1, 11);
                return mathProblem;
            }
        }

        public static int MathProblemSolver()
        {
            MathProblem mathProblem = MathProblemRandomizer();
            if (mathProblem.Operator == '+')
            {
                var answer = mathProblem.Number1 + mathProblem.Number2;
                return answer;
            }
            else if (mathProblem.Operator == '-')
            {
                var answer = mathProblem.Number1 - mathProblem.Number2;
                return answer;
            }
            else if (mathProblem.Operator == '*')
            {
                var answer = mathProblem.Number1 * mathProblem.Number2;
                return answer;
            }
            else
            {
                var answer = mathProblem.Number1 / mathProblem.Number2;
                return answer;
            }
        }

        public static string RenderMathProblem()
        {
            MathProblem Problem = MathProblemRandomizer();
            string MathProblem = $"{Problem.Number1}  {Problem.Operator}  {Problem.Number2}";
            return MathProblem;
        }
    }
}