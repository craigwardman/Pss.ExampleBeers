.PHONY: run
.PHONY: build
.PHONY: run-api
.PHONY: run-containers

# run the api
run-api: run-containers
	cd Pss.ExampleBeers.Api && dotnet run

run-containers:
	docker compose up -d

# run everything
run:
	@$(MAKE) -j 1 run-api
	
build:
	dotnet build --configuration Release