// Copyright 2024 Bloomberg Finance L.P.
// Distributed under the terms of the MIT license.

namespace BloombergLP.Sable.Cli;

public interface IConsoleLogger
{
    void LogInfo(string message);
    void LogError(string message);
}
