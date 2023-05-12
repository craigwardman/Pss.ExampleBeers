Feature: Bars
Ability to create and manage bars

    Scenario: Create a bar
        When I send a request to create a bar called 'Bar Humbug' at address '123 Fake Street'
        Then I get a successful bars response
        And my new bar is given an ID

    Scenario: Get an existing bar
        Given the database contains a bar called 'Bar Humbug' at address '123 Fake Street'
        When I request the 'Bar Humbug' bar by it's ID
        Then I get a successful bars response
        And the returned bar is called 'Bar Humbug'

    Scenario: Update a bar
        Given the database contains a bar called 'Bar Humbug' at address '123 Fake Street'
        When I request to update the 'Bar Humbug' bar to be called 'Bar Tat' now at address '1 High Street'
        Then I get a successful bars response
        And the returned bar is called 'Bar Tat'

    Scenario: Get all bars
        Given the database contains several bars, as per below
          | Name       | Address         |
          | Bar Humbug | 123 Fake Street |
          | Bar Tat    | 1 High Street   |
        When I request all the bars
        Then the returned list of bars is as below
          | Name       | Address         |
          | Bar Humbug | 123 Fake Street |
          | Bar Tat    | 1 High Street   |
