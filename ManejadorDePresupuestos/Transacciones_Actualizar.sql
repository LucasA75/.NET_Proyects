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
CREATE PROCEDURE Transacciones_Actualizar
	@ID int,
	@FechaTransaccion date,
	@Monto decimal(18,2),
	@MontoAnterior decimal(18,2),
	@CuentaID int,
	@CuentaIDAnterior int,
	@CategoriaID int,
	@Nota nchar(1000) = Null

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	-- Revertir Monto Anterior 
	UPDATE Cuentas
	SET Balance -= @MontoAnterior
	WHERE ID = @CuentaIDAnterior

	-- Ingresar cambios
	UPDATE Cuentas
	SET Balance += @Monto
	WHERE ID = @CuentaID

	UPDATE Transacciones
	SET Monto = ABS(@Monto), 
	FechaTransaccion = @FechaTransaccion,
	CuentaID = @CuentaID,
	CategoriaID = @CategoriaID,
	Nota = @Nota
	Where id = @ID



END
GO
