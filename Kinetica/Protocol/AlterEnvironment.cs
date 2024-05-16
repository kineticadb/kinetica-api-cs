/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.alterEnvironment(string,string,string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Alters an existing environment which can be referenced by a <a
    /// href="../../../concepts/udf/" target="_top">user-defined function</a>
    /// (UDF).</summary>
    public class AlterEnvironmentRequest : KineticaData
    {

        /// <summary>Modification operation to be applied
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterEnvironmentRequest.Action.INSTALL_PACKAGE">INSTALL_PACKAGE</see>:</term>
        ///         <description>Install a python package from PyPI, an
        /// external data source or KiFS</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterEnvironmentRequest.Action.INSTALL_REQUIREMENTS">INSTALL_REQUIREMENTS</see>:</term>
        ///         <description>Install packages from a requirements
        /// file</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterEnvironmentRequest.Action.UNINSTALL_PACKAGE">UNINSTALL_PACKAGE</see>:</term>
        ///         <description>Uninstall a python package.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterEnvironmentRequest.Action.UNINSTALL_REQUIREMENTS">UNINSTALL_REQUIREMENTS</see>:</term>
        ///         <description>Uninstall packages from a requirements
        /// file</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterEnvironmentRequest.Action.RESET">RESET</see>:</term>
        ///         <description>Uninstalls all packages in the environment and
        /// resets it to the original state at time of creation</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterEnvironmentRequest.Action.REBUILD">REBUILD</see>:</term>
        ///         <description>Recreates the environment and re-installs all
        /// packages, upgrades the packages if necessary based on
        /// dependencies</description>
        ///     </item>
        /// </list>
        /// A set of string constants for the parameter <see cref="action"
        /// />.</summary>
        public struct Action
        {

            /// <summary>Install a python package from PyPI, an external data
            /// source or KiFS</summary>
            public const string INSTALL_PACKAGE = "install_package";

            /// <summary>Install packages from a requirements file</summary>
            public const string INSTALL_REQUIREMENTS = "install_requirements";

            /// <summary>Uninstall a python package.</summary>
            public const string UNINSTALL_PACKAGE = "uninstall_package";

            /// <summary>Uninstall packages from a requirements file</summary>
            public const string UNINSTALL_REQUIREMENTS = "uninstall_requirements";

            /// <summary>Uninstalls all packages in the environment and resets
            /// it to the original state at time of creation</summary>
            public const string RESET = "reset";

            /// <summary>Recreates the environment and re-installs all
            /// packages, upgrades the packages if necessary based on
            /// dependencies</summary>
            public const string REBUILD = "rebuild";
        } // end struct Action


        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterEnvironmentRequest.Options.DATASOURCE_NAME">DATASOURCE_NAME</see>:</term>
        ///         <description>Name of an existing external data source from
        /// which packages specified in <paramref
        /// cref="AlterEnvironmentRequest._value" /> can be
        /// loaded</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>Name of an existing external data source from which
            /// packages specified in <see cref="_value" /> can be
            /// loaded</summary>
            public const string DATASOURCE_NAME = "datasource_name";
        } // end struct Options


        /// <summary>Name of the environment to be altered.  </summary>
        public string environment_name { get; set; }

        /// <summary>Modification operation to be applied
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterEnvironmentRequest.Action.INSTALL_PACKAGE">INSTALL_PACKAGE</see>:</term>
        ///         <description>Install a python package from PyPI, an
        /// external data source or KiFS</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterEnvironmentRequest.Action.INSTALL_REQUIREMENTS">INSTALL_REQUIREMENTS</see>:</term>
        ///         <description>Install packages from a requirements
        /// file</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterEnvironmentRequest.Action.UNINSTALL_PACKAGE">UNINSTALL_PACKAGE</see>:</term>
        ///         <description>Uninstall a python package.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterEnvironmentRequest.Action.UNINSTALL_REQUIREMENTS">UNINSTALL_REQUIREMENTS</see>:</term>
        ///         <description>Uninstall packages from a requirements
        /// file</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterEnvironmentRequest.Action.RESET">RESET</see>:</term>
        ///         <description>Uninstalls all packages in the environment and
        /// resets it to the original state at time of creation</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterEnvironmentRequest.Action.REBUILD">REBUILD</see>:</term>
        ///         <description>Recreates the environment and re-installs all
        /// packages, upgrades the packages if necessary based on
        /// dependencies</description>
        ///     </item>
        /// </list>  </summary>
        public string action { get; set; }

        /// <summary>The value of the modification, depending on <paramref
        /// cref="AlterEnvironmentRequest.action" />.  For example, if
        /// <paramref cref="AlterEnvironmentRequest.action" /> is
        /// <i>install_package</i>, this would be the python package name.
        /// <br />
        /// If <paramref cref="AlterEnvironmentRequest.action" /> is
        /// <i>install_requirements</i>, this would be the path of a
        /// requirements file from which to install packages.
        /// <br />
        /// If an external data source is specified in <i>datasource_name</i>,
        /// this can be the path to a wheel file or source archive.
        /// Alternatively, if installing from a file (wheel or source archive),
        /// the value may be a reference to a file in <a
        /// href="../../../tools/kifs/" target="_top">KiFS</a>.  </summary>
        public string _value { get; set; }

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterEnvironmentRequest.Options.DATASOURCE_NAME">DATASOURCE_NAME</see>:</term>
        ///         <description>Name of an existing external data source from
        /// which packages specified in <paramref
        /// cref="AlterEnvironmentRequest._value" /> can be
        /// loaded</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs an AlterEnvironmentRequest object with default
        /// parameters.</summary>
        public AlterEnvironmentRequest() { }

        /// <summary>Constructs an AlterEnvironmentRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="environment_name">Name of the environment to be
        /// altered.  </param>
        /// <param name="action">Modification operation to be applied
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterEnvironmentRequest.Action.INSTALL_PACKAGE">INSTALL_PACKAGE</see>:</term>
        ///         <description>Install a python package from PyPI, an
        /// external data source or KiFS</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterEnvironmentRequest.Action.INSTALL_REQUIREMENTS">INSTALL_REQUIREMENTS</see>:</term>
        ///         <description>Install packages from a requirements
        /// file</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterEnvironmentRequest.Action.UNINSTALL_PACKAGE">UNINSTALL_PACKAGE</see>:</term>
        ///         <description>Uninstall a python package.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterEnvironmentRequest.Action.UNINSTALL_REQUIREMENTS">UNINSTALL_REQUIREMENTS</see>:</term>
        ///         <description>Uninstall packages from a requirements
        /// file</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterEnvironmentRequest.Action.RESET">RESET</see>:</term>
        ///         <description>Uninstalls all packages in the environment and
        /// resets it to the original state at time of creation</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterEnvironmentRequest.Action.REBUILD">REBUILD</see>:</term>
        ///         <description>Recreates the environment and re-installs all
        /// packages, upgrades the packages if necessary based on
        /// dependencies</description>
        ///     </item>
        /// </list>  </param>
        /// <param name="_value">The value of the modification, depending on
        /// <paramref cref="AlterEnvironmentRequest.action" />.  For example,
        /// if <paramref cref="AlterEnvironmentRequest.action" /> is
        /// <i>install_package</i>, this would be the python package name.
        /// If <paramref cref="AlterEnvironmentRequest.action" /> is
        /// <i>install_requirements</i>, this would be the path of a
        /// requirements file from which to install packages.
        /// If an external data source is specified in <i>datasource_name</i>,
        /// this can be the path to a wheel file or source archive.
        /// Alternatively, if installing from a file (wheel or source archive),
        /// the value may be a reference to a file in <a
        /// href="../../../tools/kifs/" target="_top">KiFS</a>.  </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterEnvironmentRequest.Options.DATASOURCE_NAME">DATASOURCE_NAME</see>:</term>
        ///         <description>Name of an existing external data source from
        /// which packages specified in <paramref
        /// cref="AlterEnvironmentRequest._value" /> can be
        /// loaded</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public AlterEnvironmentRequest( string environment_name,
                                        string action,
                                        string _value,
                                        IDictionary<string, string> options = null)
        {
            this.environment_name = environment_name ?? "";
            this.action = action ?? "";
            this._value = _value ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class AlterEnvironmentRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.alterEnvironment(string,string,string,IDictionary{string, string})"
    /// />.</summary>
    public class AlterEnvironmentResponse : KineticaData
    {

        /// <summary>Value of <paramref
        /// cref="AlterEnvironmentRequest.environment_name" />.  </summary>
        public string environment_name { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class AlterEnvironmentResponse




}  // end namespace kinetica
