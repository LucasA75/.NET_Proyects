create database ManejoDePresupuesto;

-- drop table Transacciones;

create table TiposOperaciones(
	ID int primary key not null IDENTITY(1,1),
	Descripcion varchar(450) not null,
)

create table Usuarios(
	ID int primary key not null IDENTITY(1,1),
	Email varchar(100) not null,
	EmailNormalizado varchar(100) not null,
	PasswordHash varchar(MAX) not null,
)

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

create table Categorias(
	ID int primary key not null IDENTITY(1,1),
	Nombre varchar(100) not null,
	TipoOperacionID int not null,
	UsuarioID int not null,
	FOREIGN KEY (UsuarioID) REFERENCES Usuarios(ID),
	FOREIGN KEY (TipoOperacionID) REFERENCES TiposOperaciones(ID)
)

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

-- Nuevas
-- Drop table Usuarios
SELECT * FROM Cuentas
SELECT * FROM Usuarios
SELECT * FROM TipoCuenta
INSERT INTO Usuarios(Email,EmailNormalizado,PasswordHash) VALUES ('luc@correo.cl','luc@correo.cl','a111111111111111')

SELECT Cuentas.ID, Cuentas.Nombre, Cuentas.Balance, TipoCuenta.Nombre AS TipoCuenta, descripcion
FROM Cuentas
INNER JOIN TipoCuenta
ON TipoCuenta.ID = Cuentas.TipoCuentaID
WHERE TipoCuenta.UsuarioID = 1 AND Cuentas.ID = 1
ORDER BY TipoCuenta.Orden

INSERT INTO Categorias(Nombre,TipoOperacionID,UsuarioID) VALUES (@Nombre,@TipoOperacionID,@UsuarioID);
SELECT SCOPE_IDENTITY();
