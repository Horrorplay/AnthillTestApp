using Nest;

namespace AnthillTest.API_Gateway.Services.ElasticSearch.Abstract;

public interface IElasticSearchService
{
    Task<bool> IndexExistsAsync(string index);
    Task<bool> CreateIndexAsync<T>(string index) where T : class;
    Task<bool> DeleteIndexAsync(string index);
    Task<List<T>> GetAllAsync<T>(string index) where T : class;
    Task<List<T>> QueryAsync<T>(string query, QueryContainer predicate) where T : class;
    Task<bool> CreateOrUpdateAsync<T>(string index, T document) where T : class;
    Task<bool> DeleteAsync<T>(string index, string key) where T : class;
}
