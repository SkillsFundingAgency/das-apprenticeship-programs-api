Feature: Frameworks
    Get ALL the Frameworks


Scenario Outline: Verify correct status code is returned
    Given I send request to <uri>
    Then I get response code <code> is returned

	Examples:
	| uri                                        | code     |                                
	| frameworks                                 | OK       |
	| frameworks/403-2-1                         | OK       |
	| frameworks/codes                           | OK       |
	| frameworks/codes/403                       | OK       |
	| frameworks/403-2-6                         | OK       | 
	| frameworks/405-2-1                         | OK       |
	| frameworks/409-2-1                         | OK       |
	| frameworks/473-2-1                         | OK       |
	| frameworks/477-2-1                         | OK       |
	| frameworks/499-3-1                         | OK       |
	| frameworks/506-2-1                         | OK       |
	| frameworks/506-2-2                         | OK       |
	| frameworks/532-2-1                         | OK       |
	| frameworks/475-2-1                         | OK       |
	| frameworks/427-3-1                         | OK       |
	| frameworks/506-3-1                         | OK       |
	| frameworks/473-3-1                         | OK       |
	| frameworks/506-3-2                         | OK       |
	| frameworks/513-2-1                         | OK       |
	| frameworks/602-21-1                        | OK       |
	| frameworks/479-2-1                         | OK       |
	| frameworks/473-2-1                         | OK       |

Scenario: Verify correct amount of results returned
	Given I send request to frameworks
	Then I expect the amount of refult will be at least 200