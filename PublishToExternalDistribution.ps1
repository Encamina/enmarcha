# Restores, builds and packs as NuGet packages every Solution's project.
# Every NuGet package created by this script is pushed into the user's profile directory, in another directory called ".nuget.local".

dotnet pack -p:IncludeSymbols=false -p:SourceLinkCreate=false -p:GenerateDocumentationFile=False -p:PublishRepositoryUrl=False -o $env:USERPROFILE\.nuget.local 
