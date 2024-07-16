using backend.EfCore;
using backend.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace backend.Controllers
{
    [ApiController]
    public class APIController : ControllerBase
    {
        private readonly DbHelper _db;
        public APIController(EF_DataContext eF_DataContext)
        {
            _db = new DbHelper(eF_DataContext);
        }

        // GET: api/<APIController>
        [HttpGet]
        [Route("api/[controller]/GetProducts")]

        public IActionResult Get()
        {
            ResponseType type = ResponseType.Success;
            try
            {
                IEnumerable<Product> data = _db.GetProducts();

                if (!data.Any())
                {
                    type = ResponseType.NotFound;
                }
                return Ok(ResponseHandler.GetAppResponse(type, data));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }

        // GET api/<APIController>/5
        [HttpGet]
        [Route("api/[controller]/GetProductById/{id}")]

        public IActionResult Get(int id)
        {
            ResponseType type = ResponseType.Success;
            try
            {
                ProductModel data = _db.GetProductById(id);

                if (data == null)
                {
                    type = ResponseType.NotFound;
                }
                return Ok(ResponseHandler.GetAppResponse(type, data));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }
        [HttpGet]
        [Route("api/[controller]/GetOrders")]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                ResponseType type = ResponseType.Success;

                List<OrderData> data = await _db.GetAllOrders();

                if (!data.Any())
                {
                    type = ResponseType.NotFound;
                }
                return Ok(ResponseHandler.GetAppResponse(type, data));
            }
            catch (Exception ex)
            {
                // Trả về lỗi BadRequest với thông điệp từ ngoại lệ
                return BadRequest(ex.Message);
            }
        }





        

        [HttpPost]
        [Route("api/[controller]/SaveOrder")]
        public IActionResult SaveOrder([FromBody] OrderData orderData)
        {
            try
            {
                _db.SaveOrder(orderData.OrderModel, orderData.ProductIds);
                ResponseType type = ResponseType.Success;
                return Ok(ResponseHandler.GetAppResponse(type, orderData));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }

        // PUT api/<APIController>/5
        [HttpPut]
        [Route("api/[controller]/UpdateOrder")]

        public IActionResult Put([FromBody] OrderData orderData)
        {
            try
            {
                ResponseType type = ResponseType.Success;
                _db.SaveOrder(orderData.OrderModel, orderData.ProductIds);
                return Ok(ResponseHandler.GetAppResponse(type, orderData));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }

        // DELETE api/<APIController>/5
        [HttpDelete]
        [Route("api/[controller]/DeleteOrder/{id}")]

        public IActionResult Delete(int id)
        {
            try
            {
                ResponseType type = ResponseType.Success;
                _db.DeleteOrder(id);
                return Ok(ResponseHandler.GetAppResponse(type, "Delete Successfully"));

            }
            catch (Exception ex) {
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));

            }
        }

        [HttpDelete]
        [Route("api/[controller]/DeleteProduct/{id}")]

        public IActionResult DeleteProduct(int id)
        {
            try
            {
                ResponseType type = ResponseType.Success;
                _db.DeleteProduct(id);
                return Ok(ResponseHandler.GetAppResponse(type, "Delete Successfully"));

            }
            catch (Exception ex)
            {
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));

            }
        }

        [HttpPost]
        [Route("api/[controller]/SaveProduct")]
        public IActionResult SaveProduct([FromBody] ProductModel model)
        {
            ResponseType type = ResponseType.Success;

            try
            {
                _db.SaveProduct(model);
                return Ok(ResponseHandler.GetAppResponse(type, model));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }

        [HttpPost]
        [Route("api/[controller]/AddImage")]

        public async Task<IActionResult> PostWithImage([FromForm] ProductImage p)
        {
            ResponseType type = ResponseType.Success;

            try
            {
               await _db.SaveProductWithImageAsync(p);
                return Ok(ResponseHandler.GetAppResponse(type, p));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }

    }
}
