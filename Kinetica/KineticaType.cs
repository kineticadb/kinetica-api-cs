using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Avro;
using Newtonsoft.Json.Linq;

namespace kinetica;

/// <summary>Describes a Kinetica record type — its columns, properties, and Avro schema — and can create the type in the database.</summary>
public class KineticaType
{
    /// <summary>Describes a single column of a <see cref="KineticaType"/>: its name, data type, and properties.</summary>
    public class Column
    {
        /// <summary>The primitive data type of a column.</summary>
        public enum ColumnType
        {
            /// <summary>A byte-array (binary) column.</summary>
            BYTES = (int)Avro.Schema.Type.Bytes,
            /// <summary>A double-precision floating-point column.</summary>
            DOUBLE = (int)Avro.Schema.Type.Double,
            /// <summary>A single-precision floating-point column.</summary>
            FLOAT = (int)Avro.Schema.Type.Float,
            /// <summary>A 32-bit integer column.</summary>
            INT = (int)Avro.Schema.Type.Int,
            /// <summary>A 64-bit integer column.</summary>
            LONG = (int)Avro.Schema.Type.Long,
            /// <summary>A string column.</summary>
            STRING = (int)Avro.Schema.Type.String,
            /// <summary>A boolean column.</summary>
            BOOLEAN = (int)Avro.Schema.Type.Boolean,
            /// <summary>An unset/unrecognized column type.</summary>
            DEFAULT = (int)Avro.Schema.Type.Error
        };

        /// <summary>
        /// Default precision for decimal columns (matches Java API).
        /// </summary>
        public const int DEFAULT_DECIMAL_PRECISION = 18;

        /// <summary>
        /// Default scale for decimal columns (matches Java API).
        /// </summary>
        public const int DEFAULT_DECIMAL_SCALE = 4;

        /// <summary>
        /// Maximum precision for 8-byte decimals. Decimals with precision > 18 use 12 bytes.
        /// </summary>
        public const int DECIMAL8_MAX_PRECISION = 18;

