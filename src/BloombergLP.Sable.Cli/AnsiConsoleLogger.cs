// Copyright 2024 Bloomberg Finance L.P.
// Distributed under the terms of the MIT license.

using Spectre.Console;

namespace BloombergLP.Sable.Cli;

public class AnsiConsoleLogger : IConsoleLogger
{
    private readonly IAnsiConsole _ansiConsole;

    public AnsiConsoleLogger(IAnsiConsole ansiConsole)
    {
        _ansiConsole = ansiConsole ?? throw new ArgumentNullException(nameof(ansiConsole));
    }

    public void LogInfo(string message)
    {
        _ansiConsole.MarkupLine($"[bold mediumpurple3_1]INFO: {message.EscapeMarkup()}[/]");
    }

    public void LogError(string message)
    {
        _ansiConsole.MarkupLine($"[bold red3_1]ERROR: {message.EscapeMarkup()}[/]");
    }
}
