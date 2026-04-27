using Catalog.API.Models;
using Marten;
using Marten.Schema;

namespace Catalog.API.Data;

public class CatalogInitialData : IInitialData
{
     public async Task Populate(IDocumentStore store, CancellationToken cancellationToken)
     {
          using var session = store.LightweightSession();

          if (await session.Query<Product>().AnyAsync(cancellationToken))
          {
               return;
          }

          session.Store<Product>(GetPreconfiguredProducts());
          await session.SaveChangesAsync(cancellationToken);
     }

     private static IEnumerable<Product> GetPreconfiguredProducts() => new List<Product>()
     {
          new Product
          {
               Name = "IPhone 13",
               Category = new List<string> { "Smart Phone" },
               Description =
                    "IPhone 13 is the latest version of Apple iPhone. It has a sleek design and powerful features.",
               ImageFile = "product-1.png",
               Price = 999.99m
          },
          new Product
          {
               Name = "Samsung Galaxy S21",
               Category = new List<string> { "Smart Phone" },
               Description =
                    "Samsung Galaxy S21 is the latest version of Samsung Galaxy. It has a sleek design and powerful features.",
               ImageFile = "product-2.png",
               Price = 899.99m
          },
          new Product
          {
               Name = "MacBook Pro",
               Category = new List<string> { "Laptop" },
               Description =
                    "MacBook Pro is the latest version of Apple MacBook. It has a sleek design and powerful features.",
               ImageFile = "product-3.png",
               Price = 1999.99m
          }
     };
}
   
