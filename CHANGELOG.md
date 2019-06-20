# GPUdb C# API Changelog

## Version 6.2

## Version 6.2.0.1 - 2019-06-20

#### Changed
-   Protocol classes' constructor parameters with default values
    are defined and assigned differently (using nullable types).


## Version 6.2.0.0 - 2018-03-25

-   Added new RecordRetriever class to support multi-head record lookup by
    shard key.
-   Refactored the following classes from KineticaIngestor.cs to
    the kinetica.Utils namespace:
    -   WorkerList
    -   RecordKey
    -   RecordKeyBuilder
    -   WorkerQueue


## Version 6.1.0 - 2017-10-05

-   Added support for datetime


## Version 6.0.1 - 2017-06-19

-   Added multi-head ingestion support


## Version 6.0.0 - 2017-05-03

-   Version release
