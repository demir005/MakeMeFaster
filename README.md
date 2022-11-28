# MakeMeFaster

## Welcome!

# Steps

- Before you run an application, you will need to create a database (using MSSQL) named "MakeMeFaster"
- Go to Program.cs class
- "IWillPopulateDate()" is a method which will get a script from the project directory and run in on created DB
- Run application in Debug/Release mode
- Comment or delete PopulateDbData() call from Main method
- Go to BenchmarkService.cs class
- Start coding within GetAuthors_Optimized method

# How do I submit my solution?

- Clone the project, create a branch and work on that branch. Include a link to that branch in your submission email.

# Rules

- Only Entity Framework (Core) is allowed for using
- The data obtained in the non-optimized version of the code must also be obtained in the optimized version
- If you see potential optimization of something else, you can do it
- Entities and DbContext cannot be changed
- The models returned from the method can be changed

# What should the method return?

- Given that there is a predefined database of data, the method should in any case return the list of data currently returned by the non-optimized method.

# How will performance success be measured?

- The thought process followed to optimize, as well as the execution time quotient of the non-optimized and optimized method will be checked.

Example:

Non-optimized method Execution time: 1.2s = 1200ms
Optimized method Execution time: 100ms

### Result: 1200ms/100ms = 12x faster.

In addition, I will check the performance on my computer for individual results.

# GOOD LUCK!
