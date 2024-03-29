USE [ManejoDePresupuesto]
GO
/****** Object:  StoredProcedure [dbo].[CrearDatosUsuarioNuevo]    Script Date: 01-02-2024 15:13:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE CrearDatosUsuarioNuevo
	@UsuarioID int 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @Efectivo nvarchar(50) = 'Efectivo'
	DECLARE @CuentasDeBanco nvarchar(50) = 'Cuentas de Banco'
	DECLARE @Tarjetas nvarchar(50) = 'Tarjetas'

   INSERT INTO TipoCuenta(Nombre,UsuarioID,Orden) 
   VALUES (@Efectivo,@UsuarioID,1),
   (@CuentasDeBanco,@UsuarioID,2),
   (@Tarjetas,@UsuarioID,3)

   INSERT INTO Cuentas(Nombre,Balance,TipoCuentaID)
   SELECT Nombre,0,Id
   FROM TipoCuenta
   WHERE UsuarioID = @UsuarioID

   INSERT INTO Categorias(Nombre, TipoOperacionID, UsuarioID) 
   VALUES('Libros',2,@UsuarioID),
   ('Mangas',2,@UsuarioID),
   ('Articulos Personales',2,@UsuarioID),
   ('Bono',1,@UsuarioID),
   ('Sueldo',1,@UsuarioID)

END
