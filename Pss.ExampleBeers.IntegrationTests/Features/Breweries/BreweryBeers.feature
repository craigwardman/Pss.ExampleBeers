Feature: BreweryBeers
Breweries can be linked with beers that they sell.

    Scenario: When brewery and beer exist, link request succeeds
        Given the database contains a brewery called 'BrewHQ'
        And the database contains a beer called 'Pragmabrew' that is '8%' by volume
        When I send a request to create link 'Pragmabrew' with brewery 'BrewHQ'
        Then I get a successful breweries response

    Scenario: When brewery does not exist, link request fails
        Given the database contains a beer called 'Pragmabrew' that is '8%' by volume
        When I send a request to create link 'Pragmabrew' with brewery 'Mega Brew'
        Then I get an unsuccessful breweries response, with status 'BadRequest'

    Scenario: When beer does not exist, link request fails
        Given the database contains a brewery called 'BrewHQ'
        When I send a request to create link 'Pragmabrew' with brewery 'BreqHQ'
        Then I get an unsuccessful breweries response, with status 'BadRequest'

    Scenario: When a brewery has linked beers, brewery with beers can be retrieved
        Given the below brewery/beer links exist in the database
          | Brewery   | Beer       |
          | BrewHQ    | Pragmabrew |
          | BrewHQ    | Golden Hen |
          | Mega Brew | Pragmabrew |
        When I request the 'BrewHQ' brewery beers
        Then the returned 'BrewHQ' brewery beers list is 'Pragmabrew, Golden Hen'

    Scenario: When several breweries have linked beers, all breweries with beers can be retrieved
        Given the below brewery/beer links exist in the database
          | Brewery   | Beer       |
          | BrewHQ    | Pragmabrew |
          | BrewHQ    | Golden Hen |
          | Mega Brew | Pragmabrew |
        When I request the all brewery beers
        Then the returned brewery beers list is below
          | Brewery   | Beers                  |
          | BrewHQ    | Pragmabrew, Golden Hen |
          | Mega Brew | Pragmabrew             |