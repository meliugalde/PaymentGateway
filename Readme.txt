PaymentGateway Api is divided in 3 layers (API, Domain, Infrastructure) following the principles of clean architecture.

There are some considerations and pending refactor or improvements due to time constraints. 

Domain:
	- Models are separate by request and responses and at the moment it is using the same models through all the layers. 
	I kept it like that due to time constraints and because the same properties that are being returned to the user are the same as are being saved in the database
	However different layers should deal with different objects. There is an example of a Payment entity that should be used by the repository.

	- Currency is being used as a string, an improvement would to have an enum with the kind of currencies accepted
	- PostPaymentRequest has validation as DataAnnotation some are implemented as regular expression and there are some 
		validation in the helper (still some work to do in this area)
	- The amount validation is pending because I wasn't sure about the requirement
	
Infrastructure:
   - Unit tests are pending in PaymentRepository 
   - An improvement would be to implemente the method add and get in the payment repository async 
	
All the services should have the same logging mechanism as it's implemented in PaymentController	
	
