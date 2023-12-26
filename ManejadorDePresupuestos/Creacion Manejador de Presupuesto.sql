create database ManejoDePresupuesto;

-- drop table Transacciones;

create table Transacciones(
	id int primary key not null IDENTITY(1,1),
	UsuarioID int not null,
	FechaTransaccion datetime not null,
	Monto Decimal (18,2) not null,
	TipoTransaccionID int not null,
	Nota varchar(1000),
	CuentaID int,
	CategoriaID int
	FOREIGN KEY (TipoTransaccionID) REFERENCES TiposOperaciones(ID),
		FOREIGN KEY (UsuarioID) REFERENCES Usuarios(ID),
			FOREIGN KEY (CuentaID) REFERENCES Cuentas(ID),
			FOREIGN KEY (CategoriaID) REFERENCES Categorias(ID)
);

create table TiposOperaciones(
	ID int primary key not null IDENTITY(1,1),
	Descripcion varchar(450) not null,
)

-- Nuevas
create table TipoCuenta(
	ID int primary key not null IDENTITY(1,1),
	Nombre varchar(50) not null,
	UsuarioID int,
	Orden int,
	FOREIGN KEY (UsuarioID) REFERENCES Usuarios(ID)
)

create table Cuentas(
	ID int primary key not null IDENTITY(1,1),
	Nombre varchar(50) not null,
	TipoCuentaID int,
	Balance Decimal(18,2),
	Descripcion varchar(1000)
	FOREIGN KEY (TipoCuentaID) REFERENCES TipoCuenta(ID)
)

Drop table Usuarios

create table Usuarios(
	ID int primary key not null IDENTITY(1,1),
	Email varchar(100) not null,
	EmailNormalizado varchar(100) not null,
	PasswordHash varchar(MAX) not null,
)

create table Categorias(
	ID int primary key not null IDENTITY(1,1),
	Nombre varchar(100) not null,
	TipoOperacionID int not null,
	UsuarioID int not null,
	FOREIGN KEY (UsuarioID) REFERENCES Usuarios(ID),
	FOREIGN KEY (TipoOperacionID) REFERENCES TiposOperaciones(ID)
)