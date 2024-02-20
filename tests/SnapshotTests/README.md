This project needs to multi-target because it references Vogen.SharedTypes as a metadata
reference. 
We want to snapshot test multiple frameworks, e.g. NET461, netstandard2.0, netcoreapp2.0, 
all the way up to net8.0. But we don't want to 