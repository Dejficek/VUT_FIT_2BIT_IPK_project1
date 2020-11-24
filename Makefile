PORT?=1234
build: IPK-sniffer.csproj Program.cs
	cd src && dotnet build

run:
	cd src && dotnet run -- $(PORT)