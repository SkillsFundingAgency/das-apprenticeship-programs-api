Feature: AssessmentOrganisations
    Get ALL the AssessmentOrganisations


Scenario Outline: Verify correct status code is returned
   Given I send request to <uri>
   Then I get response code <code> is returned

Examples:
| uri                                        | code     |                                
| assessment-organisations                   | OK       |
| assessment-test                            | NotFound |
| assessment-organisations/EPA0001           | OK       |
| assessment-organisations/standards/152     | OK       |
| assessment-organisations/EPA0001/standards | OK       |
| assessment-organisations/EPA1001           | NotFound |
| assessmentorganisations/                   | NotFound |