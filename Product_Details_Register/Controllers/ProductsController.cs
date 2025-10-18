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

                Console.WriteLine("We have an exception: \n" + ex.Message);
            }

            return BadRequest();

        }


        [HttpGet]
        public async Task<IActionResult> GetProductsAsync()
        {
            List<Product> products = new List<Product>();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string sql = "SELECT * FROM Products;";
                    var data = await connection.QueryAsync<Product>(sql);
                    products = data.ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("We have an exception: \n" + ex.Message);
                return BadRequest();
            }
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductByIdAsync(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string sql = "SELECT * FROM Products WHERE Id = @Id;";
                    var product = await connection.QuerySingleOrDefaultAsync<Product>(sql, new { Id = id });
                    if (product != null)
                    {
                        return Ok(product);
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("We have an exception: \n" + ex.Message);
                return BadRequest();
            }
            return NotFound();
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateproductAsync (int id, ProductDTO productDTO)
        {
            try
            {
                using(var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string sql = @"
                                   UPDATE Products 
                                   SET Name = @Name, Brand = @Brand, Category = @Category, Price = @Price, Description = @Description
                                   WHERE Id = @Id;
                                  ";

                    var product = new Product()
                    { 
                        Id = id,
                        Name = productDTO.Name,
                        Brand = productDTO.Brand,
                        Category = productDTO.Category,
                        Price = productDTO.Price,
                        Description = productDTO.Description
                    };
                    //var updatedProduct = await connection.QuerySingleOrDefaultAsync<Product>(sql, product);
                    //if (  updatedProduct != null)
                    //{
                    //    return Ok ( updatedProduct);
                    //}

                    int count = await connection.ExecuteAsync(sql, product);
                    if (count < 1)
                    {
                        return NotFound();
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("We have an exception: \n" + ex.Message);
                return BadRequest();
            }
            //return NotFound();
            return await GetProductByIdAsync(id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductAsync (int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string sql = "DELETE FROM Products WHERE Id = @Id;";
                    int count = await connection.ExecuteAsync(sql, new { Id = id });
                    if (count < 1)
                    {
                        return NotFound();
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("We have an exception: \n" + ex.Message);
                return BadRequest();
            }
            return Ok();
        }
    }

}
