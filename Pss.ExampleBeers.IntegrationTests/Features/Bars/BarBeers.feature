Feature: BarBeers
Bars can be linked with beers that they sell.

    Scenario: When bar and beer exist, link request succeeds
        Given the database contains a bar called 'Bar Tat' at address 'Some Street'
        And the database contains a beer called 'Pragmabrew' that is '8%' by volume
        When I send a request to create link 'Pragmabrew' with bar 'Bar Tat'
        Then I get a successful bars response

    Scenario: When bar does not exist, link request fails
        Given the database contains a beer called 'Pragmabrew' that is '8%' by volume
        When I send a request to create link 'Pragmabrew' with bar 'Dog and Gun'
        Then I get an unsuccessful bars response, with status 'BadRequest'

    Scenario: When beer does not exist, link request fails
        Given the database contains a bar called 'Bar Tat' at address 'Some Street'
        When I send a request to create link 'Special Brew' with bar 'Bar Tat'
        Then I get an unsuccessful bars response, with status 'BadRequest'

    Scenario: When a bar has linked beers, bar with beers can be retrieved
        Given the below bar/beer links exist in the database
          | Bar         | Beer       |
          | Bar Tat     | Pragmabrew |
          | Bar Tat     | Golden Hen |
          | Dog and Gun | Pragmabrew |
        When I request the 'Bar Tat' bar beers
        Then the returned 'Bar Tat' bar beers list is 'Pragmabrew, Golden Hen'

    Scenario: When several bars have linked beers, all bars with beers can be retrieved
        Given the below bar/beer links exist in the database
          | Bar         | Beer       |
          | Bar Tat     | Pragmabrew |
          | Bar Tat     | Golden Hen |
          | Dog and Gun | Pragmabrew |
        When I request the all bar beers
        Then the returned bar beers list is below
          | Bar         | Beers                  |
          | Bar Tat     | Pragmabrew, Golden Hen |
          | Dog and Gun | Pragmabrew             |