# MSBuild Blacklist

A simple MSBuild task to check your project's dependencies against a _blacklist_ and fail if any matches are found.

## Why?

1. Prevent packages with breaking features/bugs from being referenced in your project (includes transitive dependencies)
2. Prevent packages with strict usage licences from being installed (e.g. AGPL)
3. Prevent unwanted project references

## Installation

Install the [Nuget Package](https://www.nuget.org/packages/Cats.Build.Blacklist) into the project(s) that need a blacklist configured.

```
    Install-Package Cats.Build.Blacklist
```

## Usage

### NuGet Packages

To configure the package blacklist create a text file in the root of the project directory `package.blacklist`. Add each package ID you wish to blacklist on a newline. Optionally, you can add a NuGet [version range](https://docs.microsoft.com/en-us/nuget/concepts/package-versioning#version-ranges) which should be on the same line as the ID separated by a forward slash (`/`). Comments can be added by prefixing with a hash (`#`).

```
# This is a comment
iTextSharp # Blacklist all versions of iTextSharp due to AGPL license
Newtonsoft.Json/[6.0.1,7.0.0) # Blacklist versions 6.0.1 <= x < 7.0.0
```

Add the package ID multiple times to perform a logical _OR_ between the rules.

```
Newtonsoft.Json/[6.0.1] # Blacklist versions 6.0.1
Newtonsoft.Json/[9.0.0] # Blacklist versions 9.0.0 (i.e. with above rule, 6.0.1 or 9.0.0 will be blacklisted)
```

Trying to build the project will result in an error if any of the blacklisted packages are found.

### Project References

If you want to blacklist a project reference (e.g. to stop a business logic project referencing the UI), then create a text file in the project directory `project.blacklist`. It works the same as above, with the project names on separate lines.

```
MyAwesomeApp.UI # Blacklist UI reference from business logic project
```

### How it works?

The build task reads the generated `project.assets.json` file that is usually found in the `obj` folder. This is a map of all the project's dependencies, which is used to determine if there are any matches on the blacklist.

You can override the any file paths used by the task by setting a property group in your `csproj`.

```xml
<PropertyGroup>
  <CatsProjectAssetsFilePath>custom\path\file.json</CatsProjectAssetsFilePath>
  <CatsPackageBlacklistFilePath>custom\path\file.blacklist</CatsPackageBlacklistFilePath>
  <CatsProjectBlacklistFilePath>custom\path\file.blacklist</CatsProjectBlacklistFilePath>
</PropertyGroup>
```