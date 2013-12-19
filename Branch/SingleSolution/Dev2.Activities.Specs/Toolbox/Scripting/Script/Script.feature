﻿Feature: Script
	In order to execute scripts
	As a Warewolf user
	I want a tool that allows me to execute javascripts, ruby or python 

Scenario: Execute Javascript Variable is 1
	Given I have a script variable "[[val]]" with this value "1"
	And I have this script to execute "javascript_one_variable.txt"
	And I have selected the language as "JavaScript"
	When I execute the script tool
	Then the script result should be "one"
	And script execution has "NO" error

Scenario: Execute Javascript blank script	
	Given I have this script to execute ""
	And I have selected the language as "JavaScript"
	When I execute the script tool
	Then the script result should be ""
	And script execution has "AN" error

Scenario: Execute Javascript Variable is 2
	Given I have a script variable "[[val]]" with this value "2"
	And I have this script to execute "javascript_one_variable.txt"
	And I have selected the language as "JavaScript"
	When I execute the script tool
	Then the script result should be "two"
	And script execution has "NO" error

Scenario: Execute Javascript Variable is 3
	Given I have a script variable "[[val]]" with this value "3"
	And I have this script to execute "javascript_one_variable.txt"
	And I have selected the language as "JavaScript"
	When I execute the script tool
	Then the script result should be "not one or two"
	And script execution has "NO" error

Scenario: Execute Javascript Variable is 100
	Given I have a script variable "[[val]]" with this value "100"
	And I have this script to execute "javascript_one_variable.txt"
	And I have selected the language as "JavaScript"
	When I execute the script tool
	Then the script result should be "not one or two"
	And script execution has "NO" error

Scenario: Execute badly formed Javascript
	Given I have a script variable "[[val]]" with this value "1"
	And I have this script to execute "javascript_badly_formatted.txt"
	And I have selected the language as "JavaScript"
	When I execute the script tool
	Then script execution has "AN" error
	
Scenario: Execute Javascript with 2 variables
	Given I have a script variable "[[val1]]" with this value "1"
	Given I have a script variable "[[val2]]" with this value "1"
	And I have this script to execute "javascript_two_variables.txt"
	And I have selected the language as "JavaScript"
	When I execute the script tool
	Then the script result should be "two"
	And script execution has "NO" error

Scenario: Execute Python Variable is 1
	Given I have a script variable "[[val]]" with this value "1"
	And I have this script to execute "python_one_variable.txt"
	And I have selected the language as "Python"
	When I execute the script tool
	Then the script result should be "one"
	And script execution has "NO" error

Scenario: Execute Python blank script	
	Given I have this script to execute ""
	And I have selected the language as "Python"
	When I execute the script tool
	Then the script result should be ""
	And script execution has "AN" error

Scenario: Execute Python Variable is 2
	Given I have a script variable "[[val]]" with this value "2"
	And I have this script to execute "python_one_variable.txt"
	And I have selected the language as "Python"
	When I execute the script tool
	Then the script result should be "two"
	And script execution has "NO" error

Scenario: Execute Python Variable is 3
	Given I have a script variable "[[val]]" with this value "3"
	And I have this script to execute "python_one_variable.txt"
	And I have selected the language as "Python"
	When I execute the script tool
	Then the script result should be "not one or two"
	And script execution has "NO" error

Scenario: Execute Python Variable is 100
	Given I have a script variable "[[val]]" with this value "100"
	And I have this script to execute "python_one_variable.txt"
	And I have selected the language as "Python"
	When I execute the script tool
	Then the script result should be "not one or two"
	And script execution has "NO" error

Scenario: Execute badly formed Python
	Given I have a script variable "[[val]]" with this value "1"
	And I have this script to execute "python_badly_formatted.txt"
	And I have selected the language as "Python"
	When I execute the script tool
	Then script execution has "AN" error
	
Scenario: Execute Python with 2 variables
	Given I have a script variable "[[val1]]" with this value "1"
	Given I have a script variable "[[val2]]" with this value "1"
	And I have this script to execute "python_two_variables.txt"
	And I have selected the language as "Python"
	When I execute the script tool
	Then the script result should be "two"
	And script execution has "NO" error

Scenario: Execute Ruby Variable is 1
	Given I have a script variable "[[val]]" with this value "1"
	And I have this script to execute "ruby_one_variable.txt"
	And I have selected the language as "Ruby"
	When I execute the script tool
	Then the script result should be "one"
	And script execution has "NO" error

Scenario: Execute Ruby blank script	
	Given I have this script to execute ""
	And I have selected the language as "Ruby"
	When I execute the script tool
	Then the script result should be ""
	And script execution has "AN" error

Scenario: Execute Ruby Variable is 2
	Given I have a script variable "[[val]]" with this value "2"
	And I have this script to execute "ruby_one_variable.txt"
	And I have selected the language as "Ruby"
	When I execute the script tool
	Then the script result should be "two"
	And script execution has "NO" error

Scenario: Execute Ruby Variable is 3
	Given I have a script variable "[[val]]" with this value "3"
	And I have this script to execute "ruby_one_variable.txt"
	And I have selected the language as "Ruby"
	When I execute the script tool
	Then the script result should be "not one or two"
	And script execution has "NO" error

Scenario: Execute Ruby Variable is 100
	Given I have a script variable "[[val]]" with this value "100"
	And I have this script to execute "ruby_one_variable.txt"
	And I have selected the language as "Ruby"
	When I execute the script tool
	Then the script result should be "not one or two"
	And script execution has "NO" error

Scenario: Execute badly formed Ruby
	Given I have a script variable "[[val]]" with this value "1"
	And I have this script to execute "ruby_badly_formatted.txt"
	And I have selected the language as "Ruby"
	When I execute the script tool
	Then script execution has "AN" error
	
Scenario: Execute Ruby with 2 variables
	Given I have a script variable "[[val1]]" with this value "1"
	Given I have a script variable "[[val2]]" with this value "1"
	And I have this script to execute "ruby_two_variables.txt"
	And I have selected the language as "Ruby"
	When I execute the script tool
	Then the script result should be "two"
	And script execution has "NO" error

Scenario: Execute Ruby with a negative recordset index
	Given I have this script to execute "[[my(-1).val]]"
	And I have selected the language as "Ruby"
	When I execute the script tool
	Then script execution has "AN" error

Scenario: Execute JavaScript with a negative recordset index
	Given I have this script to execute "[[my(-1).val]]"
	And I have selected the language as "JavaScript"
	When I execute the script tool
	Then script execution has "AN" error

Scenario: Execute Python with a negative recordset index
	Given I have this script to execute "[[my(-1).val]]"
	And I have selected the language as "Python"
	When I execute the script tool
	Then script execution has "AN" error