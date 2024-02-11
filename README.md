<h3 align="center" style="margin:0px">
	<img width="200" src="https://www.kinetica.com/wp-content/uploads/2018/08/kinetica_logo.svg" alt="Kinetica Logo"/>
</h3>
<h5 align="center" style="margin:0px">
	<a href="https://www.kinetica.com/">Website</a>
	|
	<a href="https://docs.kinetica.com/7.2/">Docs</a>
	|
	<a href="https://docs.kinetica.com/7.2/api/csharp/">API Docs</a>
	|
	<a href="https://join.slack.com/t/kinetica-community/shared_invite/zt-1bt9x3mvr-uMKrXlSDXfy3oU~sKi84qg">Community Slack</a>   
</h5>


# Kinetica C# API

-  [Overview](#overview)
-  [Requirements](#requirements)
-  [Building the API on Windows](#building-the-api-on-windows)
-  [Support](#support)
-  [Contact Us](#contact-us)
 

## Overview

This project contains the source code of the C# Kinetica API.

The documentation can be found at https://docs.kinetica.com/7.2/.
The API-specific documentation can be found at:

* https://docs.kinetica.com/7.2/api/csharp/


For changes to the client-side API, please refer to
[CHANGELOG.md](CHANGELOG.md).  For
changes to Kinetica functions, please refer to
[CHANGELOG-FUNCTIONS.md](CHANGELOG-FUNCTIONS.md).


## Requirements

The C# API requires the Json.NET package:

    https://www.newtonsoft.com/json


## Building the API on Windows

Building the API within Visual Studio is a simple process:

Open the *NuGet Package Manager Console*, under
*Tools > NuGet Package Manager > Package Manager Console*

At the command prompt, install the Json.NET package:

```
install-package Newtonsoft.Json
```

Use Visual Studio to compile the API.


## Support

For bugs, please submit an
[issue on Github](https://github.com/kineticadb/kinetica-api-cs/issues).

For support, you can post on
[stackoverflow](https://stackoverflow.com/questions/tagged/kinetica) under the
``kinetica`` tag or
[Slack](https://join.slack.com/t/kinetica-community/shared_invite/zt-1bt9x3mvr-uMKrXlSDXfy3oU~sKi84qg).


## Contact Us

* Ask a question on Slack:
  [Slack](https://join.slack.com/t/kinetica-community/shared_invite/zt-1bt9x3mvr-uMKrXlSDXfy3oU~sKi84qg)
* Follow on GitHub:
  [Follow @kineticadb](https://github.com/kineticadb) 
* Email us:  <support@kinetica.com>
* Visit:  <https://www.kinetica.com/contact/>
