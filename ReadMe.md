[![Build](https://github.com/kadete90/organization-clear-bank/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/kadete90/organization-clear-bank/actions/workflows/build.yml)

![image](https://user-images.githubusercontent.com/8395639/167250478-61422c88-1ebb-402e-ac8f-3fb3c9015fc9.png)

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
