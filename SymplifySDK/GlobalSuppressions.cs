[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633: File must have header", Justification = "figuring out how to configure")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1636: Copyright XML must match config", Justification = "figuring out how to configure")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1000: The keyword 'new' should be followed by a space.", Justification = "does not agree with dotnet format")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649: File name should match first type name", Justification = "Doesn't seem to handle enums nicely")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("SonarLint", "S1135: Complete the task associated to this 'TODO' comment", Justification = "this is fine as long as we have a ticket tracking the task")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1101: Prefix local calls with this", Justification = "we don't think this is important")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1200: Using directive should appear within a namespace declaration", Justification = "we don't want this")]

// TODO(Fabian) The below suppressions were added when enabling StyleCop.
// We should either fix them one at a time, or suppress forever by moving the suppression to above the TODO.
// If there are no suppressions left below, remove this TODO.
// Organiziation
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1201: group fields, constructors, properties, members...", Justification = "Pending")] // 6
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1204: Static members should appear before ...", Justification = "Pending")] // 1

// Naming
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1306: private fields should begin with lower-case", Justification = "Pending")] // 2
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1309: Field '_xyz' should not begin with an underscore", Justification = "Pending")] // 1
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1310: Field 'X_Y_Z' should not contain an underscore", Justification = "Pending")] // 6

// Documentation
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Some.Builtin.Category.Name", "CS1591: Missing XML comment for publicly visible type or member", Justification = "Pending")] // 81
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600: Elements should be documented", Justification = "Pending")] // 72
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602: Enumeration items should be documented", Justification = "Pending")] // 9
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1615: Document return value", Justification = "Pending")] // 1
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1629: Documentation text should end with a period", Justification = "Pending")] // 6

// Remove when fixing the above doc issues
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA0001: XML comment analysis is disabled due to project configuration", Justification = "Pending")]
