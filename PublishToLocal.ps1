# Restores, builds and packs as NuGet packages every Solution's project.
# Every NuGet package created by this script is pushed into the user's profile
# directory, in another directory called ".nuget.local".

dotnet pack -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg -p:SourceLinkCreate=true -o $env:USERPROFILE\.nuget.local 
