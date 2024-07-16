using backend.EfCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace backend.Model
{
    public class DbHelper
    {
        private EF_DataContext _context;
        public DbHelper(EF_DataContext context)
        {
            _context = context;
        }
        public List<Product> GetProducts()
        {
            return _context.Products.ToList();
        }

        //public List<OrderModel> GetOrders()
        //{
        //    List<OrderModel> response = new List<OrderModel>();
        //    var dataList = _context.Orders.ToList();

        //    dataList.ForEach(row => response.Add(new OrderModel()
        //    {
        //        id = row.id,
        //        name = row.name,
        //        address = row.address,
        //        phone = row.phone
        //    }));
        //    return response;
        //}

        public async Task<List<OrderData>> GetAllOrders()
        {
            var query = await (from o in _context.Orders
                               join op in _context.OrderProducts on o.id equals op.orderId
                               join p in _context.Products on op.productId equals p.id
                               orderby o.id ascending
                               select new
                               {
                                   Order = o,
                                   ProductId = p.id
                               }).ToListAsync();

            var orderDataList = new List<OrderData>();

            foreach (var item in query)
            {
                var orderData = orderDataList.FirstOrDefault(od => od.OrderModel.id == item.Order.id);

                if (orderData == null)
                {
                    orderData = new OrderData
                    {
                        OrderModel = new OrderModel
                        {
                            id = item.Order.id,
                            name = item.Order.name,
                            address = item.Order.address,
                            phone = item.Order.phone
                        },
                        ProductIds = new List<int> { item.ProductId }
                    };

                    orderDataList.Add(orderData);
                }
                else
                {
                    orderData.ProductIds.Add(item.ProductId);
                }
            }

            return orderDataList;
        }




        public ProductModel GetProductById(int id)
        {
            ProductModel response = new ProductModel();
            var row = _context.Products.Where(d => d.id.Equals(id)).FirstOrDefault();
            return new ProductModel()
            {
                brand = row.brand,
                id = row.id,
                name = row.name,
                price = row.price,
                size = row.size,
                image = row.image
            };
        }


        public void SaveOrder(OrderModel orderModel, List<int> productIds)
        {
            Order dbOrder;

            if (orderModel.id > 0)
            {
                // PUT - Update existing order
                dbOrder = _context.Orders
                                  .Include(o => o.OrderProducts)
                                  .FirstOrDefault(d => d.id == orderModel.id);

                if (dbOrder != null)
                {
                    dbOrder.phone = orderModel.phone;
                    dbOrder.address = orderModel.address;
                    dbOrder.name = orderModel.name;

                    // Update the product association
                    dbOrder.OrderProducts.Clear(); // Remove existing associations

                    foreach (var productId in productIds)
                    {
                        var product = _context.Products.FirstOrDefault(f => f.id == productId);
                        if (product != null)
                        {
                            dbOrder.OrderProducts.Add(new Order_Product { Order = dbOrder, Product = product });
                        }
                    }
                }
            }
            else
            {
                // POST - Create new order
                dbOrder = new Order
                {
                    phone = orderModel.phone,
                    address = orderModel.address,
                    name = orderModel.name,
                    OrderProducts = new List<Order_Product>()
                };

                foreach (var productId in productIds)
                {
                    var product = _context.Products.FirstOrDefault(f => f.id == productId);
                    if (product != null)
                    {
                        dbOrder.OrderProducts.Add(new Order_Product { Order = dbOrder, Product = product });
                    }
                }

                _context.Orders.Add(dbOrder);
            }

            _context.SaveChanges();
        }




        public void DeleteOrder(int orderId)
        {
            var order = _context.Orders.Include(o => o.OrderProducts).FirstOrDefault(o => o.id == orderId);

            if (order != null)
            {
                // Xóa các bản ghi liên kết từ bảng trung gian (OrderProducts).
                _context.OrderProducts.RemoveRange(order.OrderProducts);

                // Xóa đơn hàng.
                _context.Orders.Remove(order);

                // Lưu thay đổi vào cơ sở dữ liệu.
                _context.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Order not found with the given id.");
            }
        }


        public void DeleteProduct(int productId)
        {
            var product = _context.Products.Include(p => p.OrderProducts).FirstOrDefault(p => p.id == productId);

            if (product != null)
            {
                // Xóa tất cả các bản ghi trong bảng trung gian (OrderProducts) liên quan đến sản phẩm này.
                _context.OrderProducts.RemoveRange(product.OrderProducts);

                // Xóa sản phẩm.
                _context.Products.Remove(product);

                // Lưu thay đổi vào cơ sở dữ liệu.
                _context.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Product not found with the given id.");
            }
        }


        public void SaveProduct(ProductModel productModel)
        {
            Product dbTable = new Product();
            if (productModel.id > 0)
            {
                dbTable = _context.Products.Where(d => d.id.Equals(productModel.id)).FirstOrDefault();
                if (dbTable != null)
                {
                    dbTable.name = productModel.name;
                    dbTable.brand = productModel.brand;
                    dbTable.price = productModel.price;
                    dbTable.size = productModel.size;
                }
            }
            else
            {
                dbTable.name = productModel.name;
                dbTable.brand = productModel.brand;
                dbTable.price = productModel.price;
                dbTable.size = productModel.size;
                _context.Products.Add(dbTable);
            }
            _context.SaveChanges();
        }

        public async Task SaveProductWithImageAsync(ProductImage productImage)
        {
            Product dbTable = new Product();
            if (productImage.id > 0)
            {
                dbTable = _context.Products.Where(d => d.id.Equals(productImage.id)).FirstOrDefault();
                if (dbTable != null)
                {
                    dbTable.name = productImage.name;
                    dbTable.brand = productImage.brand;
                    dbTable.price = productImage.price;
                    dbTable.size = productImage.size;
                }
            }
            else
            {
                dbTable.name = productImage.name;
                dbTable.brand = productImage.brand;
                dbTable.price = productImage.price;
                dbTable.size = productImage.size;

                if (productImage.image.Length > 0)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", productImage.image.FileName);
                    using (var stream = System.IO.File.Create(path))
                    {
                        await productImage.image.CopyToAsync(stream);
                    }
                    dbTable.image = "/images/" + productImage.image.FileName;
                }
                else
                {
                    dbTable.image = "";
                }
                _context.Products.Add(dbTable);


            }
            _context.SaveChanges();
        }
        }
    }
