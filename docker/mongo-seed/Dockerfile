FROM mongo

COPY database.agz /database.agz

RUN sleep 5
CMD mongorestore --uri mongodb://mongo-db:27017 --gzip --drop --archive="/database.agz" --nsInclude="database.*";