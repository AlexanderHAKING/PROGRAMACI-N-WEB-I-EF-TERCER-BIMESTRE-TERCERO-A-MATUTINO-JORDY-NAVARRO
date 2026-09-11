using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovimientosREST.Data;
using MovimientosREST.Models;
using MovimientosREST.Services;

namespace MovimientosREST.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovimientoInventarioController : ControllerBase
{
    private readonly MovimientosDbContext _context;
    private readonly ProductoSoapClient _productoSoapClient;

    public MovimientoInventarioController(
        MovimientosDbContext context,
        ProductoSoapClient productoSoapClient
        )
    {
        _context = context;
        _productoSoapClient = productoSoapClient;
    }

    [HttpGet]
    public async Task<ActionResult<List<MovimientoInventario>>> Listar()
    {
        return await _context.MovimientosInventario
            .OrderByDescending(x => x.Fecha)
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MovimientoInventario>> Obtener(int id)
    {
        var movimiento = await _context.MovimientosInventario.FindAsync(id);
        return movimiento is null ? NotFound() : Ok(movimiento);
    }

    [HttpGet("producto/{idProducto:int}")]
    public async Task<ActionResult<List<MovimientoInventario>>> ObtenerPorProducto(int idProducto)
    {
        var movimientos = await _context.MovimientosInventario
            .Where(x => x.IdProducto == idProducto)
            .OrderByDescending(x => x.Fecha)
            .ToListAsync();

        return movimientos.Count == 0 ? NotFound() : Ok(movimientos);
    }

    [HttpPost]
    public async Task<IActionResult> Registrar(MovimientoInventario movimiento)
    {   
        var producto = await _productoSoapClient.ObtenerProductoAsync(movimiento.IdProducto);
        if (producto is null)
            return BadRequest("El producto no existe.");

        if (!producto.Estado)
            return BadRequest("El producto no está activo.");

        movimiento.Tipo = movimiento.Tipo.ToUpper().Trim();
        if (movimiento.Tipo != "ENTRADA" && movimiento.Tipo != "SALIDA")
            return BadRequest("El tipo debe ser ENTRADA o SALIDA.");

        if (movimiento.Cantidad <= 0)
            return BadRequest("La cantidad debe ser mayor que cero.");

        movimiento.StockAnterior = producto.Stock;

        if (movimiento.Tipo == "SALIDA" && movimiento.Cantidad > producto.Stock)
            return BadRequest("No existe stock suficiente.");

        movimiento.StockResultante = movimiento.Tipo == "ENTRADA"
            ? producto.Stock + movimiento.Cantidad
            : producto.Stock - movimiento.Cantidad;

        var stockActualizado = await _productoSoapClient.ActualizarStockAsync(
            movimiento.IdProducto,
            movimiento.StockResultante);

        if (!stockActualizado)
            return BadRequest("No fue posible actualizar el stock del producto.");

        movimiento.IdMovimiento = 0;
        movimiento.Fecha = DateTime.Now;

        _context.MovimientosInventario.Add(movimiento);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Obtener), new { id = movimiento.IdMovimiento }, movimiento);
    }
}
