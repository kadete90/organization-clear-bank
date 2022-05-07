[![Build](https://github.com/kadete90/organization-clear-bank/actions/workflows/build.yml/badge.svg?branch=main&event=push)](https://github.com/kadete90/organization-clear-bank/actions/workflows/build.yml)
[![Coverage Status](https://coveralls.io/repos/github/kadete90/organization-clear-bank/badge.svg?branch=main)](https://coveralls.io/github/kadete90/organization-clear-bank?branch=main)

_Reduced % due to no Unit Tests for IDataStore dummy implementations_

![image](https://user-images.githubusercontent.com/8395639/167251148-f1148065-1cab-40bf-8a35-c7c7d779cb4d.png)
![image](https://user-images.githubusercontent.com/8395639/167251122-e5f6d009-c2c8-4619-9632-f7ac6b3deda2.png)

### Test Description
In the 'PaymentService.cs' file you will find a method for making a payment. At a high level the steps for making a payment are:

 - Lookup the account the payment is being made from
 - Check the account is in a valid state to make the payment
 - Deduct the payment amount from the account's balance and update the account in the database
 
What we’d like you to do is refactor the code with the following things in mind:  
 - Adherence to SOLID principals
 - Testability  
 - Readability 

We’d also like you to add some unit tests to the ClearBank.DeveloperTest.Tests project to show how you would test the code that you’ve produced. The only specific ‘rules’ are:  

 - The solution should build.
 - The tests should all pass.
 - You should not change the method signature of the MakePayment method.

You are free to use any frameworks/NuGet packages that you see fit.  
 
You should plan to spend around an hour completing the exercise. 
