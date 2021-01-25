# Kinetica REST API Changelog


## Version 7.1

## Version 7.1.2.0 -- 2021-01-25

#### Changed Endpoints

##### Non-breaking Changes
-   The 'output_tracks' option is added for /match/graph's match_supply_demand_solver for timestamped tracks generation directly and accurately.
-   All graph endpoints except for ``/delete/graph`` and ``/modify/graph`` can now to a 'server_id' as an option.
-   /admin/reblance and /admin/remove/ranks' aggressiveness option defaults to 10
-   Added execute_as option to ``/create/materializedview`` and ``/alter/table`` endpoints.
-   /create/table/external, /insert/records/fromfile and /insert/records/frompayload' new option: 'type_inference_mode' with valid_choices: accurate, speed

##### Breaking Changes
-   Multiple graph servers support:
    -   ``/create/graph`` and ``/modify/graph`` response now has a 'result' field indicating success on all graph servers.
    -   ``/show/graph``
        -   Response now provide info about which graph server each graph is on.
        -   Will also provide some extra info about each server (CPU load and memory)
    -   ``/show/system/properties`` graph parameters are now vectorized and prefixed with 'graph.'
    -   ``/show/system status`` replaced graph_status by vector of graph status similar to ranks.
-   /admin/add/ranks, /admin/remove/ranks, and /admin/rebalance now require the database to be offline


## Version 7.1.1.0 -- 2020-11-05

#### Changed Endpoints

##### Non-breaking Changes
-   /create/type
    -   Added "uuid" as a new column property.
    -   Added "init_with_uuid" as a new column property for uuid column.
-   /create/table endpoint: added "SERIES" to the ``valid_choices`` for the
    ``partition_type``


## Version 7.1.0.0 -- 2020-08-18

#### Added
-   Added the following endpoints to support cluster resiliency:
    -   ``/admin/add/host``
    -   ``/admin/alter/host``
    -   ``/admin/remove/host``
    -   ``/admin/switchover``
-   Added the following endpoints to support SQL schemas:
    -   ``/create/schema``
    -   ``/alter/schema``
    -   ``/drop/schema``
    -   ``/show/schema``
    -   ``/has/schema``
-   Added the following endpoints to support data source:
    -   ``/create/datasource``
    -   ``alter/datasource``
    -   ``/drop/datasource``
    -   ``/show/datasource``
    -   ``/grant/permission/datasource``
    -   ``/revoke/permission/datasource``

#### Changed Endpoints

##### Non-breaking Changes
-   Deprecated collection_name parameters wherever they appear. The new method is to qualify the table name or set the default schema.
-   Added qualified_table_name to the response info map for the following endpoints:
    -   ``/create/materializedview``
    -   ``/create/table``
    -   ``/create/union``
    -   ``/merge/records``
-   Added qualified_projection_name to the response info map for the following endpoints:
    -   ``/create/projection``
-   Added qualified_result_table_name to the response info map for the following endpoints:
    -   ``/aggregate/groupby``
    -   ``/aggregate/unique``
    -   ``/aggregate/unpivot``
-   Added qualified_table_name to the response info map for the following endpoints:
    -   ``/filter``
    -   ``/filter/byarea``
    -   ``/filter/byarea/geometry``
    -   ``/filter/bybox``
    -   ``/filter/bybox/geometry``
    -   ``/filter/bygeometry``
    -   ``/filter/bylist``
    -   ``/filter/byradius``
    -   ``/filter/byradius/geometry``
    -   ``/filter/byrange``
    -   ``/filter/byseries``
    -   ``/filter/bystring``
    -   ``/filter/bytable``
    -   ``/filter/byvalue``
-   /admin/rebalance
    -   Renamed option table_whitelist to include_tables
    -   Renamed option table_blacklist to exclude_tables
-   /alter/system/properties
      - Removed "enable_compound_equi_join" as an option
-   /alter/user
    -   Added "set_default_schema" as an action
-   /alter/table
    -   Added "move_to_schema" as an action
