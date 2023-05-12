Feature: Breweries
Ability to create and manage breweries

    Scenario: Create a brewery
        When I send a request to create a brewery called 'BrewHQ'
        Then I get a successful breweries response
        And my new brewery is given an ID

    Scenario: Get an existing brewery
        Given the database contains a brewery called 'BrewHQ'
        When I request the 'BrewHQ' brewery by it's ID
        Then I get a successful breweries response
        And the returned brewery is called 'BrewHQ'

    Scenario: Update a brewery
        Given the database contains a brewery called 'BrewHQ'
        When I request to update the 'BrewHQ' brewery to be called 'Mega Brewer'
        Then I get a successful breweries response
        And the returned brewery is called 'Mega Brewer'

    Scenario: Get all breweries
        Given the database contains several breweries, as per below
          | Name        |
          | BrewHQ      |
          | Mega Brewer |
        When I request all the breweries
        Then the returned list of breweries is as below
          | Name        |
          | BrewHQ      |
          | Mega Brewer |
