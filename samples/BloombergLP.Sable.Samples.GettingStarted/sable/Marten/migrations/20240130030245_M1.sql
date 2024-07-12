-- Generated by Sable on 1/30/2024 3:02:45 AM
    
    
-- MANUALLY EDITED TO BE MADE INDEMPOTENT SINCE THE 'NoIdempotenceWrapper' DIRECTIVE IS SET. THIS IS AN ADVANCED USE CASE
-- ORIGINALLY GENERATED SCRIPT: CREATE INDEX CONCURRENTLY mt_doc_book_idx_name ON books.mt_doc_book USING btree ((data ->> 'Name'));
    
-- Sable NoIdempotenceWrapper
-- Sable NoTransactionWrapper

CREATE INDEX CONCURRENTLY IF NOT EXISTS mt_doc_book_idx_name ON books.mt_doc_book USING btree ((data ->> 'Name'));