-   /show/table
    -   Added "schema_name" to additional_info map
    -   Added the following table_descriptions: LOGICAL_VIEW, LOGICAL_EXTERNAL_TABLE, MATERIALIZED_EXTERNAL_TABLE, SCHEMA

- Added ``cb_pointalphas`` option and ``cb_pointalpha_attrs`` and
  ``cb_pointalpha_vals`` fields to ``visualize/image/classbreak`` to support
  manipulation of transparency in class-break visualization.

##### Breaking Changes
-   /admin/add/ranks
    -   changed added_ranks parameter in response to an array of strings
    -   removed results parameter from response
-   /admin/remove/ranks
    -   changed ranks parameter in request to an array of strings
    -   changed removed_ranks parameter in response to an array of strings
    -   removed results parameter from response



## Version 7.0


### Version 7.0.20.0 - 2020-11-18

#### Changed Endpoints

##### Non-breaking Changes
-   Added ``execute_as`` additional_info map key to ``/show/sql/proc`` response.


### Version 7.0.19.0 - 2020-08-24

#### Changed Endpoints

##### Non-breaking Changes
-   Added 'output_edge_path' (default=false) and 'output_wkt_path' (default=true)
    options for turning on and off ability to export out aggregated path lists
    columns onto the solution table for the path solvers of /solve/graph
    endpoint for more speed.
-   /create/table endpoint: added "SERIES" to the ``valid_choices`` for the
    ``partition_type``

### Version 7.0.18.0 - 2020-07-30

#### Changed Endpoints

##### Non-breaking Changes

- Added ``cb_pointalphas`` option and ``cb_pointalpha_attrs`` and
  ``cb_pointalpha_vals`` fields to ``visualize/image/classbreak`` to support
  manipulation of transparency in class-break visualization.

##### Breaking Changes


### Version 7.0.17.0 - 2020-07-06


##### Non-breaking Changes
-   Added a job_tag option to the following endpoints:
    -   ``/create/job``
    -   ``/get/job``
    -   ``/admin/alter/job``
-   Added a job_tag field to ``/show/job``

#### Changed Endpoints
- /match/graph option 'enable_truck_reuse' added for supply demand solver of /match/graph for reusing truck in multiple rounds from the same originating depot
- /match/graph option 'truck_service_limit' added for sypply demand solver of /match/graph as an additional constraint on the total cost of any truck's delivery route.
- /match/graph option 'max_num_threads" added for multi-core support for many trips map matching. If specified it will not be exceeded but it can be lower due to memory and number of core threads availibility.
- /match/graph solve_method 'incremental_weighted' is removed as it was defunc.
- /match/graph option 'unit_unloading_cost' added for match_supply_demand_solver type to add unloading time to the optimization.
- /match/graph option 'filter_folding_paths' added for the markov_chain solver type to filter out the folding paths for more accurate results.
- /match/graph option 'max_trip_cost' added for match_supply_demand solver type to restrict trips between stops excedding this number except from/to the origin.
- /solve/graph option 'accurate_snaps' added.
- /match/graph for solve type 'match_supply_demand' only: the option 'aggregated_output' (default: true) added
- /create/graph, /modify/graph: newly added option of 'add_turns' (default: false), 'turn_angle' (default: 60)
- /solve/graph, /match/graph: newly added options of 'left_turn_penaly', 'right_turn_penalty', 'intersection_penalty', 'sharp_turn_penalty' (default: 0.0)
- /solve/graph Added 'output_edge_path' (default=false) and 'output_wkt_path' (default=true) options for turning on and off ability to export out aggregated path lists columns onto the solutioon table for the path solvers of /solve/graph endpoint for more speed.

##### Breaking Changes
-

##### Non-breaking Changes
-   Added ``datasource_name`` option to ``/create/external/table`` and ``/insert/records/from/files``


### Version 7.0.16.0 - TBD

#### Changed Endpoints

##### Non-breaking Changes
-   Added 'unit_unloading_cost' option is added to the /match/graph endpoint for
    match_supply_demand solve case to add the unloading time per drop amount.
-   Added total_number_of_records and has_more_records to /get/recordsfromcollection response info map.

