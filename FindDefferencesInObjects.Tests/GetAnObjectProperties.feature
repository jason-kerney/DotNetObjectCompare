Feature: GetAnObjectProperties
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

Scenario: Get a list of properties from an object
	Given the 'expected' student object with an Age of 40 and a Class of 'Math'
	When I get a list of the 'expected' properties
	Then the 'Simple' list of PropertyInfo objects is retrieved

Scenario: Get all readable properties from an object
	Given the 'expected' object under test with the following values
		| StringProperty | IntProperty | ReadonlyProperty | PrivateProperty | WriteOnlyProperty |
		| StringProperty | 34          | true             | Not So Private  | Write Only        |
	When I get a list of the 'expected' properties
	Then the 'Complex' list of PropertyInfo objects is retrieved
