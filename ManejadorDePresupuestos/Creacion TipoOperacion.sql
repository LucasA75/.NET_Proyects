INSERT INTO TiposOperaciones(Descripcion) VALUES('Ingreso'),('Gasto')

SELECT * FROM Transacciones

SELECT * FROM Categorias
SELECT * FROM TiposOperaciones

INSERT INTO Transacciones(Monto,Nota,FechaTransaccion,CategoriaID,CuentaID,TipoTransaccionID,UsuarioID)
VALUES (@Monto,@Nota,@FechaTransaccion,@CategoriaID,@CuentaID,TipoTransaccionID,UsuarioID);
SELECT SCOPE_IDENTITY();

      SELECT tr.UsuarioID,tr.FechaTransaccion,tr.Monto,tr.Nota,TipoCuenta.Nombre,Categorias.Nombre
      FROM Transacciones tr
      INNER JOIN TipoCuenta
      ON TipoCuenta.ID = tr.CuentaID
	  INNER JOIN  Categorias
	  ON Categorias.ID =  tr.CategoriaID
      WHERE TipoCuenta.UsuarioID = 1
      ORDER BY TipoCuenta.Orden


SELECT tr.* , Categorias.Nombre
FROM Transacciones tr
INNER JOIN Categorias
ON tr.CategoriaID = Categorias.ID
WHERE tr.id = 2 AND tr.UsuarioID = 1