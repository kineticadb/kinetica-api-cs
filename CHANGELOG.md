# GPUdb C# API Changelog

## Version 7.2

### Version 7.2.2.1 - 2025-03-28

#### Added
-   Example using a JDBC bridge driver with Kinetica JDBC

#### Changed
-   Code styling update

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint related changes.


### Version 7.2.2.0 - 2024-10-15

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint related changes.


### Version 7.2.1.0 - 2024-09-07

#### Added
-   OAuth2 authentication support


### Version 7.2.0.0 - 2024-02-11

#### Added
-   Support for Array & Vector types

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint related changes.



## Version 7.1

### Version 7.1.10.0 - 2024-05-16

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint related changes.


### Version 7.1.9.1 - 2023-09-17

#### Changed
-   Converted to .NET Core project


### Version 7.1.9.0 - 2023-03-19

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint related changes.


### Version 7.1.8.0 - 2022-10-22

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint related changes.


### Version 7.1.7.0 - 2022-07-18

#### Changed
-   Removed client-side primary key check, to improve performance and make
    returned errors more consistently delivered

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint related changes.


### Version 7.1.6.0 - 2022-01-27

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint related changes.


### Version 7.1.5.0 - 2021-10-13

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint related changes.


### Version 7.1.4.0 - 2021-07-29

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint related changes.


### Version 7.1.3.0 - 2021-03-05

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint related changes.



### Version 7.1.2.0 - 2021-01-25

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint related changes.



### Version 7.1.1.0 - 2020-10-28

#### Note
-   Check CHANGELOG-FUNCTIONS.md for endpoint related changes.



### Version 7.1.0.0 - 2020-08-18

#### Note
-   Version release
-   Check CHANGELOG-FUNCTIONS.md for endpoint related changes.



## Version 7.0

### Version 7.0.20.0 - 2020-11-25

#### Note
-   Check CHANGELOG-FUNCTIONS.md for endpoint related changes.


### Version 7.0.19.0 - 2020-08-24

#### Note
-   Check CHANGELOG-FUNCTIONS.md for endpoint related changes.


### Version 7.0.18.0 - 2020-07-30

#### Note
-   Check CHANGELOG-FUNCTIONS.md for endpoint related changes.


### Version 7.0.17.0 - 2020-07-06

#### Note
-   Check CHANGELOG-FUNCTIONS.md for endpoint related changes.


### Version 7.0.16.0 - 2020-05-28

#### Note
-   Check CHANGELOG-FUNCTIONS.md for endpoint related changes.


### Version 7.0.15.0 - 2020-04-27

#### Note
-   Check CHANGELOG-FUNCTIONS.md for endpoint related changes.



### Version 7.0.14.0 - 2020-03-25

#### Note
-   Check CHANGELOG-FUNCTIONS.md for endpoint related changes.



### Version 7.0.13.0 - 2020-03-10

#### Note
-   Check CHANGELOG-FUNCTIONS.md for endpoint related changes.



### Version 7.0.12.0 - 2020-01-17

#### Note
-   Check CHANGELOG-FUNCTIONS.md for endpoint related changes.


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



## Version 6.1

## Version 6.1.0 - 2017-10-05

-   Added support for datetime



## Version 6.0

## Version 6.0.1 - 2017-06-19

-   Added multi-head ingestion support


## Version 6.0.0 - 2017-05-03

-   Version release
