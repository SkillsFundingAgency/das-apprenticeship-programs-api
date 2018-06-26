Feature: Standards
    Get ALL the Standards

Scenario Outline: Verify correct status code is returned
    Given I send request to <uri>
    Then I get response code <code> is returned

	Examples:
	| uri                                        | code     | 
	| standards                                  | OK       |
	| standards/1                                | OK       |

Scenario: Verify correct amount of results returned
	Given I send request to standards
	Then I expect the amount of refult will be at least 200