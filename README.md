# Programación Web I - EF Tercer Bimestre

Proyecto de examen práctico para **Tercero A Matutino**.

## Orden de ejecución

### Opción rápida

Desde la carpeta principal, ejecutar con doble clic:

`abrir-productos-angular.bat`

Este archivo inicia los tres servicios y abre Angular:

- ProductosSOAP en `http://localhost:5163`
- MovimientosREST en `http://localhost:5265`
- Productos Angular en `http://localhost:4200`

Si los servicios ya están abiertos, no se debe ejecutar nuevamente el lanzador para evitar conflictos de puertos.

## 1. SQL Server

1. Abrir SQL Server Management Studio.
2. Ejecutar `SQL/EF3_Productos_Movimientos.sql`.
3. La conexión configurada por defecto es:

`Server=localhost\SQLEXPRESS;Database=EF3ProductosDB;Trusted_Connection=True;TrustServerCertificate=True;`

Si la instancia de SQL Server es diferente, modificar `DefaultConnection` en:

- `ProductosSOAP/appsettings.json`
- `MovimientosREST/appsettings.json`

## 2. ProductosSOAP

Abrir `ProductosSOAP/ProductosSOAP.csproj` en Visual Studio y ejecutar el perfil **HTTP**.

- Servicio: `http://localhost:5163/ProductoService.svc`
- WSDL: `http://localhost:5163/ProductoService.svc?wsdl`

El WSDL se muestra como XML en el navegador; eso es normal y confirma que el servicio está publicado.

## 3. MovimientosREST

Abrir `MovimientosREST/MovimientosREST.csproj` en Visual Studio y ejecutar el perfil **HTTP**.

URL base:

`http://localhost:5265`

Endpoints existentes:

- `GET /api/MovimientoInventario`
- `GET /api/MovimientoInventario/{id}`
- `GET /api/MovimientoInventario/producto/{idProducto}`
- `POST /api/MovimientoInventario`

El endpoint por producto devuelve únicamente los movimientos de ese producto y responde `404` si no existen movimientos.

El registro de movimientos:

- Acepta solamente `ENTRADA` y `SALIDA`.
- Rechaza cantidades menores o iguales a cero.
- Rechaza productos inexistentes o inactivos.
- Consulta el producto mediante ProductosSOAP.
- Usa el stock SOAP como `StockAnterior`.
- Calcula `StockResultante`.
- Rechaza una salida mayor al stock disponible.
- Actualiza el stock mediante ProductosSOAP antes de guardar el movimiento.

## 4. Productos Angular

Para iniciar únicamente Angular, ejecutar:

`productos-angular/abrir-angular.bat`

También se puede abrir una terminal en `productos-angular` y ejecutar:

```text
npm install
npm start
```

La aplicación estará disponible en `http://localhost:4200`.

Angular consume los servicios existentes:

- `ProductoSoapService` consulta ProductosSOAP, envía `SOAPAction` y procesa la respuesta XML.
- `MovimientoService` consume el endpoint REST existente para listar y registrar movimientos.

No se creó un segundo servicio REST para reemplazar el servicio solicitado. `ProductoSoapClient` es únicamente el cliente interno que permite a MovimientosREST consultar SOAP y actualizar el stock.

Para que aparezcan los productos, SQL Server, ProductosSOAP y MovimientosREST deben estar funcionando.

## 5. Pruebas con Postman

La colección está en:

`Postman/POSTMAN_MovimientosREST.postman_collection.json`

Se puede importar manualmente en Postman o ejecutar:

`Postman/abrir-productos-postman.bat`

La colección incluye pruebas para:

- Listar movimientos.
- Consultar un movimiento por ID.
- Consultar movimientos por producto.
- Registrar una entrada válida.
- Registrar una salida válida.
- Cantidad igual a cero.
- Tipo diferente de `ENTRADA` o `SALIDA`.
- Producto inexistente.
- Producto inactivo.
- Salida superior al stock disponible.

Los POST válidos modifican el stock y agregan un movimiento en la base de datos.

## Estado verificado

- ProductosSOAP funcional y conectado a SQL Server.
- MovimientosREST funcional, con validaciones e integración SOAP.
- Angular funcional, mostrando productos SOAP y movimientos REST.
- Pruebas REST preparadas para Postman.
- Compilación .NET correcta, sin errores ni advertencias.
- Compilación Angular correcta.

## Archivos de inicio

- `abrir-productos-angular.bat`: inicia todo el proyecto.
- `productos-angular/abrir-angular.bat`: inicia solamente Angular.
- `productos-angular/INICIAR_ANGULAR.bat`: alternativa para iniciar Angular.
- `Postman/abrir-productos-postman.bat`: abre la colección de Postman.
