using LibLite.CheapGet.Client.Console.Services.UI;
using LibLite.CheapGet.Core.CGQL.Services;
using System;
using System.Threading.Tasks;

namespace LibLite.CheapGet.Client.Console.Services.Application
{
    public class Application : IApplication
    {
        private readonly ILexer _lexer;
        private readonly IParser _parser;
        private readonly IInterpreter _interpreter;

        public Application(
            ILexer lexer,
            IParser parser,
            IInterpreter interpreter)
        {
            _lexer = lexer;
            _parser = parser;
            _interpreter = interpreter;
        }

        public async Task StartAsync()
        {
            while (true)
            {
                try
                {
                    System.Console.Write("-> ");
                    var input = System.Console.ReadLine();
                    var tokens = _lexer.Lex(input);
                    var expression = _parser.Parse(tokens);
                    var task = _interpreter.InterpretAsync(expression);
                    await DisplayProgressBarUntilCompletedAsync(task);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
            }
        }

        private static async Task DisplayProgressBarUntilCompletedAsync(Task task)
        {
            using var progressBar = new ProgressBar();
            var progress = 0;
            while (!task.IsCompleted)
            {
                var percentage = (double)progress / 100;
                progressBar.Report(percentage);
                await Task.Delay(20);
                progress++;
                progress %= 100;
            }
        }
    }
}
