CREATE TABLE users(
	id int primary key auto_increment,
	username varchar(100),
	password_hash varchar(250),
	role varchar(100)
);