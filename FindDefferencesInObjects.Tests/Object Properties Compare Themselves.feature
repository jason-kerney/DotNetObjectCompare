Feature: Object Properties Compare Themselves
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

@PassingResult
Scenario: Two Exact Properties Compared
	Given Two ObjectProperties with a name of 'Age'
	And a value of 50
	When One is used to compare the other
	Then the result should be SameResult

@FailingResult
Scenario: Two Different Valued Property Objects
	Given Two ObjectProperties with a name of 'Distance'
	And the 'first' having a value of 50
	And the 'second' having a value of 25
	When the 'first' is compared to the 'second'
	Then the result should be DifferentResult('Distance', '50', '25')

@WrongName
Scenario: Two Different Named Property Objects
	Given the 'first' ObjectProperty with a name of 'Age'
	And the 'second' ObjectProperty with a name of 'Distance'
	And the 'first' has a value of 50
	And the 'second' has a value of 50
	When the 'first' is compared to the 'second' 
	Then the result should be DifferentNameResult('Age', 'Distance')