##### Breaking Changes

### Version 7.0.12.0 - 2020-1-10

##### Non-breaking Changes
-   Added "count" to the info map in the responses for create_projection, create_union and execute_sql.

### Version 7.0.11.0 - 2019-12-10

#### Added Endpoints
-   Added a new endpoint ``/insert/records/fromfiles`` to insert records from
    external files into a new or existing table.
-   Added a new endpoint ``/modify/graph`` for updates of an existing graph
    in a non-streaming fashion.

#### Changed Endpoints

##### Non-breaking Changes
-   Added an option ``remove_label_only`` to create and modify graph endpoints (see option's doc)
-   Added ``enable_overlapped_equi_join`` and ``enable_compound_equi_join`` options to ``/alter/system/properties``
-   Added ``columns`` and ``sql_where`` options to ``/grant/permission/table``

### Version 7.0.10.0 - 2019-11-13

#### Changed Endpoints

##### Breaking Changes
-   Add ``additional_info`` map to ``/show/sql/proc`` to list attributes of sql procedures.

##### Non-breaking Changes
- Added ``allpaths`` solve graph option to solve for all the paths between source and destination.
  It is recommended to run this with the options of max and min solution radiua set carefully.
  The min value should be greater than or equal to the shortest path cost and it is further advised to set the
  max_number_targets option to limit the resulting paths.

- Added ``modify`` create graph option. When ``recreate`` and ``modify`` is both true for
  an existing graph it'll update the graph instead of deleting and recreating it.

- Added ``match_batch_solves`` solver type valid choice to the ``/match/graph``
  endpoint's ``solve_method`` parameter. This solver is strictly for WKTPOINT
  source and destination pairs with an ID and runs as many shortest paths as
  there are unique source and destination pairs provided as efficiently as
  possible. This change was made to bridge the gap for the ``shortest_path``
  solver in batch mode only for WKTPOINT node types in ``/solve/graph`` because
  it is not possible to match the ID of the pair in the solution_table.
- Added ``execute_interval`` and ``execute_start_time`` to the ``/show/sql/proc``.
- Added default values for parameters ``offset`` and ``limit`` to the following
  endpoints:
    - ``/aggregate/groupby``
    - ``/aggregate/unique``
    - ``/execute/sql``
    - ``/get/records/bycolumn``
- Changed default limit from 10k to -9999 (END_OF_SET) for the following endpoints:
    - ``/aggregate/groupby``
    - ``/aggregate/unique``
    - ``/execute/sql``
    - ``/get/records/bycolumn``
    - ``/get/records/fromcollection``
    - ``/get/records``
- Added "compact_after_rebalance", "compact_only", and "repair_incorrectly_sharded_data" options to /admin/rebalance


### Version 7.0.9.0 - 2019-10-16

#### Added Endpoints

- Added new ``/show/sql/proc`` endpoint to show SQL Procedure definitions

#### Changed Endpoints

##### Breaking Changes

- Updated the ``/solve/graph/`` endpoint to remove clunky and obscure primary parameters:
    - Removed ``source_node``, ``source_node_id``, ``destination_node_ids``, and ``node_type`` parameters
    - Added ``source_nodes`` parameter as vector of strings that is of NODE component type. For backwards compatibility, one of the following could be passed to both ``source_nodes`` and ``destination_nodes``:
        - ``["{'POINT(10 10)'} AS NODE_WKTPOINT"]``
        - ``["POINT(10 10)"]``
        - ``["table.column AS NODE_WKTPOINT"]``
- In the case of ``backhaul_routing`` solver type, the fixed assets are now placed in ``source_nodes`` and the remote assets are placed in ``destination_nodes`` instead of having all assets being placed in ``destination_nodes`` and the number of fixed assets being placed in ``source_node_id``

##### Non-breaking Changes

- Added ``bad_record_indices`` and ``error_N`` info map keys to ``/insert/records`` response
- Added ``evict_columns`` option to ``/alter/system/properties``
- Added ``return_individual_errors``, ``allow_partial_batch``, and ``dry_run`` options to ``/insert/records``
- Added ``retain_partitions`` option to ``/create/projection``. This option will cause the projection to be created with the same partition scheme as the source table, if possible. In prior versions, this was the default behavior but we are now making it optional and turned off by default.
- Added a ``max_combinations`` option to ``/match/graph`` for ``match_supply_demand`` solver type
- Added ``index_type`` option to ``/alter/table``. This option affects the ``create_index`` and ``delete_index`` actions and can have a value of ``column`` (the default) or ``chunk_skip``. Setting the option to ``column`` will create/delete a standard attribute index while the value of ``chunk_skip`` will create/delete a chunk-skip index, which is useful in cases where there are large numbers of chunks (i.e. due to partitioning).

### Version 7.0.8.0 - 2019-09-18

#### Changed Endpoints

##### Non-breaking Changes

- Added ``truncate_strings`` option to ``/update/records``

### Version 7.0.7.0 - 2019-08-15

#### Changed Endpoints

##### Non-breaking Changes

- Added ``match_supply_demand`` solver type valid choice to the ``/match/graph`` endpoint's ``solve_method`` parameter
- Added ``partial_loading`` option to ``/match/graph``
- Added support for the ``ulong`` (unsigned long) property within the ``string`` type to ``/create/type``

### Version 7.0.6.0 - 2019-07-11

#### Added Endpoints

- Added internal ``/show/graph/grammar`` endpoint to be consumed by GAdmin
- Added internal ``/alter/graph`` endpoint to enable cancelling graph jobs
- Added the following endpoints to support proc (UDF) execute permissions:
    - ``/grant/permission/proc``
    - ``/revoke/permission/proc``

- Added ``/show/graph`` to show basic properties of one or all graphs on the graph server

#### Changed Endpoints

##### Breaking Changes

- Moved ``rings`` option to a top-level parameter in ``/query/graph``

##### Non-breaking Changes

- Added ``aggressiveness`` option to ``/admin/rebalance`` and ``/admin/remove/ranks``
- Added ``show_async_jobs`` option to ``/admin/show/jobs`` to enable showing completed async jobs as they are no longer shown by default

### Version 7.0.5.0 - 2019-06-25

#### Changed Endpoints

- Aligned ``/visualize/isochrone`` ``options`` parameter with the ``contour_options`` and ``solve_options`` parameters
- Added ``use_priority_queue_solvers`` option to ``/visualize/isochrone`` to accelerate solve times for small to medium underlying graphs

### Version 7.0.4.0 - 2019-06-05

### Version 7.0.3.0 - 2019-05-01

### Version 7.0.2.0 - 2019-04-10

### Version 7.0.1.0 - 2019-03-01

### Version 7.0.0.0 - 2018-06-28

#### Added Endpoints

- Added the following endpoints for resource management:

    - ``/create/resourcegroup``
    - ``/show/resourcegroups``
    - ``/alter/resourcegroup``
    - ``/delete/resourcegroup``
    - ``/alter/tier``

#### Changed Endpoints

##### Breaking Changes

- Updated ``/create/user/internal`` to handle resource group names

##### Non-breaking Changes

- Added ``info`` string-to-string map to the responses of all endpoints. This map will contain additional information that may vary from endpoint to endpoint
- Updated ``/show/security`` response to include resource group names
- Updated ``/create/user/internal`` response to handle resource group names

## Version 6.2

### Version 6.2.0.0 - 2018-02-11

#### Added Endpoints

- Added the following endpoints to submit synchronously running jobs and retrieve the results of the job:
    - ``/create/job``
    - ``/get/job``
- Added ``/create/materializedview`` endpoint

#### Changed Endpoints

##### Non-breaking Changes

- Added ``max_concurrency_per_node`` option to ``/create/proc``

## Version 6.1

### Version 6.1.0.0 - 2018-01-08

#### Added Endpoints

- Added the following geometry-specific endpoints:

    - ``/aggregate/minmax/geometry``
    - ``/filter/byarea/geometry``
    - ``/filter/bybox/geometry``
    - ``/filter/byradius/geometry``

#### Changed Endpoints

##### Breaking Changes

- The following endpoints no longer support operations on geometry columns:
    - ``/aggregate/minmax``
    - ``/filter/byarea``
    - ``/filter/bybox``
    - ``/filter/byradius``

##### Non-breaking Changes

- Added support for the ``wkt`` property within the ``string`` type to ``/create/type``
- Added support for column names other than "WKT" to ``/filter/bygeometry``

## Version 6.0

### Version 6.0.0.0 - 2017-01-24

#### Added Endpoints

- Added ``/has/proc`` endpoint to support external procs

#### Changed Endpoints

##### Breaking Changes

- Moved ``execution_mode`` option to a top-level parameter in ``/create/proc``
- Modified ``/execute/proc``
- Shuffled ``/create/projection`` parameter order

##### Non-breaking Changes

- Added ``timings`` to the ``/show/proc`` response
- The following supported values have been added to ``action`` in ``/alter/table``:
    - ``add_column``
    - ``delete_column``
    - ``change_column``
    - ``rename_table``
- The following options have been added to ``/alter/table``:
    - ``column_default_value``
    - ``column_properties``
    - ``column_type``
    - ``validate_change_column``
    - ``copy_values_from_column``
    - ``rename_column``
- Added support for the following properties to ``/create/type``:
    - ``decimal`` (base type ``string``)
    - ``date`` (base type ``string``)
    - ``time`` (base type ``string``)
- The following supported values have been added to ``mode`` in ``/create/union``:
    - ``union_all`` (default)
    - ``union``
    - ``union_distinct``
    - ``except``
    - ``intersect``
- The following options have been added to ``/create/projection``:
    - ``expression``
    - ``limit``

## Version 5.4

### Version 5.4.0.0 - 2016-11-30

#### Added Endpoints

- Added /create/projection endpoint for selecting a subset of columns (including derived columns) from a table into a new result table with optional sorting
- Added the following external proc support endpoints:
    - ``/create/proc``
    - ``/delete/proc``
    - ``/execute/proc`` (replaces Node.js-exclusive version)
    - ``/kill/proc``
    - ``/show/proc``
    - ``/show/proc/status``

#### Changed Endpoints

##### Non-breaking Changes

- Null values for nullable columns are now supported in ``/update/records``

## Version 5.2

### Version 5.2.0.0 - 2016-09-21

#### Added Endpoints

- Added the following security system endpoints:
    - ``/alter/user``
    - ``/create/role``
    - ``/create/user/external``
    - ``/create/user/internal``
    - ``/delete/role``
    - ``/delete/user``
    - ``/grant/permission/system``
    - ``/grant/permission/table``
    - ``/grant/role``
    - ``/revoke/permission/system``
    - ``/revoke/permission/table``
    - ``/revoke/role``
    - ``/show/security``
- Added the ``/create/union`` endpoint

#### Changed Endpoints

##### Breaking Changes

- ``/alter/table/properties`` merged into ``/alter/table``
- ``/show/table/properties`` merged into ``/show/table``
- ``/visualize/image/heatmap`` has additional style options and a simplified schema

##### Non-breaking Changes

- ``/get/records`` now shows if there are more records to retrieve
- Added ``percentile`` to the list of supported values for ``stats`` in ``/aggregate/statistics``
- Added support for changing the maximum request timeout time to ``/alter/system/properties``
- Added ``not_in_list`` to the list of supported values for the ``filter_mode`` option in ``/filter/bylist`` to enable inverse matching
- Added ``result_table`` option to ``/aggregate/groupby``
- Added support for ``arg_min``, ``arg_max``, and ``count_distinct`` aggregates to ``/aggregate/groupby``
- Added ``result_table`` option to ``/aggregate/unique``

## Version 5.1

### Version 5.1.0.0 - 2016-05-06

#### Changed Endpoints

##### Non-breaking Changes

- Added ``having`` option to ``/aggregate/groupby``

## Version 4.2

### Version 4.2.0.0 - 2016-04-11

#### Changed Endpoints

##### Breaking Changes

- Refactor schemas and integrate documentation into JSON schemas
