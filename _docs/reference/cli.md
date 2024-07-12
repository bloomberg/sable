# Command Line Interface

## `sable init`

Initialize the migration infrastructure for a database.

### Usage

```bash
sable init [OPTIONS]
```

### Options

| Option                                | Description                                                                                                                                                                                              |
|---------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `-p, --project <project-file-path>`   | Path to the project file of the Marten project. Defaults to using the project file in the current directory if it's the only one in there.                                                               |
| `-d, --database <database-name>`      | Which database to use. Defaults to the `Marten` database.                                                                                                                                                |
| `-s, --schema <schema-name>`          | Name of the database schema. Defaults to the `public` schema.                                                                                                                                            |
| `-c, --container-options <options-file-path>` | Path to a JSON file that contains options for building a custom Postgres container that is used as the shadow database for migration management. See [How Sable Works](./how-sable-works) to learn more. |

## `sable migrations add`

Add a new migration for a database.

### Usage

```bash
sable migrations add <migration-name> [OPTIONS]
```

### Options

| Option                                        | Description                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             |
|-----------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `-p, --project <project-file-path>`           | Path to the project file of the Marten project. Defaults to using the project file in the current directory if it's the only one in there.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              |
| `-d, --database <database-name>`              | Which database to use. Defaults to the `Marten` database.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               |
| `-c, --container-options <options-file-path>` | Path to a JSON file that contains options for building a custom Postgres container that is used as the shadow database for migration management. See [How Sable Works](./how-sable-works) to learn more.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                |
| `--no-transaction-wrapper`   | By default, when embedding a migration as part of a larger, aggregate migration script, Sable will wrap it in an anynomous function block to ensure it is executed idemtotently. Additionally, that code block will then be wrapped in a trasaction block to ensure the entire migration is executed in a single atomic operation. However, some Postgres statements must not be executed within a trasaction. For a migration that contains those type of statements, this flag must be set to avoid running into issues when generating migration scripts. This option isreserved for advanced use cases. Do not use it unless you know what you are doing.                                         |  
| `--no-idempotence-wrapper`   | By default, when embedding a migration as part of a larger, aggregate migration script, Sable will wrap it in an anynomous function block to ensure it is executed idemtotently. However, some Postgres statements must not be executed within such a block. For a migration that contains those type of statements, this flag must be set to avoid running into issues when generating migration scripts. Additionally, given that those statements will execute outside of indempotent context, they must be made to be indempotent (e.g., `CREATE INDEX CONCURRENTLY IF NOT EXISTS my_index ON my_table (column_name);` instead of `CREATE INDEX CONCURRENTLY my_index ON my_table (column_name);`). This option isreserved for advanced use cases. Do not use it unless you know what you are doing.|  

## `sable migrations script`

Create an idempotent migration script from existing migrations that can be used to bring a database up to date.

### Usage

```bash
sable migrations script  [OPTIONS]
```

### Options

| Option                              | Description                                                                                                                                |
|-------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------|
| `-p, --project <project-file-path>` | Path to the project file of the Marten project. Defaults to using the project file in the current directory if it's the only one in there. |
| `-d, --database <database-name>`    | Which database to use. Defaults to the `Marten` database.                                                                                  |
| `-f, --from <migration-identifier>` | Id or name of the first migration that should be included in the script. Defaults to the first migration that was generated.               |
| `-t, --to <migration-identifier>`   | Id or name of the last migration that should be included in the script. Defaults to the last migration that was generated.                 |
| `-o, --output <output-file-path>`   | Path of the file to save the script to. Defaults to a path within the `sable` directory tree.                                         |

## `sable migrations backfill`

For an existing database that is already up to date, and for which the migration infrastructure has newly been initialized, backfill the newly created migrations.

### Usage

```bash
sable migrations backfill  [OPTIONS]
```

### Options

| Option                              | Description                                                       |
|-------------------------------------| ----------------------------------------------------------------- |
| `-p, --project <project-file-path>` | Path to the project file of the Marten project. Defaults to using the project file in the current directory if it's the only one in there.               |
| `-d, --database <database-name>`    | Which database to use. Defaults to the `Marten` database.                                         |
| `-o, --output <output-file-path>`   | Path of the file to save the script to. Defaults to a path within the `sable` directory tree.                                         |

## `sable database update`

Use pending migrations to bring a database up to date.

### Usage

```bash
sable database update <connection-string>  [OPTIONS]
```

### Options

| Option                                   | Description                                                                                                                                |
|------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------|
| `-p, --project <project-file-path>`      | Path to the project file of the Marten project. Defaults to using the project file in the current directory if it's the only one in there. |
| `-d, --database <database-name>`         | Which database to use. Defaults to the `Marten` database.                                                                                  |
| `-m, --migration <migration-identifier>` | Id or name of the latest migration that should be applied. Defaults to the last migration that was generated.                              |