// Copyright 2024 Bloomberg Finance L.P.
// Distributed under the terms of the MIT license.

using Scriban;

namespace BloombergLP.Sable.Cli;

public class Migration
{
    public string DatabaseSchemaName { get; }
    public string FilePath { get; }
    public string Id { get; }
    public string Name { get; }
    public string Script { get; }
    public string Timestamp { get; }
    private bool _wrapInTransaction = true;
    private bool _wrapInIdempotenceBlock = true;

    public Migration(string filePath, string databaseSchemaName)
    {
        DatabaseSchemaName = databaseSchemaName;
        FilePath = filePath;
        Id = Path.GetFileNameWithoutExtension(filePath);
        Timestamp = Id.Split("_").First();
        Script = File.ReadAllText(FilePath);
        var scriptReader = new StringReader(Script);
        while (true)
        {
            var line = scriptReader.ReadLine();
            line = line?.Trim();
            if (line == string.Empty)
            {
                continue;
            }
            if (line is null)
            {
                break;
            }
            if (line.StartsWith("--"))
            {
                var isDirective = line
                    .StartsWith(SableCliConstants.DirectivePrefix);
                if (isDirective)
                {
                    switch (line)
                    {
                        case SableCliConstants.NoTransactionWrapperDirective:
                            _wrapInTransaction = false;
                            break;
                        case SableCliConstants.NoIdempotenceWrapperDirective:
                            _wrapInIdempotenceBlock = false;
                            break;
                    }
                }
            }
            else
            {
                break;
            }

        }
    }

    public string GetIdempotentScript()
    {
        if (!_wrapInIdempotenceBlock)
        {
            var template = Template.Parse(Templates.NoIdempotenceBlockTemplate);
            var result = template.Render(new
            {
                DatabaseSchemaName,
                MigrationId = Id,
                Script = Script
            }, member => member.Name);
            return result;
        }

        var idempotentMigrationScriptTemplate = Template.Parse(Templates.IdempotentMigrationScriptTemplate);
        var idempotentScript = idempotentMigrationScriptTemplate.Render(new
        {
            DatabaseSchemaName,
            MigrationId = Id,
            Backfilled = "0",
            Script = Script
        }, member => member.Name);
        return idempotentScript;
    }

    public string GetTransactionalIdempotentScript()
    {
        var idempotentScript = GetIdempotentScript();
        if (!_wrapInTransaction)
        {
            return idempotentScript;
        }
        var template = Template.Parse(Templates.TransactionTemplate);
        var result = template.Render(new
        {
            Script = idempotentScript
        }, member => member.Name);
        return result;
    }

    public string GetIdempotentMigrationRecordInsertionScript(bool backfill = false)
    {
        var template = Template.Parse(Templates.IdempotentMigrationRecordInsertionTemplate);
        var result = template.Render(new
        {
            DatabaseSchemaName,
            MigrationId = Id,
            Backfilled = backfill ? "1" : "0"
        }, member => member.Name);
        return result;
    }
}
