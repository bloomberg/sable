
-- Sable NoIdempotenceWrapper

CREATE SCHEMA IF NOT EXISTS books;

CREATE TABLE IF NOT EXISTS books.__sable_migrations (
    migration_id        character varying(150)      NOT NULL,
    date_applied        timestamp without time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    backfilled          boolean                     NOT NULL DEFAULT false,
CONSTRAINT pkey___sable_migrations_migration_id PRIMARY KEY (migration_id)
);

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM  books.__sable_migrations WHERE migration_id = '20231013223111_InfrastructureSetup') THEN

        RAISE NOTICE 'Inserting record for migration with Id = 20231013223111_InfrastructureSetup';

        INSERT INTO books.__sable_migrations (migration_id, backfilled)
        VALUES ('20231013223111_InfrastructureSetup', '0');
    END IF;
END $$;
