using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Product_Details_Register.Models;

namespace Product_Details_Register.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly string connectionString;
        public ProductsController(IConfiguration config)
        {
            connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductAsync(ProductDTO productDto)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var sql = @"
                                INSERT INTO Products (Name, Brand, Category, Price, Description, CreatedAt)
                                OUTPUT INSERTED.* 
                                VALUES (@Name, @Brand, @Category, @Price, @Description, @CreatedAt);
                               ";

                    var product = new Product()
                    {
                        Name = productDto.Name,
                        Brand = productDto.Brand,
                        Category = productDto.Category,
                        Price = productDto.Price,
                        Description = productDto.Description,
                        CreatedAt = DateTime.UtcNow
                    };

                    var newProduct = await connection.QuerySingleOrDefaultAsync<Product>(sql, product);
                    
                    if (newProduct != null)
                    {
                        return Ok(newProduct);
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("We have an exception: \n", ex);
            }
            
            return BadRequest();
            
        }
    }
}
