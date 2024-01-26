SELECT t.id, t.Monto, t.FechaTransaccion, ct.Nombre as Categoria, cu.Nombre as Cuenta,
                ct.TipoOperacionID as TipoOperacion
                FROM Transacciones t
                INNER JOIN Categorias ct
                ON ct.ID = t.CategoriaID
                INNER JOIN Cuentas cu
                ON cu.ID = t.CuentaID
                WHERE t.CuentaID = 1 AND t.UsuarioID = 1 


Select * FROM
Categorias ct
Inner JOIN
TiposOperaciones 
ON ct.TipoOperacionID = TiposOperaciones.ID

Declare @fechainicio date = '2024-01-01'
Declare @fechaFin date = '2024-01-30'
Declare @UsuarioID int = 1


SELECT datediff(d,@fechaInicio,FechaTransaccion) / 7 + 1 as Semana, Sum(Monto) as Monto, ct.TipoOperacionID
From Transacciones
INNER JOIN Categorias ct
ON ct.ID = Transacciones.CategoriaID
Where Transacciones.UsuarioID = @UsuarioID AND
FechaTransaccion Between @fechaInicio and @fechaFin
Group BY datediff(d,@fechaInicio,FechaTransaccion) / 7, ct.TipoOperacionID