        /// <summary>
        /// Regex to parse decimal(precision, scale) format from column properties.
        /// </summary>
        private static readonly Regex DecimalPattern = new Regex(
            @"decimal\s*\(\s*(\d+)\s*,\s*(\d+)\s*\)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private string m_name;
        private ColumnType m_type;
        private bool m_isNullable;
        private IList<string> m_properties;
        private int m_precision;
        private int m_scale;
        private bool m_isDecimal;

        /// <summary>
        /// Creates a Column object from the given name, type, and properties.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="properties"></param>
        public Column(string name, ColumnType type, IList<string>? properties = null)
        {
            m_name = name;
            m_type = type;
            m_isNullable = false;
            m_properties = properties ?? [];

            Initialize();
        }

        /// <summary>
        /// Returns the name of the column.
        /// </summary>
        /// <returns></returns>
        public string getName() { return m_name; }

        /// <summary>
        /// Returns the enumeration for the column type.
        /// </summary>
        /// <returns></returns>
        public ColumnType getType() { return m_type; }

        /// <summary>
        /// Returns if the column is nullable.
        /// </summary>
        /// <returns></returns>
        public bool isNullable() { return m_isNullable; }

        /// <summary>
        /// Returns the properties for the column.
        /// </summary>
        /// <returns></returns>
        public IList<string> getProperties() { return m_properties; }

        /// <summary>
        /// Returns whether this column is a decimal type.
        /// </summary>
        /// <returns>True if the column has a decimal property.</returns>
        public bool isDecimal() { return m_isDecimal; }

        /// <summary>
        /// Returns the precision for decimal columns.
        /// For non-decimal columns, returns DEFAULT_DECIMAL_PRECISION.
        /// </summary>
        /// <returns>The total number of digits in the decimal.</returns>
        public int getDecimalPrecision() { return m_precision; }

        /// <summary>
        /// Returns the scale for decimal columns.
        /// For non-decimal columns, returns DEFAULT_DECIMAL_SCALE.
        /// </summary>
        /// <returns>The number of digits after the decimal point.</returns>
        public int getDecimalScale() { return m_scale; }

        /// <summary>
        /// Returns the byte size needed to store decimal values for this column.
        /// Returns 8 for precision &lt;= 18, or 12 for precision &gt; 18.
        /// </summary>
        /// <returns>The byte size (8 or 12) for storing decimal values.</returns>
        public int getDecimalByteSize() { return m_precision > DECIMAL8_MAX_PRECISION ? 12 : 8; }

        internal void setIsNullable( bool val ) { m_isNullable = val;  }

        /// <summary>
        /// Returns the string format of the data type.
        /// </summary>
        /// <returns></returns>
        public string getTypeString()
        {
            return m_type switch
            {
                ColumnType.BYTES => "bytes",
                ColumnType.DOUBLE => "double",
                ColumnType.FLOAT => "float",
                ColumnType.INT => "int",
                ColumnType.LONG => "long",
                ColumnType.STRING => "string",
                _ => throw new KineticaException("Unsupported column type: " + m_type),
            };
        }  // end getTypeString()

        private void Initialize()
        {
            if (string.IsNullOrEmpty(m_name))
            {
                throw new ArgumentException("Name must not be empty.");
            }

            switch (m_type)
            {
                case ColumnType.BYTES:
                case ColumnType.DOUBLE:
                case ColumnType.FLOAT:
                case ColumnType.INT:
                case ColumnType.LONG:
                case ColumnType.STRING:
                case ColumnType.BOOLEAN:
                    break;

                default:
                    throw new ArgumentException($"Column {m_name} must be of type BYTES, DOUBLE, FLOAT, INT, LONG, STRING or BOOLEAN.");
            }

            // Initialize decimal info with defaults
            m_precision = DEFAULT_DECIMAL_PRECISION;
            m_scale = DEFAULT_DECIMAL_SCALE;
            m_isDecimal = false;

            foreach (var it in m_properties)
            {
                if (string.IsNullOrEmpty(it))
                {
                    throw new ArgumentException("Properties must not be empty.");
                }

                if (!m_isNullable && (it == ColumnProperty.NULLABLE))
                {
                    m_isNullable = true;
                }

                // Check for decimal property and extract precision/scale
                if (!m_isDecimal)
                {
                    SetDecimalInfo(it);
                }
            }
        }

        /// <summary>
        /// Parses a decimal property string and extracts precision and scale.
        /// Supports formats: "decimal", "decimal(19,4)"
        /// </summary>
        /// <param name="property">The property string to parse.</param>
        private void SetDecimalInfo(string property)
        {
            // Check for simple "decimal" property
            if (property.Equals(ColumnProperty.DECIMAL, StringComparison.OrdinalIgnoreCase))
            {
                m_isDecimal = true;
                // Use defaults already set
                return;
            }

            // Check for decimal(precision, scale) format
            var match = DecimalPattern.Match(property);
            if (match.Success)
            {
                m_isDecimal = true;
                if (int.TryParse(match.Groups[1].Value, out int precision))
                    m_precision = precision;
                if (int.TryParse(match.Groups[2].Value, out int scale))
                    m_scale = scale;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{m_name} ({m_type})";
        }
    }  // end class Column

    private class TypeData
    {
        public string? label;
        public IList<Column> columns = [];
        public Dictionary<string, int> columnMap = [];
        public string? schemaString = null;
        public Schema? schema = null;
        public Type? sourceType = null;
    }

    private TypeData m_data = new();
    private IDictionary<string, IList<string>> m_properties = new Dictionary<string, IList<string>>();
    private string? m_typeId = null;

    /// <summary>
    /// Create a KineticaType object based on an existing table in the database.
    /// </summary>
    /// <param name="kinetica"></param>
    /// <param name="tableName"></param>
    /// <returns></returns>
    public static KineticaType fromTable(Kinetica kinetica, string tableName)
    {
        var response = kinetica.showTable(tableName);
        var typeIdCount = response.type_ids.Count;

        if (typeIdCount == 0)
        {
            throw new KineticaException($"Table {tableName} does not exist.");
        }

        string typeId = response.type_ids[0];
        if (typeIdCount > 1)
        {
            for (int i = 1; i < typeIdCount; ++i)
            {
                if (response.type_ids[i] != typeId)
                {
                    throw new KineticaException("Table {tableName} is not homogeneous.");
                }
            }
        }

        return new KineticaType(response.type_labels[0], response.type_schemas[0], response.properties[0], typeId );
    }

    /// <summary>
    /// Create a KineticaType object based on an existing type in the database.
    /// </summary>
    /// <param name="kinetica"></param>
    /// <param name="typeId"></param>
    /// <returns></returns>
    public static KineticaType fromTypeID(Kinetica kinetica, string typeId)
    {
        var response = kinetica.showTypes(typeId, "");

        if (response.type_ids.Count < 1)
        {
            throw new KineticaException($"Type {typeId} does not exist.");
        }

        return new KineticaType(response.labels[0], response.type_schemas[0], response.properties[0]);
    }


    /// <summary>
    /// Create a KineticaType object based on information provided in a dynamic schema.
    /// </summary>
    /// <param name="dynamicTableSchemaString">The dynamic schema string.</param>
    /// <param name="columnHeaders">List of column names.</param>
    /// <param name="columnTypes">List of column types.</param>
    /// <returns></returns>
    /// <remarks>
    /// This method parses dynamic JSON schemas at runtime and creates types dynamically.
    /// It is not compatible with trimming or Native AOT compilation.
    /// </remarks>
    [RequiresUnreferencedCode("Dynamic schema parsing uses JSON reflection which may be incompatible with trimming.")]
    public static KineticaType fromDynamicSchema( string dynamicTableSchemaString,
                                                  Object[] columnHeaders, Object[] columnTypes )
    {
        // Make sure that the lists of column names and types are of the same length
        if ( columnHeaders.Length != columnTypes.Length )
            throw new KineticaException($"List of column names ({columnHeaders.Length}) and types ({columnTypes.Length}) are not of the same length." );

        // Parse the schema string so that we can later check if a given column is nullable
        JObject dynamicSchemaJson;
        try
        {
            dynamicSchemaJson = JObject.Parse( dynamicTableSchemaString );
        }
        catch ( Exception ex )
        {
            throw new KineticaException( ex.ToString() );
        }

        // Create a delegate for checking if a field/column is nullable
        // ------------------------------------------------------------
        // The first parameter is the column name, the second is the JSON object
        var isColumnNullable = new Func<string, JObject, bool>( (columnName, schemaJson) => {
            // Find the appropriate field
            bool foundField = false;
            JToken? fieldList = schemaJson["fields"];
            if (fieldList != null)
            {
                foreach (var field in fieldList)
                {
                    if ((string?)field["name"] == columnName)
                    {
                        foundField = true; // found it!
                        // Get the type and see if it's a nullable type
                        // (each field is an array of the column type; so need
                        // to extract the type element first)
                        JToken? fieldType = field["type"];
                        if (fieldType != null)
                        {
                            var typeElement = fieldType["items"];
                            if (typeElement == null || typeElement is JValue) // not an array, so can't be nullable
                                return false;
                            // If the type is an array and the second value is 'null', then it's a nullable
                            if ((typeElement is JArray) && ((string)(typeElement as JArray)[1] == "null"))
                                return true;
                            return false;
                        }
                    } // end if
                }  // end foreach
            }
            if ( !foundField )
                throw new KineticaException( $"Could not find the field named '{columnName}'" );
            return false; // shouldn't ever get here
        } );

        // Create appropriate columns and column properties
        // ------------------------------------------------
        List<Column> columns = [];
        Dictionary<string, IList<string>> columnProperties = [];
        for ( int i = 0; i < columnHeaders.Length; ++i )
        {
            // Get the column's name
            string column_name = (string)columnHeaders[i];

            // Get the column's type in string format, which might be a property and not a primitive type
            // (if so, then we'll have to infer the primitive type and save the property)
            string columnTypeString = (string)columnTypes[i];

            // Need to create a list for the properties of the column (we'll
            // extract at most one from the column type)
            List<string> columnProperty = [];

            // We need to also infer the primitive type for this column
            Column.ColumnType columnType;

            // Infer the type and property from the given 'type'
            switch ( columnTypeString )
            {
                // Primitive type string (and all properties based on it)
                case "string":
                    columnType = Column.ColumnType.STRING;
                    break;

                // All properties allowed for the primitive string type
                case ColumnProperty.CHAR1:
                case ColumnProperty.CHAR2:
                case ColumnProperty.CHAR4:
                case ColumnProperty.CHAR8:
                case ColumnProperty.CHAR16:
                case ColumnProperty.CHAR32:
                case ColumnProperty.CHAR64:
                case ColumnProperty.CHAR128:
                case ColumnProperty.CHAR256:
                case ColumnProperty.DATE:
                case ColumnProperty.DATETIME:
                case ColumnProperty.DECIMAL:
                case ColumnProperty.IPV4:
                case ColumnProperty.TIME:
                case ColumnProperty.ULONG:
                case ColumnProperty.UUID:
                case ColumnProperty.JSON:
                case ColumnProperty.WKT:
                    columnType = Column.ColumnType.STRING;
                    columnProperty.Add( columnTypeString );
                    break;

                // Geometry type returned by spatial SQL functions (ST_GeomFromText, ST_Intersection, etc.)
                // Server returns geometry values as WKT strings in the dynamic response payload
                // Note: Using string literal to match Java API and avoid modifying auto-generated ColumnProperty.cs
                case "geometry":
                    columnType = Column.ColumnType.STRING;
                    columnProperty.Add( ColumnProperty.WKT );  // Mark as WKT geometry for downstream detection
                    break;

                // Primitive type integer
                case "int":
                    columnType = Column.ColumnType.INT;
                    break;

                // Properties allowed for the primitive integer type
                case ColumnProperty.INT8:
                case ColumnProperty.INT16:
                case ColumnProperty.BOOLEAN:
                    columnType = Column.ColumnType.INT;
                    columnProperty.Add( columnTypeString );
                    break;

                // Primitive type long
                case "long":
                    columnType = Column.ColumnType.LONG;
                    break;

                // Properties allowed for the long type
                case ColumnProperty.TIMESTAMP:
                    columnType = Column.ColumnType.LONG;
                    columnProperty.Add( columnTypeString );
                    break;

                // Primitive type float
                case "float":
                    columnType = Column.ColumnType.FLOAT;
                    break;

                // Primitive type double
                case "double":
                    columnType = Column.ColumnType.DOUBLE;
                    break;

                // Primitive type bytes
                case "bytes":
                    columnType = Column.ColumnType.BYTES;
                    break;

                default:
                    // Handle array types (array(int), array(double,10), etc.)
                    if (columnTypeString.StartsWith(ColumnProperty.ARRAY, StringComparison.OrdinalIgnoreCase))
                    {
                        columnType = Column.ColumnType.STRING;
                        columnProperty.Add(columnTypeString);
                    }
                    // Handle vector types (vector(1024), etc.)
                    else if (columnTypeString.StartsWith(ColumnProperty.VECTOR, StringComparison.OrdinalIgnoreCase))
                    {
                        columnType = Column.ColumnType.BYTES;
                        columnProperty.Add(columnTypeString);
                    }
                    // Handle decimal types with precision/scale (decimal(18,4), etc.)
                    else if (columnTypeString.StartsWith(ColumnProperty.DECIMAL, StringComparison.OrdinalIgnoreCase))
                    {
                        columnType = Column.ColumnType.STRING;
                        columnProperty.Add(columnTypeString);
                    }
                    else
                    {
                        throw new KineticaException($"Unknown data type/property: {columnTypeString}");
                    }
                    break;
            }  // end switch

            // Check if the column is nullable (where the column name is "column_#" as returned by Kinetica)
            if ( isColumnNullable( $"column_{i + 1}", dynamicSchemaJson ) )
                columnProperty.Add( ColumnProperty.NULLABLE );

            // Now that we have the name, the type and potentially a property for the column,
            // create a Column type and add it to the list
            Column column = new( column_name, columnType, columnProperty );
            columns.Add( column );

            // Also, save the column property in the column name->property map
            columnProperties.Add( column_name, columnProperty );
        }  // end looping over column headers and types


        // Create and return the KineticaType object based on the columns and properties
        return new KineticaType( "", columns, columnProperties );
    }  // end fromDynamicSchema()



    /// <summary>
    /// Create a KineticaType object from properties of a record class and Kinetica column properties.
    /// It ignores any properties inherited from base classes, and also ignores any member fields of
    /// the class.
    ///
    /// For integer, long, float, and double column types, the user can use the nullable type (e.g. int?)
    /// to declare the column to be nullable.  The <paramref name="properties"/> does not need to contain
    /// the <see cref="ColumnProperty.NULLABLE"/> property.  However, for string type columns, instead of
    /// using nullable type, use the regular string type; additionally, add the
    /// <see cref="ColumnProperty.NULLABLE"/> in <paramref name="properties"/>.
    ///
    /// </summary>
    /// <param name="recordClass">A class type.</param>
    /// <param name="properties">Properties for the columns.</param>
    /// <returns></returns>
    /// <remarks>
    /// This method uses reflection to inspect the record class properties.
    /// It is not compatible with trimming or Native AOT compilation.
    /// </remarks>
    [RequiresUnreferencedCode("Uses reflection to inspect record class properties. Not compatible with trimming.")]
    public static KineticaType fromClass( Type recordClass, IDictionary<string, IList<string>> properties = null )
    {
        return fromClass( recordClass, "", properties );
    }   // end fromClass()


    /// <summary>
    /// Create a KineticaType object from properties of a record class and Kinetica column properties.
    /// It ignores any properties inherited from base classes, and also ignores any member fields of
    /// the class.
    /// </summary>
    /// <param name="recordClass">A class type.</param>
    /// <param name="label">Any label for the type.</param>
    /// <param name="properties">Properties for the columns.</param>
    /// <returns></returns>
    /// <remarks>
    /// This method uses reflection to inspect the record class properties.
    /// It is not compatible with trimming or Native AOT compilation.
    /// </remarks>
    [RequiresUnreferencedCode("Uses reflection to inspect record class properties. Not compatible with trimming.")]
    public static KineticaType fromClass( Type recordClass, string label, IDictionary<string, IList<string>>? properties = null )
    {
        // Get the fields in order (******skipping properties inherited from base classes******)
        // (fields only from this type, i.e. do not include any inherited fields), and public types only
        System.Reflection.PropertyInfo[] typeProperties = recordClass.GetProperties( System.Reflection.BindingFlags.DeclaredOnly |
                                                                                      System.Reflection.BindingFlags.Instance |
                                                                                      System.Reflection.BindingFlags.Public );

        Array.Sort( typeProperties, (p1, p2) => p1.MetadataToken.CompareTo( p2.MetadataToken ) );

        // Need to have a list of columns
        List<Column> columns = [];
        List<string> columnNames = [];

        // Per property, check that it is one of: int, long, float, double, string, bytes
        foreach ( var typeProperty in typeProperties )
        {
            string            columnName = "";
            Column.ColumnType columnType = Column.ColumnType.DEFAULT;
            IList<string>?    columnProperties = null;
            bool              isColumnNullable = false;

            // Get the column name
            columnName = typeProperty.Name;

            Type? propertyType = typeProperty.PropertyType;

            // Check if the field is nullable (declared as T? or Nullable<T>)
            if ( typeProperty.PropertyType.IsGenericType &&
                      ( typeProperty.PropertyType.GetGenericTypeDefinition() == typeof( Nullable<> ) ) )
            {   // the field is a nullable field
                isColumnNullable = true;
                // Change the property type to be the underlying type
                propertyType = Nullable.GetUnderlyingType(propertyType);
            }

            // Check the column data type (must be one of int, long, float, double, string, and bytes)
            if ( propertyType == typeof( System.String ) )
            {
                columnType = Column.ColumnType.STRING;
            }
            else if ( propertyType == typeof( System.Int32 ) )
            {
                columnType = Column.ColumnType.INT;
            }
            else if ( propertyType == typeof( System.Int64 ) )
            {
                columnType = Column.ColumnType.LONG;
            }
            else if ( propertyType == typeof( float ) )
            {
                columnType = Column.ColumnType.FLOAT;
            }
            else if ( propertyType == typeof( double ) )
            {
                columnType = Column.ColumnType.DOUBLE;
            }
            else if ( propertyType == typeof( byte ) )
            {
                columnType = Column.ColumnType.BYTES;
            }
            else
                throw new KineticaException( "Unsupported data type for " + propertyType?.Name +
                                          ": " + propertyType +
                                          " (must be one of int, long, float, double, string, and byte)" );

            // Extract the given column's properties, if any
            properties?.TryGetValue(columnName, out columnProperties);

            // Keep a list of the column names for checking the properties
            columnNames.Add( columnName );

            // Create the column
            Column column = new( columnName, columnType, columnProperties );
            if ( isColumnNullable ) // Set the appropriate nullable flag for the column
                column.setIsNullable( true );

            // Save the column
            columns.Add( column );
        }  // end looping over all members of the class type

        // Check for extraneous properties
        if (properties != null)
        {
            IEnumerable<string> propertyKeys = properties.Keys;
            var unknownColumns = propertyKeys.Where(e => !columnNames.Contains(e));
            // Check if any property is provided for wrong/non-existing columns
            if (unknownColumns.Any())
                throw new KineticaException("Properties specified for unknown columns.");
        }
        else properties = new Dictionary<string, IList<string>>();

        // Create the kinetica type
        KineticaType kType = new(label, columns, properties);

        // Save the class information in the type
        kType.saveSourceType( recordClass );

        return kType;
    } // end fromClass()


    /// <summary>
    /// Create a KineticaType object from properties of a record object and Kinetica column properties.
    /// It ignores any properties inherited from base classes, and also ignores any member fields of
    /// the class.
    /// </summary>
    /// <param name="recordObj">A record object.</param>
    /// <param name="properties">Properties for the columns.</param>
    /// <returns></returns>
    /// <remarks>
    /// This method uses reflection to inspect the record object's type.
    /// It is not compatible with trimming or Native AOT compilation.
    /// </remarks>
    [RequiresUnreferencedCode("Uses reflection to inspect record object type. Not compatible with trimming.")]
    public static KineticaType fromObject( Object recordObj, IDictionary<string, IList<string>> properties = null )
    {
        return fromObject( recordObj, "", properties );
    }  // end fromObject()


    /// <summary>
    /// Create a KineticaType object from properties of a record object and Kinetica column properties.
    /// It ignores any properties inherited from base classes, and also ignores any member fields of
    /// the class.
    /// </summary>
    /// <param name="recordObj">A record object.</param>
    /// <param name="label">Any label for the type.</param>
    /// <param name="properties">Properties for the columns.</param>
    /// <returns></returns>
    /// <remarks>
    /// This method uses reflection to inspect the record object's type.
    /// It is not compatible with trimming or Native AOT compilation.
    /// </remarks>
    [RequiresUnreferencedCode("Uses reflection to inspect record object type. Not compatible with trimming.")]
    public static KineticaType fromObject(Object recordObj, string label = "", IDictionary<string, IList<string>> properties = null)
    {
        // Create the type schema from the object
        // --------------------------------------
        // Get the class type
        Type object_type = recordObj.GetType();

        return fromClass( object_type, label, properties );
    }  // end fromObject()

    /// <summary>
    /// Create a KineticaType object with the given column information.
    /// </summary>
    /// <param name="columns">A list of Columns with information on all the columns for the type.</param>
    public KineticaType(IList<Column> columns)
    {
        m_data.columns = columns;
        Initialize();
        CreateSchema();  // create the schema from columns
    }

    /// <summary>
    /// Create a KineticaType object with the given column and label information.
    /// </summary>
    /// <param name="label">The label for the type.</param>
    /// <param name="columns">A list of Columns with information on all the columns for the type.</param>
    public KineticaType(string label, IList<Column> columns) : this(columns)
    {
        m_data.label = label;
    }

    /// <summary>
    /// Create a KineticaType object with the given column, label, and property information.
    /// </summary>
    /// <param name="label">The label for the type.</param>
    /// <param name="columns">A list of Columns with information on all the columns for the type.</param>
    /// <param name="properties">A per-column property information</param>
    public KineticaType( string label, IList<Column> columns, IDictionary<string, IList<string>> properties ) : this( label, columns )
    {
        m_properties = properties ?? new Dictionary<string, IList<string>>();
    }

    /// <summary>
    /// Create a KineticaType object using the string-formatted schema for the type.
    /// </summary>
    /// <param name="typeSchema"></param>
    public KineticaType(string typeSchema)
    {
        m_data.schemaString = typeSchema;
        CreateSchemaFromString( typeSchema );
        CreateSchema();
    }

    /// <summary>
    /// Create a KineticaType object using the string-formatted schema and properties for its columns.
    /// </summary>
    /// <param name="label">The label for the type.</param>
    /// <param name="typeSchema">The string-formatted schema for the type.</param>
    /// <param name="properties">A per-column based set of properties.</param>
    /// <param name="typeId">An optional ID for this type with which to identify it in the database.</param>
    public KineticaType(string label, string typeSchema, IDictionary<string, IList<string>> properties, string? typeId = null )
    {
        m_properties = properties;
        m_typeId = typeId;
        m_data.label = label;
        m_data.schemaString = typeSchema;
        CreateSchemaFromString(typeSchema, properties);
        CreateSchema();
    }

    /// <summary>Gets the type's label.</summary>
    public string getLabel() { return m_data.label; }
    /// <summary>Gets the list of columns that make up the type.</summary>
    public IList<Column> getColumns() { return m_data.columns; }
    /// <summary>Gets the column at the given zero-based index.</summary>
    /// <param name="index">The zero-based column index.</param>
    public Column getColumn(int index) { return m_data.columns[index]; }
    /// <summary>Gets the column with the given name.</summary>
    /// <param name="name">The column name.</param>
    public Column getColumn(string name) { return m_data.columns[getColumnIndex(name)]; }
    /// <summary>Gets the number of columns in the type.</summary>
    public int getColumnCount() { return m_data.columns.Count; }
    /// <summary>Gets the zero-based index of the column with the given name.</summary>
    /// <param name="name">The column name.</param>
    public int getColumnIndex(string name) { return m_data.columnMap[name]; }
    /// <summary>Determines whether the type contains a column with the given name.</summary>
    /// <param name="name">The column name.</param>
    public bool hasColumn(string name) { return m_data.columnMap.ContainsKey(name); }
    /// <summary>Gets the Avro schema describing the type.</summary>
    public Schema getSchema() { return m_data.schema; }
    /// <summary>Gets the .NET source type this Kinetica type was built from, or null if none.</summary>
    public Type? getSourceType() { return m_data.sourceType;}
    /// <summary>Gets the Avro schema as a JSON string.</summary>
    public string getSchemaString() { return m_data.schemaString; }
    /// <summary>Gets the server-assigned type ID, or empty if the type has not been created in the database.</summary>
    public string getTypeID() { return m_typeId;  }

    /// <summary>
    /// Saves the given type as this KineticaType's source type.
    /// </summary>
    /// <param name="sourceType">The type that works as the source. </param>
    public void saveSourceType( Type sourceType )
    {
        this.m_data.sourceType = sourceType;
    }  // end saveSourceType


    /// <summary>
    /// Given a handle to the server, creates a type in the database based
    /// on this data type.
    /// </summary>
    /// <param name="kinetica">The handle to the database server.</param>
    /// <returns>The ID with which the database identifies this type.</returns>
    public string create(Kinetica kinetica)
    {
        // Save the association between this KineticaType's source and itself in the Kinetica object
        // for future reference (it helps with encoding and decoding records)
        if ( this.m_data.sourceType != null )
            kinetica.SetKineticaSourceClassToTypeMapping( this.m_data.sourceType, this );

        // Register the type with Kinetica
        CreateTypeResponse response = kinetica.createType( m_data.schemaString, m_data.label, m_properties);
        return response.type_id;
    }  // end create()

    private KineticaType() { }

    /// <summary>
    /// Initializes the type based on the columns.  Verifies that the columns
    /// are valid.
    /// </summary>
    private void Initialize()
    {
        int columnCount = m_data.columns.Count;

        if (columnCount == 0)
        {
            throw new ArgumentException("At least one column must be specified.");
        }

        for (int i = 0; i < columnCount; ++i)
        {
            string columnName = m_data.columns[i].getName();

            if (m_data.columnMap.ContainsKey(columnName))
            {
                throw new ArgumentException("Duplicate column name " + columnName + " specified.");
            }

            m_data.columnMap[columnName] = i;
        }
    }  // end Initialize()


    /// <summary>
    /// Creates a schema object from a string.
    /// </summary>
    /// <param name="typeSchema">The schema in a string format.</param>
    /// <param name="properties">Properties for the columns.</param>
    private void CreateSchemaFromString( string typeSchema,
                                         IDictionary<string, IList<string>> properties = null)
    {
        // Create the avro schema from the string and save it
        try
        {
            m_data.schema = RecordSchema.Parse(KineticaData.NormalizeSchemaJson(typeSchema));
        }
        catch (Exception ex)
        {
            throw new KineticaException(ex.ToString());
        }

        var root = JObject.Parse(typeSchema);

        var rootType = root["type"];
        if ((null == rootType) || !rootType.ToString().Contains("record"))
        {
            throw new ArgumentException("Schema must be of type record.");
        }

        var fields = root["fields"];
        if ((null == fields) || !fields.HasValues)
        {
            throw new ArgumentException("Schema has no fields.");
        }

        foreach (var field in fields)
        {
            //if (!field->first.empty() || field->second.empty())
            //{
            //    throw std::invalid_argument("Schema has invalid field.");
            //}

            // Do NOT use ToString 'cause it includes the double quotes (turns it into a JSON representation)
            string? fieldName = (string?)field["name"];
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentException("Schema has unnamed field.");
            }

            if (m_data.columnMap.ContainsKey(fieldName))
            {
                throw new ArgumentException($"Duplicate field name {fieldName}.");
            }

            var fieldType = field["type"];
            if (null == fieldType)
            {
                throw new ArgumentException($"Field {fieldName} has no type.");
            }

            // Flag for nullability
            bool isColumnNullable = false;

            if (fieldType.HasValues) // If it has children
            {
                var fieldTypeArray = fieldType;

                foreach (var fieldTypeElement in fieldTypeArray.Children())
                {
                    bool valid = false;
                    //if (fieldTypeElement->first.empty())
                    {
                        var fieldTypeElementString = fieldTypeElement.ToString();

                        if (!string.IsNullOrEmpty(fieldTypeElementString))
                        {
                            if (fieldTypeElementString == "null" || fieldTypeElementString == "\"null\"")
                            {
                                isColumnNullable = true;
                                valid = true;
                            }
                            else //if (fieldType->empty())
                            {
                                fieldType = fieldTypeElement; // fieldTypeElementString;
                                valid = true;
                            }
                        }
                    }

                    if (!valid)
                    {
                        throw new ArgumentException("Field {fieldName} has invalid type.");
                    }
                }
            }

            Column.ColumnType columnType;

            if (fieldType.ToString().Equals("bytes") || fieldType.ToString().Equals("\"bytes\""))
            {
                columnType = Column.ColumnType.BYTES;
            }
            else if (fieldType.ToString().Equals("double") || fieldType.ToString().Equals("\"double\""))
            {
                columnType = Column.ColumnType.DOUBLE;
            }
            else if (fieldType.ToString().Equals("float") || fieldType.ToString().Equals("\"float\""))
            {
                columnType = Column.ColumnType.FLOAT;
            }
            else if (fieldType.ToString().Equals("int") || fieldType.ToString().Equals("\"int\""))
            {
                columnType = Column.ColumnType.INT;
            }
            else if (fieldType.ToString().Equals("long") || fieldType.ToString().Equals("\"long\""))
            {
                columnType = Column.ColumnType.LONG;
            }
            else if (fieldType.ToString().Equals("string") || fieldType.ToString().Equals("\"string\""))
            {
                columnType = Column.ColumnType.STRING;
            }
            else if (fieldType.ToString().Equals("boolean") || fieldType.ToString().Equals("\"boolean\""))
            {
                columnType = Column.ColumnType.BOOLEAN;
            }
            else
            {
                throw new ArgumentException("Field {fieldName} must be of type bytes, double, float, int, long, string or boolean.");
            }

            IList<string>? columnProperties = null;
            properties?.TryGetValue(fieldName, out columnProperties);
            // Check the column properties for nullability
            if ( ( null != columnProperties ) &&
                 ( columnProperties.Contains( ColumnProperty.NULLABLE ) ) )
                isColumnNullable = true;

            // Create the column to be added
            Column column = new( fieldName, columnType, columnProperties );

            column.setIsNullable( isColumnNullable );

            m_data.columns.Add( column );

            m_data.columnMap[fieldName] = m_data.columns.Count - 1;
        }
    }  // end CreateSchemaFromString()

