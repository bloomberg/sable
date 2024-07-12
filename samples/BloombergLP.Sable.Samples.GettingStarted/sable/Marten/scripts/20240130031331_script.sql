-- Generated by Sable on 1/30/2024 3:13:31 AM


BEGIN;


-- Generated by Sable on 1/30/2024 3:00:18 AM
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
    IF NOT EXISTS(SELECT 1 FROM  books.__sable_migrations WHERE migration_id = '20240130030018_InfrastructureSetup') THEN

        RAISE NOTICE 'Inserting record for migration with Id = 20240130030018_InfrastructureSetup';

        INSERT INTO books.__sable_migrations (migration_id, backfilled)
        VALUES ('20240130030018_InfrastructureSetup', '0');
    END IF;
END $$;

COMMIT;



BEGIN;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM  books.__sable_migrations WHERE migration_id = '20240130030021_Initial') THEN

        RAISE NOTICE 'Running migration with Id = 20240130030021_Initial';

        -- Generated by Sable on 1/30/2024 3:00:21 AM
        
        CREATE OR REPLACE FUNCTION books.mt_immutable_timestamp(value text) RETURNS timestamp without time zone LANGUAGE sql IMMUTABLE AS
        $function$
        select value::timestamp
        
        $function$;
        
        
        CREATE OR REPLACE FUNCTION books.mt_immutable_timestamptz(value text) RETURNS timestamp with time zone LANGUAGE sql IMMUTABLE AS
        $function$
        select value::timestamptz
        
        $function$;
        
        
        CREATE OR REPLACE FUNCTION books.mt_grams_vector(text)
                RETURNS tsvector		
                LANGUAGE plpgsql
                IMMUTABLE STRICT
        AS $function$
        BEGIN
                RETURN (SELECT array_to_string(mt_grams_array($1), ' ')::tsvector);
        END
        $function$;
        
        
        CREATE OR REPLACE FUNCTION books.mt_grams_query(text)
                RETURNS tsquery		
                LANGUAGE plpgsql
                IMMUTABLE STRICT
        AS $function$
        BEGIN
                RETURN (SELECT array_to_string(mt_grams_array($1), ' & ')::tsquery);
        END
        $function$;
        
        
        CREATE OR REPLACE FUNCTION books.mt_grams_array(words text)
                RETURNS text[]		
                LANGUAGE plpgsql
                IMMUTABLE STRICT
        AS $function$
                DECLARE result text[];
                DECLARE word text;
                DECLARE clean_word text;
                BEGIN
                        FOREACH word IN ARRAY string_to_array(words, ' ')
                        LOOP
                             clean_word = regexp_replace(word, '[^a-zA-Z0-9]+', '','g');
                             FOR i IN 1 .. length(clean_word)
                             LOOP
                                 result := result || quote_literal(substr(lower(clean_word), i, 1));
                                 result := result || quote_literal(substr(lower(clean_word), i, 2));
                                 result := result || quote_literal(substr(lower(clean_word), i, 3));
                             END LOOP;
                        END LOOP;
        
                        RETURN ARRAY(SELECT DISTINCT e FROM unnest(result) AS a(e) ORDER BY e);
                END;
        $function$;
        
        
        CREATE TABLE IF NOT EXISTS books.mt_doc_book (
            id                  uuid                        NOT NULL,
            data                jsonb                       NOT NULL,
            mt_last_modified    timestamp with time zone    NULL DEFAULT (transaction_timestamp()),
            mt_version          uuid                        NOT NULL DEFAULT (md5(random()::text || clock_timestamp()::text)::uuid),
            mt_dotnet_type      varchar                     NULL,
        CONSTRAINT pkey_mt_doc_book_id PRIMARY KEY (id)
        );
        
        CREATE OR REPLACE FUNCTION books.mt_upsert_book(doc JSONB, docDotNetType varchar, docId uuid, docVersion uuid) RETURNS UUID LANGUAGE plpgsql SECURITY INVOKER AS $function$
        DECLARE
          final_version uuid;
        BEGIN
        INSERT INTO books.mt_doc_book ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp())
          ON CONFLICT ON CONSTRAINT pkey_mt_doc_book_id
          DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp();
        
          SELECT mt_version FROM books.mt_doc_book into final_version WHERE id = docId ;
          RETURN final_version;
        END;
        $function$;
        
        
        CREATE OR REPLACE FUNCTION books.mt_insert_book(doc JSONB, docDotNetType varchar, docId uuid, docVersion uuid) RETURNS UUID LANGUAGE plpgsql SECURITY INVOKER AS $function$
        BEGIN
        INSERT INTO books.mt_doc_book ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp());
        
          RETURN docVersion;
        END;
        $function$;
        
        
        CREATE OR REPLACE FUNCTION books.mt_update_book(doc JSONB, docDotNetType varchar, docId uuid, docVersion uuid) RETURNS UUID LANGUAGE plpgsql SECURITY INVOKER AS $function$
        DECLARE
          final_version uuid;
        BEGIN
          UPDATE books.mt_doc_book SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp() where id = docId;
        
          SELECT mt_version FROM books.mt_doc_book into final_version WHERE id = docId ;
          RETURN final_version;
        END;
        $function$;
        

        
        INSERT INTO books.__sable_migrations (migration_id, backfilled)
        VALUES ('20240130030021_Initial', '0');
    END IF;
END $$;

COMMIT;



-- Generated by Sable on 1/30/2024 3:02:45 AM
    
    
-- MANUALLY EDITED TO BE MADE INDEMPOTENT SINCE THE 'NoIdempotenceWrapper' DIRECTIVE IS SET. THIS IS AN ADVANCED USE CASE
-- ORIGINALLY GENERATED SCRIPT: CREATE INDEX CONCURRENTLY mt_doc_book_idx_name ON books.mt_doc_book USING btree ((data ->> 'Name'));
    
-- Sable NoIdempotenceWrapper
-- Sable NoTransactionWrapper

CREATE INDEX CONCURRENTLY IF NOT EXISTS mt_doc_book_idx_name ON books.mt_doc_book USING btree ((data ->> 'Name'));


DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM  books.__sable_migrations WHERE migration_id = '20240130030245_M1') THEN

        RAISE NOTICE 'Inserting record for migration with Id = 20240130030245_M1';

        INSERT INTO books.__sable_migrations (migration_id, backfilled)
        VALUES ('20240130030245_M1', '0');
    END IF;
END $$;



BEGIN;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM  books.__sable_migrations WHERE migration_id = '20240130031301_M2') THEN

        RAISE NOTICE 'Running migration with Id = 20240130031301_M2';

        -- Generated by Sable on 1/30/2024 3:13:01 AM
        
        CREATE INDEX mt_doc_book_idx_contents ON books.mt_doc_book USING btree ((data ->> 'Contents'));

        
        INSERT INTO books.__sable_migrations (migration_id, backfilled)
        VALUES ('20240130031301_M2', '0');
    END IF;
END $$;

COMMIT;
