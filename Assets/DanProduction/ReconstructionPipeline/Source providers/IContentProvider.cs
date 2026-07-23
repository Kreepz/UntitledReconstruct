using System.Collections.Generic;
using System.Threading.Tasks;

public interface IContentProvider
{
    Task<List<LevelMetadata>> GetLevelCatalogueAsync();
}
