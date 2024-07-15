CREATE INDEX mt_doc_book_idx_name ON books.mt_doc_book USING btree ((data ->> 'Name'));
