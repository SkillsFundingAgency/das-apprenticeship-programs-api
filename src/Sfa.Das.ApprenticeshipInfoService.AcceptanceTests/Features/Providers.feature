Feature: Providers
    Get ALL the Providers


Scenario Outline: Verify correct status code is returned
    Given I send request to <uri>
    Then I get response code <code> is returned

	Examples:
	| uri                                        | code     |                                
	| providers                                  | OK       |
	| providers/10000020                         | OK       |

Scenario: Verify correct amount of results returned
	Given I send request to providers
	Then I expect the amount of refult will be at least 2000