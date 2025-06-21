
using Catalog.API.Products.GetProductById;
using JasperFx.CodeGeneration.Frames;

namespace Catalog.API.Products.UpdateProduct;

public record UpdateProductCommand(Guid Id, string Name, List<string> Category, string Description, string ImageFile, decimal Price)
	: ICommand<UpdateProductResult>;
public record UpdateProductResult(bool IsSuccess);
internal class UpdateProductCommandHandler
	(IDocumentSession session, ILogger<GetProductByIdQueryHandle> logger)
	: ICommandHandler<UpdateProductCommand, UpdateProductResult>
{
	public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
	{
		logger.LogInformation("UpdateProductHandler.Handle called with {@Command}", command);

		var product = await session.LoadAsync<Product>(command.Id, cancellationToken);

		if (product is null)
		{
			throw new ProductNotFoundException();
		}

		product.Name = command.Name;
		product.Category = command.Category;
		product.Description = command.Description;
		product.Price = command.Price;
		product.ImageFile = command.ImageFile;

		session.Update(product);
		await session.SaveChangesAsync(cancellationToken);

		return new UpdateProductResult(true);
	}
}
