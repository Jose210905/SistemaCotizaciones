<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SistemaCotizaciones.Pages.Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Sistema de Cotizaciones - Dashboard</title>
    <link href="../CSS/styles.css" rel="stylesheet" />
</head>
<body>
    <!-- Header -->
    <div class="header">
        <div class="container">
            <h1>Sistema Generador de Cotizaciones</h1>
            <p>Bienvenido, <asp:Label ID="lblUsuario" runat="server"></asp:Label></p>
        </div>
    </div>

    <!-- Menú de navegación -->
    <div class="nav-menu">
        <div class="container">
            <a href="Default.aspx">🏠 Dashboard</a>
            <a href="Clientes.aspx">👥 Clientes</a>
            <a href="Productos.aspx">📦 Productos</a>
            <a href="Cotizaciones.aspx">📋 Cotizaciones</a>
            <a href="Reportes.aspx">📊 Reportes</a>
            <asp:LinkButton ID="btnCerrarSesion" runat="server" CssClass="nav-link" 
                           OnClick="btnCerrarSesion_Click" style="color: white; text-decoration: none;">
                🚪 Cerrar Sesión
            </asp:LinkButton>
        </div>
    </div>

    <div class="container">
        <form id="form1" runat="server">
            
            <!-- Alertas de cotizaciones por vencer -->
            <asp:Panel ID="pnlAlertas" runat="server" Visible="false" CssClass="alert alert-warning" 
                      style="background-color: #fff3cd; border-color: #ffeaa7; color: #856404;">
                <h4>⚠️ Alertas del Sistema</h4>
                <asp:Label ID="lblAlertas" runat="server"></asp:Label>
            </asp:Panel>

            <!-- Resumen del sistema -->
            <div style="display: flex; flex-wrap: wrap; gap: 20px; margin: 20px 0;">
                
                <!-- Tarjeta Clientes -->
                <div class="form-container" style="flex: 1; min-width: 250px; text-align: center;">
                    <h3 style="color: #3498db;">👥 Clientes</h3>
                    <div style="font-size: 2em; font-weight: bold; color: #2c3e50; margin: 10px 0;">
                        <asp:Label ID="lblTotalClientes" runat="server" Text="0"></asp:Label>
                    </div>
                    <p>Clientes registrados</p>
                    <a href="Clientes.aspx" class="btn btn-primary">Gestionar Clientes</a>
                </div>

                <!-- Tarjeta Productos -->
                <div class="form-container" style="flex: 1; min-width: 250px; text-align: center;">
                    <h3 style="color: #27ae60;">📦 Productos</h3>
                    <div style="font-size: 2em; font-weight: bold; color: #2c3e50; margin: 10px 0;">
                        <asp:Label ID="lblTotalProductos" runat="server" Text="0"></asp:Label>
                    </div>
                    <p>Productos disponibles</p>
                    <a href="Productos.aspx" class="btn btn-success">Gestionar Productos</a>
                </div>

                <!-- Tarjeta Cotizaciones -->
                <div class="form-container" style="flex: 1; min-width: 250px; text-align: center;">
                    <h3 style="color: #e74c3c;">📋 Cotizaciones</h3>
                    <div style="font-size: 2em; font-weight: bold; color: #2c3e50; margin: 10px 0;">
                        <asp:Label ID="lblTotalCotizaciones" runat="server" Text="0"></asp:Label>
                    </div>
                    <p>Cotizaciones activas</p>
                    <a href="Cotizaciones.aspx" class="btn btn-danger">Gestionar Cotizaciones</a>
                </div>
            </div>

            <!-- Cotizaciones recientes -->
            <div class="form-container">
                <h3>📋 Cotizaciones Recientes</h3>
                <asp:Panel ID="pnlCotizacionesRecientes" runat="server">
                    <asp:GridView ID="gvCotizacionesRecientes" runat="server" CssClass="table" 
                                 AutoGenerateColumns="false" EmptyDataText="No hay cotizaciones recientes">
                        <Columns>
                            <asp:BoundField DataField="ID" HeaderText="ID" />
                            <asp:BoundField DataField="NombreCliente" HeaderText="Cliente" />
                            <asp:BoundField DataField="NombreProducto" HeaderText="Producto" />
                            <asp:BoundField DataField="CantidadProducto" HeaderText="Cantidad" />
                            <asp:BoundField DataField="Total" HeaderText="Total" DataFormatString="{0:C}" />
                            <asp:BoundField DataField="FechaCotizacion" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                            <asp:BoundField DataField="Estado" HeaderText="Estado" />
                        </Columns>
                    </asp:GridView>
                </asp:Panel>
            </div>

            <!-- Acciones rápidas -->
            <div class="form-container">
                <h3>🚀 Acciones Rápidas</h3>
                <div style="display: flex; gap: 15px; flex-wrap: wrap;">
                    <a href="Clientes.aspx" class="btn btn-primary">➕ Nuevo Cliente</a>
                    <a href="Productos.aspx" class="btn btn-success">➕ Nuevo Producto</a>
                    <a href="Cotizaciones.aspx" class="btn btn-danger">➕ Nueva Cotización</a>
                    <a href="Reportes.aspx" class="btn" style="background-color: #9b59b6; color: white;">📊 Ver Reportes</a>
                </div>
            </div>

        </form>
    </div>
</body>
</html>