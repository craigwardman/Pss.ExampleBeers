Feature: Beers
Ability to create and manage beers

    Scenario: Create a beer
        When I send a request to create a beer called 'Pragmabrew' that is '8%' by volume
        Then I get a successful beers response
        And my new beer is given an ID

    Scenario: Get an existing beer
        Given the database contains a beer called 'Pragmabrew' that is '8%' by volume
        When I request the 'Pragmabrew' beer by it's ID
        Then I get a successful beers response
        And the returned beer is called 'Pragmabrew' that is '8%' by volume

    Scenario: Update a beer
        Given the database contains a beer called 'Pragmabrew' that is '8%' by volume
        When I request to update the 'Pragmabrew' beer to be called 'Craig Dog' that is '6%' by volume
        Then I get a successful beers response
        And the returned beer is called 'Craig Dog' that is '6%' by volume

    Scenario: Get all beers
        Given the database contains several beers, as per below
          | Name       | PercentageAlcoholByVolume |
          | Pragmabrew | 8%                        |
          | Craig Dog  | 7%                        |
        When I request all the beers
        Then the returned list of beers is as below
          | Name       | PercentageAlcoholByVolume |
          | Pragmabrew | 8%                        |
          | Craig Dog  | 7%                        |

    Scenario: Get beers within an alcoholic volume range
        Given the database contains several beers, as per below
          | Name              | PercentageAlcoholByVolume |
          | Pragmabrew        | 8%                        |
          | Craig Dog         | 7%                        |
          | Golden Duck       | 6%                        |
          | Old Speckled Frog | 5%                        |
        When I request the beers between greater than '5%' and less than '8%' volume
        Then the returned list of beers is as below
          | Name        | PercentageAlcoholByVolume |
          | Craig Dog   | 7%                        |
          | Golden Duck | 6%                        |