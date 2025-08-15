<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Reportes.aspx.cs" Inherits="SistemaCotizaciones.Pages.Reportes" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Sistema de Cotizaciones - Reportes</title>
    <link href="../CSS/styles.css" rel="stylesheet" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <style type="text/css">
        .reporte-section {
            background-color: #f8f9fa;
            border: 1px solid #dee2e6;
            border-radius: 8px;
            padding: 20px;
            margin-bottom: 20px;
        }
        .reporte-header {
            background-color: #17a2b8;
            color: white;
            padding: 10px 15px;
            border-radius: 5px 5px 0 0;
            margin: -20px -20px 15px -20px;
            font-weight: bold;
        }
        .filtros-container {
            display: flex;
            gap: 15px;
            align-items: end;
            margin-bottom: 15px;
            flex-wrap: wrap;
        }
        .filtro-group {
            display: flex;
            flex-direction: column;
            min-width: 150px;
        }
        .estadisticas {
            display: flex;
            gap: 20px;
            margin: 20px 0;
            flex-wrap: wrap;
        }
        .estadistica-card {
            background-color: #fff;
            border: 1px solid #dee2e6;
            border-radius: 8px;
            padding: 15px;
            text-align: center;
            min-width: 120px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .estadistica-numero {
            font-size: 24px;
            font-weight: bold;
            color: #17a2b8;
        }
        .estadistica-label {
            font-size: 12px;
            color: #6c757d;
            margin-top: 5px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <!-- Header -->
        <div class="header">
            <h1>🌪 Sistema de Cotizaciones - Pasty Custom Design</h1>
        </div>

        <!-- Navigation Menu -->
        <div class="nav-menu">
            <a href="Default.aspx">🏠 Dashboard</a>
            <a href="Clientes.aspx">👥 Clientes</a>
            <a href="Productos.aspx">📦 Productos</a>
            <a href="Cotizaciones.aspx">📋 Cotizaciones</a>
            <a href="Reportes.aspx" style="background-color: #2c3e50;">📊 Reportes</a>
            <asp:LinkButton ID="lnkLogout" runat="server" OnClick="lnkLogout_Click" style="color: white; text-decoration: none; padding: 10px 20px; margin: 0 5px;">🚪 Cerrar Sesión</asp:LinkButton>
        </div>

        <div class="container">
            <div class="form-container">
                <h2>📊 Sistema de Reportes</h2>

                <!-- Mensajes de error/éxito -->
                <asp:Panel ID="pnlMessage" runat="server" Visible="false">
                    <asp:Label ID="lblMessage" runat="server" CssClass="alert"></asp:Label>
                </asp:Panel>

                <!-- Estadísticas Generales -->
                <div class="reporte-section">
                    <div class="reporte-header">📈 Estadísticas Generales del Sistema</div>
                    <div class="estadisticas">
                        <div class="estadistica-card">
                            <div class="estadistica-numero">
                                <asp:Label ID="lblTotalClientes" runat="server" Text="0"></asp:Label>
                            </div>
                            <div class="estadistica-label">Total Clientes</div>
                        </div>
                        <div class="estadistica-card">
                            <div class="estadistica-numero">
                                <asp:Label ID="lblTotalProductos" runat="server" Text="0"></asp:Label>
                            </div>
                            <div class="estadistica-label">Total Productos</div>
                        </div>
                        <div class="estadistica-card">
                            <div class="estadistica-numero">
                                <asp:Label ID="lblTotalCotizaciones" runat="server" Text="0"></asp:Label>
                            </div>
                            <div class="estadistica-label">Total Cotizaciones</div>
                        </div>
                        <div class="estadistica-card">
                            <div class="estadistica-numero">
                                <asp:Label ID="lblProductosAgotados" runat="server" Text="0"></asp:Label>
                            </div>
                            <div class="estadistica-label">Productos Agotados</div>
                        </div>
                        <div class="estadistica-card">
                            <div class="estadistica-numero">
                                <asp:Label ID="lblCotizacionesVencidas" runat="server" Text="0"></asp:Label>
                            </div>
                            <div class="estadistica-label">Cotizaciones Vencidas</div>
                        </div>
                    </div>
                    <asp:Button ID="btnActualizarEstadisticas" runat="server" Text="🔄 Actualizar Estadísticas" 
                        CssClass="btn btn-primary" OnClick="btnActualizarEstadisticas_Click" />
                </div>

                <!-- REQ-REP-01: Reporte de Clientes -->
                <div class="reporte-section">
                    <div class="reporte-header">👥 REQ-REP-01: Reporte de Clientes</div>
                    <div class="filtros-container">
                        <div class="filtro-group">
                            <label>Filtrar por:</label>
                            <asp:DropDownList ID="ddlFiltroClientes" runat="server" CssClass="form-control">
                                <asp:ListItem Value="todos">Todos los clientes</asp:ListItem>
                                <asp:ListItem Value="activos">Solo activos</asp:ListItem>
                                <asp:ListItem Value="inactivos">Solo inactivos</asp:ListItem>
                                <asp:ListItem Value="tipo">Por tipo de cliente</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="filtro-group">
                            <label>Tipo de cliente:</label>
                            <asp:DropDownList ID="ddlTipoCliente" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">Todos los tipos</asp:ListItem>
                                <asp:ListItem Value="Individual">Individual</asp:ListItem>
                                <asp:ListItem Value="Empresa">Empresa</asp:ListItem>
                                <asp:ListItem Value="Mayorista">Mayorista</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="filtro-group">
                            <asp:Button ID="btnGenerarReporteClientes" runat="server" Text="📋 Generar Reporte" 
                                CssClass="btn btn-success" OnClick="btnGenerarReporteClientes_Click" />
                        </div>
                    </div>
                    <asp:Panel ID="pnlReporteClientes" runat="server" Visible="false">
                        <asp:GridView ID="gvReporteClientes" runat="server" CssClass="table" AutoGenerateColumns="false" 
                            EmptyDataText="No hay clientes que coincidan con los filtros seleccionados."
                            HeaderStyle-BackgroundColor="#17a2b8" HeaderStyle-ForeColor="White">
                            <Columns>
                                <asp:BoundField DataField="ID" HeaderText="ID" />
                                <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                                <asp:BoundField DataField="Identificacion" HeaderText="Identificación" />
                                <asp:BoundField DataField="Telefono" HeaderText="Teléfono" />
                                <asp:BoundField DataField="Email" HeaderText="Email" />
                                <asp:BoundField DataField="TipoCliente" HeaderText="Tipo" />
                                <asp:BoundField DataField="FechaCreacion" HeaderText="Fecha Registro" DataFormatString="{0:dd/MM/yyyy}" />
                                <asp:TemplateField HeaderText="Estado">
                                    <ItemTemplate>
                                        <asp:Label ID="lblEstadoCliente" runat="server" 
                                            Text='<%# Convert.ToBoolean(Eval("Activo")) ? "✅ ACTIVO" : "❌ INACTIVO" %>'
                                            ForeColor='<%# Convert.ToBoolean(Eval("Activo")) ? System.Drawing.Color.Green : System.Drawing.Color.Red %>'
                                            Font-Bold="true">
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>
                </div>

                <!-- REQ-REP-02: Reporte de Productos -->
                <div class="reporte-section">
                    <div class="reporte-header">📦 REQ-REP-02: Reporte de Productos</div>
                    <div class="filtros-container">
                        <div class="filtro-group">
                            <label>Filtrar por stock:</label>
                            <asp:DropDownList ID="ddlFiltroProductos" runat="server" CssClass="form-control">
                                <asp:ListItem Value="todos">Todos los productos</asp:ListItem>
                                <asp:ListItem Value="disponibles">Con stock disponible</asp:ListItem>
                                <asp:ListItem Value="agotados">Productos agotados</asp:ListItem>
                                <asp:ListItem Value="bajo_stock">Stock bajo (≤5)</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="filtro-group">
                            <label>Precio mínimo (₡):</label>
                            <asp:TextBox ID="txtPrecioMinimo" runat="server" CssClass="form-control" 
                                TextMode="Number" step="0.01" placeholder="0.00"></asp:TextBox>
                        </div>
                        <div class="filtro-group">
                            <label>Precio máximo (₡):</label>
                            <asp:TextBox ID="txtPrecioMaximo" runat="server" CssClass="form-control" 
                                TextMode="Number" step="0.01" placeholder="999999.99"></asp:TextBox>
                        </div>
                        <div class="filtro-group">
                            <asp:Button ID="btnGenerarReporteProductos" runat="server" Text="📋 Generar Reporte" 
                                CssClass="btn btn-success" OnClick="btnGenerarReporteProductos_Click" />
                        </div>
                    </div>
                    <asp:Panel ID="pnlReporteProductos" runat="server" Visible="false">
                        <asp:GridView ID="gvReporteProductos" runat="server" CssClass="table" AutoGenerateColumns="false" 
                            EmptyDataText="No hay productos que coincidan con los filtros seleccionados."
                            HeaderStyle-BackgroundColor="#27ae60" HeaderStyle-ForeColor="White">
                            <Columns>
                                <asp:BoundField DataField="ID" HeaderText="ID" />
                                <asp:BoundField DataField="CodigoCabys" HeaderText="Código CABYS" />
                                <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                                <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                                <asp:BoundField DataField="CantidadDisponible" HeaderText="Stock" />
                                <asp:BoundField DataField="Precio" HeaderText="Precio (₡)" DataFormatString="{0:N2}" />
                                <asp:BoundField DataField="FechaCreacion" HeaderText="Fecha Registro" DataFormatString="{0:dd/MM/yyyy}" />
                                <asp:TemplateField HeaderText="Estado Stock">
                                    <ItemTemplate>
                                        <asp:Label ID="lblEstadoStock" runat="server" 
                                            Text='<%# (Convert.ToInt32(Eval("CantidadDisponible")) == 0) ? "❌ AGOTADO" : 
                                                   (Convert.ToInt32(Eval("CantidadDisponible")) <= 5) ? "⚠️ BAJO" : "✅ DISPONIBLE" %>'
                                            ForeColor='<%# (Convert.ToInt32(Eval("CantidadDisponible")) == 0) ? System.Drawing.Color.Red : 
                                                          (Convert.ToInt32(Eval("CantidadDisponible")) <= 5) ? System.Drawing.Color.Orange : System.Drawing.Color.Green %>'
                                            Font-Bold="true">
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>
                </div>

                <!-- REQ-REP-03: Reporte de Cotizaciones -->
                <div class="reporte-section">
                    <div class="reporte-header">📋 REQ-REP-03: Reporte de Cotizaciones</div>
                    <div class="filtros-container">
                        <div class="filtro-group">
                            <label>Filtrar por estado:</label>
                            <asp:DropDownList ID="ddlFiltroCotizaciones" runat="server" CssClass="form-control">
                                <asp:ListItem Value="todas">Todas las cotizaciones</asp:ListItem>
                                <asp:ListItem Value="vigentes">Solo vigentes (≤30 días)</asp:ListItem>
                                <asp:ListItem Value="vencidas">Solo vencidas (>30 días)</asp:ListItem>
                                <asp:ListItem Value="por_vencer">Por vencer (23-30 días)</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="filtro-group">
                            <label>Fecha desde:</label>
                            <asp:TextBox ID="txtFechaDesde" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="filtro-group">
                            <label>Fecha hasta:</label>
                            <asp:TextBox ID="txtFechaHasta" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="filtro-group">
                            <asp:Button ID="btnGenerarReporteCotizaciones" runat="server" Text="📋 Generar Reporte" 
                                CssClass="btn btn-success" OnClick="btnGenerarReporteCotizaciones_Click" />
                        </div>
                    </div>
                    <asp:Panel ID="pnlReporteCotizaciones" runat="server" Visible="false">
                        <div style="margin-bottom: 10px;">
                            <strong>Total cotizaciones encontradas: 
                                <asp:Label ID="lblTotalCotizacionesReporte" runat="server" Text="0" style="color: #e74c3c;"></asp:Label>
                            </strong>
                            <span style="margin-left: 20px;">Monto total: 
                                <asp:Label ID="lblMontoTotalCotizaciones" runat="server" Text="₡0.00" style="color: #27ae60; font-weight: bold;"></asp:Label>
                            </span>
                        </div>
                        <asp:GridView ID="gvReporteCotizaciones" runat="server" CssClass="table" AutoGenerateColumns="false" 
                            EmptyDataText="No hay cotizaciones que coincidan con los filtros seleccionados."
                            HeaderStyle-BackgroundColor="#e74c3c" HeaderStyle-ForeColor="White">
                            <Columns>
                                <asp:BoundField DataField="ID" HeaderText="ID" />
                                <asp:BoundField DataField="NombreCliente" HeaderText="Cliente" />
                                <asp:BoundField DataField="IdentificacionCliente" HeaderText="Identificación" />
                                <asp:BoundField DataField="NombreProducto" HeaderText="Producto" />
                                <asp:BoundField DataField="Cantidad" HeaderText="Cantidad" />
                                <asp:BoundField DataField="PrecioUnitario" HeaderText="Precio Unit. (₡)" DataFormatString="{0:N2}" />
                                <asp:BoundField DataField="Descuento" HeaderText="Desc. %" DataFormatString="{0:N1}" />
                                <asp:BoundField DataField="Total" HeaderText="Total (₡)" DataFormatString="{0:N2}" />
                                <asp:BoundField DataField="FechaCotizacion" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                                <asp:TemplateField HeaderText="Estado">
                                    <ItemTemplate>
                                        <asp:Label ID="lblEstadoCotizacion" runat="server" 
                                            Text='<%# DateTime.Now.Subtract((DateTime)Eval("FechaCotizacion")).Days > 30 ? "⏰ VENCIDA" : 
                                                   DateTime.Now.Subtract((DateTime)Eval("FechaCotizacion")).Days > 23 ? "⚠️ POR VENCER" : "✅ VIGENTE" %>'
                                            ForeColor='<%# DateTime.Now.Subtract((DateTime)Eval("FechaCotizacion")).Days > 30 ? System.Drawing.Color.Red : 
                                                          DateTime.Now.Subtract((DateTime)Eval("FechaCotizacion")).Days > 23 ? System.Drawing.Color.Orange : System.Drawing.Color.Green %>'
                                            Font-Bold="true">
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>
                </div>

                <!-- REQ-REP-04: Reporte de Ventas -->
                <div class="reporte-section">
                    <div class="reporte-header">💰 REQ-REP-04: Reporte de Ventas (Análisis Financiero)</div>
                    <div class="filtros-container">
                        <div class="filtro-group">
                            <label>Análisis por:</label>
                            <asp:DropDownList ID="ddlFiltroVentas" runat="server" CssClass="form-control">
                                <asp:ListItem Value="todas">Todas las ventas</asp:ListItem>
                                <asp:ListItem Value="monto_alto">Ventas altas (>₡50,000)</asp:ListItem>
                                <asp:ListItem Value="monto_medio">Ventas medias (₡10,000-₡50,000)</asp:ListItem>
                                <asp:ListItem Value="monto_bajo">Ventas bajas (<₡10,000)</asp:ListItem>
                                <asp:ListItem Value="mes_actual">Del mes actual</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="filtro-group">
                            <label>Monto mínimo (₡):</label>
                            <asp:TextBox ID="txtMontoMinimo" runat="server" CssClass="form-control" 
                                TextMode="Number" step="0.01" placeholder="0.00"></asp:TextBox>
                        </div>
                        <div class="filtro-group">
                            <label>Monto máximo (₡):</label>
                            <asp:TextBox ID="txtMontoMaximo" runat="server" CssClass="form-control" 
                                TextMode="Number" step="0.01" placeholder="999999.99"></asp:TextBox>
                        </div>
                        <div class="filtro-group">
                            <asp:Button ID="btnGenerarReporteVentas" runat="server" Text="📋 Generar Reporte" 
                                CssClass="btn btn-success" OnClick="btnGenerarReporteVentas_Click" />
                        </div>
                    </div>
                    
                    <!-- Estadísticas de Ventas -->
                    <asp:Panel ID="pnlEstadisticasVentas" runat="server" Visible="false">
                        <div class="estadisticas">
                            <div class="estadistica-card">
                                <div class="estadistica-numero">
                                    <asp:Label ID="lblTotalVentas" runat="server" Text="0"></asp:Label>
                                </div>
                                <div class="estadistica-label">Total Ventas</div>
                            </div>
                            <div class="estadistica-card">
                                <div class="estadistica-numero">
                                    <asp:Label ID="lblMontoTotalVentas" runat="server" Text="₡0.00"></asp:Label>
                                </div>
                                <div class="estadistica-label">Monto Total</div>
                            </div>
                            <div class="estadistica-card">
                                <div class="estadistica-numero">
                                    <asp:Label ID="lblPromedioVenta" runat="server" Text="₡0.00"></asp:Label>
                                </div>
                                <div class="estadistica-label">Promedio por Venta</div>
                            </div>
                            <div class="estadistica-card">
                                <div class="estadistica-numero">
                                    <asp:Label ID="lblVentaMasAlta" runat="server" Text="₡0.00"></asp:Label>
                                </div>
                                <div class="estadistica-label">Venta Más Alta</div>
                            </div>
                        </div>
                    </asp:Panel>
                    
                    <asp:Panel ID="pnlReporteVentas" runat="server" Visible="false">
                        <asp:GridView ID="gvReporteVentas" runat="server" CssClass="table" AutoGenerateColumns="false" 
                            EmptyDataText="No hay ventas que coincidan con los filtros seleccionados."
                            HeaderStyle-BackgroundColor="#f39c12" HeaderStyle-ForeColor="White">
                            <Columns>
                                <asp:BoundField DataField="ID" HeaderText="ID Cotización" />
                                <asp:BoundField DataField="NombreCliente" HeaderText="Cliente" />
                                <asp:BoundField DataField="TipoCliente" HeaderText="Tipo Cliente" />
                                <asp:BoundField DataField="NombreProducto" HeaderText="Producto" />
                                <asp:BoundField DataField="Cantidad" HeaderText="Cantidad" />
                                <asp:BoundField DataField="PrecioUnitario" HeaderText="Precio Unit. (₡)" DataFormatString="{0:N2}" />
                                <asp:BoundField DataField="Descuento" HeaderText="Desc. %" DataFormatString="{0:N1}" />
                                <asp:BoundField DataField="Total" HeaderText="Total (₡)" DataFormatString="{0:N2}" />
                                <asp:BoundField DataField="FechaCotizacion" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                                <asp:TemplateField HeaderText="Categoría">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCategoriaVenta" runat="server" 
                                            Text='<%# Convert.ToDecimal(Eval("Total")) > 50000 ? "💎 ALTA" : 
                                                   Convert.ToDecimal(Eval("Total")) >= 10000 ? "💰 MEDIA" : "💵 BAJA" %>'
                                            ForeColor='<%# Convert.ToDecimal(Eval("Total")) > 50000 ? System.Drawing.Color.Purple : 
                                                          Convert.ToDecimal(Eval("Total")) >= 10000 ? System.Drawing.Color.Green : System.Drawing.Color.Blue %>'
                                            Font-Bold="true">
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>
                </div>

                <!-- Botones de Exportación -->
                <div style="margin-top: 30px; text-align: center;">
                    <h4>📤 Exportar Reportes</h4>
                    <asp:Button ID="btnExportarTodo" runat="server" Text="📊 Exportar Reporte Completo (PDF)" 
                        CssClass="btn btn-primary" OnClick="btnExportarTodo_Click" />
                    <span style="margin: 0 10px;">|</span>
                    <asp:Button ID="btnImprimirReporte" runat="server" Text="🖨️ Imprimir Reporte" 
                        CssClass="btn btn-secondary" OnClientClick="window.print(); return false;" />
                </div>
            </div>
        </div>
    </form>

    <script type="text/javascript">
        // Establecer fecha actual por defecto
        window.onload = function() {
            var today = new Date().toISOString().split('T')[0];
            var fechaHasta = document.getElementById('<%= txtFechaHasta.ClientID %>');
            if (fechaHasta.value === '') {
                fechaHasta.value = today;
            }
            
            var fechaDesde = document.getElementById('<%= txtFechaDesde.ClientID %>');
            if (fechaDesde.value === '') {
                var firstDay = new Date(new Date().getFullYear(), new Date().getMonth(), 1).toISOString().split('T')[0];
                fechaDesde.value = firstDay;
            }
        };
    </script>
</body>
</html>