    /// <summary>
    /// Create an avro schema from either the columns or the schema string.
    /// </summary>
    private void CreateSchema()
    {
        // First, check if the schema has already been created
        if (m_data.schema != null)
        {
            // nothing to do
            return;
        }

        // Check if the schema string exists, if so, create the schema from that
        if (m_data.schemaString != null)
        {
            try
            {
                m_data.schema = RecordSchema.Parse(KineticaData.NormalizeSchemaJson(m_data.schemaString));
                return;
            }
            catch (Exception ex)
            {
                throw new KineticaException(ex.ToString());
            }
        }  // done creating the schema from the schema string

        // Since the shortcuts didn't apply, create a JSON object from the columns
        // and then create the schema and the schema string off it
        // --------------------------------------------------------------------------
        // Create the json string for the type
        string schemaString = "";
        // Create the json string opening with empty fields (with a generic 'type_name' (because the
        // server always replaces the name with this string anyway) )
        string schemaOpening = "{'type':'record','name':'type_name','fields':[";
        // Create the json string closing
        string schemaClosing = "]}";

        schemaString += schemaOpening;

        // Create the json substrings for the columns
        foreach (var column in m_data.columns)
        {
            // Add the name
            string fieldName = ("'name':'" + column.getName() + "'");

            // Add the type
            string fieldType = "";
            if (column.isNullable())
            {  // the column is nullable, so we need a union
                fieldType = ("['" + column.getTypeString() + "','null']");
            }
            else  // regular type, no union needed
            {
                fieldType = ( "'" + column.getTypeString() + "'" );
            }
            fieldType = ("'type':" + fieldType);

            // Put the field together
            string field = ("{" + fieldName + "," + fieldType + "},");
            schemaString += field;
        }  // end looping over the fields

        // Trim the trailing comma from the fields
        char[] comma = [','];
        schemaString = schemaString.TrimEnd(comma);
        // Add the ending of the json string
        schemaString += schemaClosing;

        // Create the RecordSchema from the JSON string
        try
        {
            m_data.schema = RecordSchema.Parse(KineticaData.NormalizeSchemaJson(schemaString));
        }
        catch (Exception ex)
        {
            throw new KineticaException(ex.ToString());
        }

        // Save the schema string
        m_data.schemaString = m_data.schema.ToString();
        return;
    }  // end CreateSchema()
}  // end class KineticaType
