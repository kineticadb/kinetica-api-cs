/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.filterByBox(string,string,string,double,double,string,double,double,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Calculates how many objects within the given table lie in a rectangular
    /// box. The operation is synchronous, meaning that a response will not be
    /// returned until all the objects are fully available. The response
    /// payload provides the count of the resulting set. A new resultant set
    /// which satisfies the input NAI restriction specification is also created
    /// when a <see cref="view_name" /> is passed in as part of the input
    /// payload.</summary>
    public class FilterByBoxRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByBoxRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>Name of a collection which is to contain the
        /// newly created view. If the collection provided is non-existent, the
        /// collection will be automatically created. If empty, then the newly
        /// created view will be top-level.</description>
        ///     </item>
        /// </list>
        /// <br />
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>Name of a collection which is to contain the newly
            /// created view. If the collection provided is non-existent, the
            /// collection will be automatically created. If empty, then the
            /// newly created view will be top-level.</summary>
            public const string COLLECTION_NAME = "collection_name";
        } // end struct Options


        /// <summary>Name of the table on which the bounding box operation will
        /// be performed. Must be an existing table.  </summary>
        public string table_name { get; set; }

        /// <summary>Optional name of the result view that will be created
        /// containing the results of the query. Has the same naming
        /// restrictions as <a href="../../concepts/tables.html"
        /// target="_top">tables</a>.  </summary>
        public string view_name { get; set; } = "";

        /// <summary>Name of the column on which to perform the bounding box
        /// query. Must be a valid numeric column.  </summary>
        public string x_column_name { get; set; }

        /// <summary>Lower bound for the column chosen by <paramref
        /// cref="FilterByBoxRequest.x_column_name" />.  Must be less than or
        /// equal to <paramref cref="FilterByBoxRequest.max_x" />.  </summary>
        public double min_x { get; set; }

        /// <summary>Upper bound for <paramref
        /// cref="FilterByBoxRequest.x_column_name" />.  Must be greater than
        /// or equal to <paramref cref="FilterByBoxRequest.min_x" />.
        /// </summary>
        public double max_x { get; set; }

        /// <summary>Name of a column on which to perform the bounding box
        /// query. Must be a valid numeric column.  </summary>
        public string y_column_name { get; set; }

        /// <summary>Lower bound for <paramref
        /// cref="FilterByBoxRequest.y_column_name" />. Must be less than or
        /// equal to <paramref cref="FilterByBoxRequest.max_y" />.  </summary>
        public double min_y { get; set; }

        /// <summary>Upper bound for <paramref
        /// cref="FilterByBoxRequest.y_column_name" />. Must be greater than or
        /// equal to <paramref cref="FilterByBoxRequest.min_y" />.  </summary>
        public double max_y { get; set; }

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByBoxRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>Name of a collection which is to contain the
        /// newly created view. If the collection provided is non-existent, the
        /// collection will be automatically created. If empty, then the newly
        /// created view will be top-level.</description>
        ///     </item>
        /// </list>
        ///   </summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a FilterByBoxRequest object with default
        /// parameters.</summary>
        public FilterByBoxRequest() { }

        /// <summary>Constructs a FilterByBoxRequest object with the specified
        /// parameters.</summary>
        /// 
        /// <param name="table_name">Name of the table on which the bounding
        /// box operation will be performed. Must be an existing table.
        /// </param>
        /// <param name="view_name">Optional name of the result view that will
        /// be created containing the results of the query. Has the same naming
        /// restrictions as <a href="../../concepts/tables.html"
        /// target="_top">tables</a>.  </param>
        /// <param name="x_column_name">Name of the column on which to perform
        /// the bounding box query. Must be a valid numeric column.  </param>
        /// <param name="min_x">Lower bound for the column chosen by <paramref
        /// cref="FilterByBoxRequest.x_column_name" />.  Must be less than or
        /// equal to <paramref cref="FilterByBoxRequest.max_x" />.  </param>
        /// <param name="max_x">Upper bound for <paramref
        /// cref="FilterByBoxRequest.x_column_name" />.  Must be greater than
        /// or equal to <paramref cref="FilterByBoxRequest.min_x" />.  </param>
        /// <param name="y_column_name">Name of a column on which to perform
        /// the bounding box query. Must be a valid numeric column.  </param>
        /// <param name="min_y">Lower bound for <paramref
        /// cref="FilterByBoxRequest.y_column_name" />. Must be less than or
        /// equal to <paramref cref="FilterByBoxRequest.max_y" />.  </param>
        /// <param name="max_y">Upper bound for <paramref
        /// cref="FilterByBoxRequest.y_column_name" />. Must be greater than or
        /// equal to <paramref cref="FilterByBoxRequest.min_y" />.  </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByBoxRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>Name of a collection which is to contain the
        /// newly created view. If the collection provided is non-existent, the
        /// collection will be automatically created. If empty, then the newly
        /// created view will be top-level.</description>
        ///     </item>
        /// </list>
        ///   </param>
        /// 
        public FilterByBoxRequest( string table_name,
                                   string view_name,
                                   string x_column_name,
                                   double min_x,
                                   double max_x,
                                   string y_column_name,
                                   double min_y,
                                   double max_y,
                                   IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.view_name = view_name ?? "";
            this.x_column_name = x_column_name ?? "";
            this.min_x = min_x;
            this.max_x = max_x;
            this.y_column_name = y_column_name ?? "";
            this.min_y = min_y;
            this.max_y = max_y;
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class FilterByBoxRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.filterByBox(string,string,string,double,double,string,double,double,IDictionary{string, string})"
    /// />.</summary>
    public class FilterByBoxResponse : KineticaData
    {

        /// <summary>The number of records passing the box filter.  </summary>
        public long count { get; set; }

    } // end class FilterByBoxResponse




}  // end namespace kinetica
