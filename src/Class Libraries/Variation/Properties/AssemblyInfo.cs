using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

[assembly: CLSCompliant(true)]
[assembly: AssemblyDefaultAlias("Cavity.Variation.dll")]
[assembly: AssemblyTitle("Cavity.Variation.dll")]

#if (DEBUG)

[assembly: AssemblyDescription("Cavity : Variation Library (Debug)")]

#else

[assembly: AssemblyDescription("Cavity : Variation Library (Release)")]

#endif

[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Cavity", Justification = "This is a root namespace.")]