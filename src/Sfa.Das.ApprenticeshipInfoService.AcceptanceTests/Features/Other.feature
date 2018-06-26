Feature: Other
    Get ALL the Other


Scenario Outline: Verify correct status code is returned
   Given I send request to <uri>
   Then I get response code <code> is returned

	Examples:
	| uri                       | code |
	| apprenticeship-programmes | OK   |
	| health                    | OK   |

Scenario: Verify correct amount of results returned
	Given I send request to apprenticeship-programmes
	Then I expect the amount of refult will be at least 800