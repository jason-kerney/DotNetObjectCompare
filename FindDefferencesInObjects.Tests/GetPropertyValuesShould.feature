Feature: Evaluate the properties of objects
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

Scenario: Given an object get the values of its properties
	Given the 'expected' object under test with
		| StringProperty | IntProperty | ReadonlyProperty | PrivateProperty | WriteOnlyProperty |
		| StringProperty | 34          | true             | Not So Private  | Write Only        |
	When I contruct a PropertyValues object from the 'expected'
	Then the 'expected' result should be a verifiable list of properties
