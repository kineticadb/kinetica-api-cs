# GPUdb C# API Changelog

## Version 7.0

### Version 7.0.5.0 - 2019-06-26

#### Added
-   Minor documentation and some options for some endpoints

#### Changed
-   Parameters for /visualize/isoschrone


### Version 7.0.4.0 -- 2019-06-20

#### Fixed
-   Protocol classes' constructor parameters with default values
    are now correctly defined and assigned.

### Version 7.0.0.0 - 2019-01-31

-   Version release


## Version 6.2.0 - 2018-03-25

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
