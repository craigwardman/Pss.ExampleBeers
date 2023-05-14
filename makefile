.PHONY: run
.PHONY: build
.PHONY: test
.PHONY: test-real
.PHONY: run-api
.PHONY: run-containers
.PHONY: stop-containers

# run the api
run-api: run-containers
	cd Pss.ExampleBeers.Api && dotnet run

run-containers:
	docker compose up -d

stop-containers:
	docker compose down

# run everything
run:
	@$(MAKE) -j 1 run-api
	
build:
	dotnet build --configuration Release
	
test:
	dotnet test
	
test-real: stop-containers
	dotnet test Pss.ExampleBeers.IntegrationTests/Pss.ExampleBeers.IntegrationTests.csproj -c=Debug-RealProviderMode