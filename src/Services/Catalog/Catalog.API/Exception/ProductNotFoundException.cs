namespace Catalog.API.Exception;
public class ProductNotFoundException : System.Exception
{
	public ProductNotFoundException() : base("Product not found!")
	{

	}

}
