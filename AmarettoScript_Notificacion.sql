/*
    Amaretto Cakes - Tabla de notificaciones
    ----------------------------------------
    Guarda la notificación que se genera al registrar un pedido. Alimenta la
    campana del encabezado y deja constancia de si el correo de confirmación
    salió o no.

    Ejecutar una sola vez sobre la base AmarettoDB. El script es idempotente:
    si la tabla ya existe no hace nada.
*/

USE [AmarettoDB]
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE [name] = 'Notificacion')
BEGIN
    CREATE TABLE [dbo].[Notificacion](
        [IdNotificacion] [int] IDENTITY(1,1) NOT NULL,
        [IdUsuario]      [int] NOT NULL,
        [IdPedido]       [int] NULL,
        [Titulo]         [varchar](150) NOT NULL,
        [Mensaje]        [varchar](500) NOT NULL,
        [Tipo]           [varchar](30)  NOT NULL CONSTRAINT [DF_Notificacion_Tipo] DEFAULT ('Pedido'),
        [Leida]          [bit] NOT NULL CONSTRAINT [DF_Notificacion_Leida] DEFAULT (0),
        [FechaCreacion]  [datetime] NOT NULL CONSTRAINT [DF_Notificacion_Fecha] DEFAULT (getdate()),
        [CorreoEnviado]  [bit] NOT NULL CONSTRAINT [DF_Notificacion_Correo] DEFAULT (0),
        [DetalleEnvio]   [varchar](300) NULL,
        CONSTRAINT [PK_Notificacion] PRIMARY KEY CLUSTERED ([IdNotificacion] ASC)
    ) ON [PRIMARY]

    ALTER TABLE [dbo].[Notificacion] WITH CHECK
        ADD CONSTRAINT [FK_Notificacion_Usuario] FOREIGN KEY([IdUsuario])
        REFERENCES [dbo].[Usuario] ([IdUsuario])

    ALTER TABLE [dbo].[Notificacion] WITH CHECK
        ADD CONSTRAINT [FK_Notificacion_Pedido] FOREIGN KEY([IdPedido])
        REFERENCES [dbo].[Pedido] ([IdPedido])

    CREATE INDEX [IX_Notificacion_Usuario_Fecha]
        ON [dbo].[Notificacion] ([IdUsuario] ASC, [FechaCreacion] DESC)

    PRINT 'Tabla Notificacion creada.'
END
ELSE
BEGIN
    PRINT 'La tabla Notificacion ya existe, no se hicieron cambios.'
END
GO

/*  Para revertir:
    DROP TABLE [dbo].[Notificacion]
*/
