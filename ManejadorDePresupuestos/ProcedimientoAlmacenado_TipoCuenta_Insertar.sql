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
CREATE PROCEDURE TipoCuenta_Insertar
	@Nombre varchar(50),
	@UsuarioId int 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Declare @Orden int;
	Select @Orden = COALESCE(MAX(Orden),0)+1
	FROM TipoCuenta
	WHERE UsuarioID = @UsuarioId

	INSERT INTO TipoCuenta(Nombre,UsuarioID,Orden)
	VALUES (@Nombre,@UsuarioId,@Orden)

	SELECT SCOPE_IDENTITY()

END
GO
