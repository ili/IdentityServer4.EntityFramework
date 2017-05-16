﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LinqToDB;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyProduct         (LinqToDBConstants.ProductName)]
[assembly: AssemblyCulture         ("")]

[assembly: System.Security.AllowPartiallyTrustedCallers]

[assembly: InternalsVisibleTo("IdentityServer4.LinqToDB.UnitTests")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("7069c5d0-6596-4090-85bc-a2b9870c7df4")]