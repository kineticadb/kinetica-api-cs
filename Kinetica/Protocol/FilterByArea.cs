/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.filterByArea(string,string,string,IList{double},string,IList{double},IDictionary{string, string})"
    /// />.
    /// <br />
    /// Calculates which objects from a table are within a named area of
    /// interest (NAI/polygon). The operation is synchronous, meaning that a
    /// response will not be returned until all the matching objects are fully
    /// available. The response payload provides the count of the resulting
    /// set. A new resultant set (view) which satisfies the input NAI
    /// restriction specification is created with the name <see
    /// cref="view_name" /> passed in as part of the input.</summary>
    public class FilterByAreaRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByAreaRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>Name of a collection which is to contain the
        /// newly created view. If the collection provided is non-existent, the
        /// collection will be automatically created.  If empty, then the newly
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
            /// collection will be automatically created.  If empty, then the
            /// newly created view will be top-level.</summary>
            public const string COLLECTION_NAME = "collection_name";
        } // end struct Options


        /// <summary>Name of the table to filter.  This may be the name of a
        /// collection, a table or a view (when chaining queries).  Collections
        /// may be filtered only if all tables within the collection have the
        /// same type ID.  </summary>
        public string table_name { get; set; }

        /// <summary>If provided, then this will be the name of the view
        /// containing the results. Has the same naming restrictions as <a
        /// href="../../concepts/tables.html" target="_top">tables</a>.
        /// </summary>
        public string view_name { get; set; } = "";

        /// <summary>Name of the column containing the x values to be filtered.
        /// </summary>
        public string x_column_name { get; set; }

        /// <summary>List of x coordinates of the vertices of the polygon
        /// representing the area to be filtered.  </summary>
        public IList<double> x_vector { get; set; } = new List<double>();

        /// <summary>Name of the column containing the y values to be filtered.
        /// </summary>
        public string y_column_name { get; set; }

        /// <summary>List of y coordinates of the vertices of the polygon
        /// representing the area to be filtered.  </summary>
        public IList<double> y_vector { get; set; } = new List<double>();

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByAreaRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>Name of a collection which is to contain the
        /// newly created view. If the collection provided is non-existent, the
        /// collection will be automatically created.  If empty, then the newly
        /// created view will be top-level.</description>
        ///     </item>
        /// </list>
        ///   </summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a FilterByAreaRequest object with default
        /// parameters.</summary>
        public FilterByAreaRequest() { }

        /// <summary>Constructs a FilterByAreaRequest object with the specified
        /// parameters.</summary>
        /// 
        /// <param name="table_name">Name of the table to filter.  This may be
        /// the name of a collection, a table or a view (when chaining
        /// queries).  Collections may be filtered only if all tables within
        /// the collection have the same type ID.  </param>
        /// <param name="view_name">If provided, then this will be the name of
        /// the view containing the results. Has the same naming restrictions
        /// as <a href="../../concepts/tables.html" target="_top">tables</a>.
        /// </param>
        /// <param name="x_column_name">Name of the column containing the x
        /// values to be filtered.  </param>
        /// <param name="x_vector">List of x coordinates of the vertices of the
        /// polygon representing the area to be filtered.  </param>
        /// <param name="y_column_name">Name of the column containing the y
        /// values to be filtered.  </param>
        /// <param name="y_vector">List of y coordinates of the vertices of the
        /// polygon representing the area to be filtered.  </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByAreaRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>Name of a collection which is to contain the
        /// newly created view. If the collection provided is non-existent, the
        /// collection will be automatically created.  If empty, then the newly
        /// created view will be top-level.</description>
        ///     </item>
        /// </list>
        ///   </param>
        /// 
        public FilterByAreaRequest( string table_name,
                                    string view_name,
                                    string x_column_name,
                                    IList<double> x_vector,
                                    string y_column_name,
                                    IList<double> y_vector,
                                    IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.view_name = view_name ?? "";
            this.x_column_name = x_column_name ?? "";
            this.x_vector = x_vector ?? new List<double>();
            this.y_column_name = y_column_name ?? "";
            this.y_vector = y_vector ?? new List<double>();
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class FilterByAreaRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.filterByArea(string,string,string,IList{double},string,IList{double},IDictionary{string, string})"
    /// />.</summary>
    public class FilterByAreaResponse : KineticaData
    {

        /// <summary>The number of records passing the area filter.  </summary>
        public long count { get; set; }

    } // end class FilterByAreaResponse




}  // end namespace kinetica
