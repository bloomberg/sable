CREATE OR REPLACE FUNCTION orders.mt_immutable_timestamp(value text) RETURNS timestamp without time zone LANGUAGE sql IMMUTABLE AS
$function$
select value::timestamp

$function$;


CREATE OR REPLACE FUNCTION orders.mt_immutable_timestamptz(value text) RETURNS timestamp with time zone LANGUAGE sql IMMUTABLE AS
$function$
select value::timestamptz

$function$;


CREATE OR REPLACE FUNCTION orders.mt_grams_vector(text)
        RETURNS tsvector		
        LANGUAGE plpgsql
        IMMUTABLE STRICT
AS $function$
BEGIN
        RETURN (SELECT array_to_string(mt_grams_array($1), ' ')::tsvector);
END
$function$;


CREATE OR REPLACE FUNCTION orders.mt_grams_query(text)
        RETURNS tsquery		
        LANGUAGE plpgsql
        IMMUTABLE STRICT
AS $function$
BEGIN
        RETURN (SELECT array_to_string(mt_grams_array($1), ' & ')::tsquery);
END
$function$;


CREATE OR REPLACE FUNCTION orders.mt_grams_array(words text)
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


CREATE TABLE IF NOT EXISTS orders.mt_doc_order (
    id                  uuid                        NOT NULL,
    data                jsonb                       NOT NULL,
    mt_last_modified    timestamp with time zone    NULL DEFAULT (transaction_timestamp()),
    mt_version          uuid                        NOT NULL DEFAULT (md5(random()::text || clock_timestamp()::text)::uuid),
    mt_dotnet_type      varchar                     NULL,
CONSTRAINT pkey_mt_doc_order_id PRIMARY KEY (id)
);

CREATE INDEX mt_doc_order_idx_customer_id ON orders.mt_doc_order USING btree ((CAST(data ->> 'CustomerId' as uuid)));

CREATE OR REPLACE FUNCTION orders.mt_upsert_order(doc JSONB, docDotNetType varchar, docId uuid, docVersion uuid) RETURNS UUID LANGUAGE plpgsql SECURITY INVOKER AS $function$
DECLARE
  final_version uuid;
BEGIN
INSERT INTO orders.mt_doc_order ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp())
  ON CONFLICT ON CONSTRAINT pkey_mt_doc_order_id
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp();

  SELECT mt_version FROM orders.mt_doc_order into final_version WHERE id = docId ;
  RETURN final_version;
END;
$function$;


CREATE OR REPLACE FUNCTION orders.mt_insert_order(doc JSONB, docDotNetType varchar, docId uuid, docVersion uuid) RETURNS UUID LANGUAGE plpgsql SECURITY INVOKER AS $function$
BEGIN
INSERT INTO orders.mt_doc_order ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp());

  RETURN docVersion;
END;
$function$;


CREATE OR REPLACE FUNCTION orders.mt_update_order(doc JSONB, docDotNetType varchar, docId uuid, docVersion uuid) RETURNS UUID LANGUAGE plpgsql SECURITY INVOKER AS $function$
DECLARE
  final_version uuid;
BEGIN
  UPDATE orders.mt_doc_order SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp() where id = docId;

  SELECT mt_version FROM orders.mt_doc_order into final_version WHERE id = docId ;
  RETURN final_version;
END;
$function$;

