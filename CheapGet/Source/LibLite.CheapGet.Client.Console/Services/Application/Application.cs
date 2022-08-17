﻿using LibLite.CheapGet.Business.Services.DSL;
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
                    await _interpreter.InterpretAsync(expression);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
            }
        }
    }
}