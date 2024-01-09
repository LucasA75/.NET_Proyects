-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE Transacciones_Insertar
	@UsuarioID int,
	@FechaTransaccion date,
	@Monto decimal(18,2),
	@CategoriaID int,
	@CuentaID int,
	@Nota nvarchar(1000) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO Transacciones(UsuarioID,FechaTransaccion,Monto,CategoriaID,CuentaID,Nota)
	VALUES(@UsuarioID,@FechaTransaccion,ABS(@Monto),@CategoriaID,@CuentaID,@Nota);

	UPDATE Cuentas
	SET Balance += @Monto
	WHERE ID = @CuentaID;
	
	SELECT SCOPE_IDENTITY();

END
GO
