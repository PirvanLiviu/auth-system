# auth-system

Simple auth system, backend only (who needs frontend when you can make api calls to the databse), that can create a user by storing user's data inside a database (without password hashing, but is not very secure), log in a user and also get a user by id (by verifying user's jwt token). The database includes only one table, called "User", that holds the following information about the user: id, email, username, password (hashed). Two users can not have the same email or the same username, and of course the id is the primary key and is expressed as a serial integer.
