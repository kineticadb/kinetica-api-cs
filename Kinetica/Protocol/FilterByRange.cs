/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.filterByRange(string,string,string,double,double,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Calculates which objects from a table have a column that is within the
    /// given bounds. An object from the table identified by <see
    /// cref="table_name" /> is added to the view <see cref="view_name" /> if
    /// its column is within [<see cref="lower_bound" />, <see
    /// cref="upper_bound" />] (inclusive). The operation is synchronous. The
    /// response provides a count of the number of objects which passed the
    /// bound filter.  Although this functionality can also be accomplished
    /// with the standard filter function, it is more efficient.
    /// <br />
    /// For track objects, the count reflects how many points fall within the
    /// given bounds (which may not include all the track points of any given
    /// track).</summary>
    public class FilterByRangeRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByRangeRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
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


        /// <summary>Name of the table on which the filter by range operation
        /// will be performed.  Must be an existing table.  </summary>
        public string table_name { get; set; }

        /// <summary>If provided, then this will be the name of the view
        /// containing the results. Has the same naming restrictions as <a
        /// href="../../concepts/tables.html" target="_top">tables</a>.
        /// </summary>
        public string view_name { get; set; } = "";

        /// <summary>Name of a column on which the operation would be applied.
        /// </summary>
        public string column_name { get; set; }

        /// <summary>Value of the lower bound (inclusive).  </summary>
        public double lower_bound { get; set; }

        /// <summary>Value of the upper bound (inclusive).  </summary>
        public double upper_bound { get; set; }

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByRangeRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>Name of a collection which is to contain the
        /// newly created view. If the collection provided is non-existent, the
        /// collection will be automatically created. If empty, then the newly
        /// created view will be top-level.</description>
        ///     </item>
        /// </list>
        ///   </summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a FilterByRangeRequest object with default
        /// parameters.</summary>
        public FilterByRangeRequest() { }

        /// <summary>Constructs a FilterByRangeRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="table_name">Name of the table on which the filter by
        /// range operation will be performed.  Must be an existing table.
        /// </param>
        /// <param name="view_name">If provided, then this will be the name of
        /// the view containing the results. Has the same naming restrictions
        /// as <a href="../../concepts/tables.html" target="_top">tables</a>.
        /// </param>
        /// <param name="column_name">Name of a column on which the operation
        /// would be applied.  </param>
        /// <param name="lower_bound">Value of the lower bound (inclusive).
        /// </param>
        /// <param name="upper_bound">Value of the upper bound (inclusive).
        /// </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByRangeRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>Name of a collection which is to contain the
        /// newly created view. If the collection provided is non-existent, the
        /// collection will be automatically created. If empty, then the newly
        /// created view will be top-level.</description>
        ///     </item>
        /// </list>
        ///   </param>
        /// 
        public FilterByRangeRequest( string table_name,
                                     string view_name,
                                     string column_name,
                                     double lower_bound,
                                     double upper_bound,
                                     IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.view_name = view_name ?? "";
            this.column_name = column_name ?? "";
            this.lower_bound = lower_bound;
            this.upper_bound = upper_bound;
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class FilterByRangeRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.filterByRange(string,string,string,double,double,IDictionary{string, string})"
    /// />.</summary>
    public class FilterByRangeResponse : KineticaData
    {

        /// <summary>The number of records passing the range filter.
        /// </summary>
        public long count { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class FilterByRangeResponse




}  // end namespace kinetica
