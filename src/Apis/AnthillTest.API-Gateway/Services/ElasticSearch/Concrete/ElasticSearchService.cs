﻿using AnthillTest.API_Gateway.Models.LogModels;
using AnthillTest.API_Gateway.Services.ElasticSearch.Abstract;
using Elasticsearch.Net;
using Nest;
using Polly;

namespace AnthillTest.API_Gateway.Services.ElasticSearch.Concrete;

public class ElasticSearchService : IElasticSearchService
{
    private IElasticClient _client;
    private IConfiguration _configuration;
    private ElasticSearchLogOptions _options;
    private ILogger<ElasticSearchService> _logger;

    public ElasticSearchService(IConfiguration configuration, ILogger<ElasticSearchService> logger)
    {
        _configuration = configuration;
        _options = _configuration.GetSection("ElasticSearchLogOptions").Get<ElasticSearchLogOptions>()!;

        _client = GetClient();
        _logger = logger;
    }

    private ElasticClient GetClient()
    {
        var settings = new ConnectionSettings(new Uri(_options.ConnectionString));
        return new ElasticClient(settings);
    }

    public async Task<bool> IndexExistsAsync(string index)
    {
        var indexExists = await _client.Indices.ExistsAsync(index);
        return indexExists.Exists;
    }

    public async Task<bool> CreateIndexAsync<T>(string index) where T : class
    {
        var indexExists = await _client.Indices.ExistsAsync(index);
        if (!indexExists.Exists)
        {
            await Polly.Policy.Handle<Exception>().RetryAsync(3, (exception, retryCount) =>
            {
                _logger.LogError(exception, $"Failed to create index '{index}', retry count: {retryCount}");
            })
            .ExecuteAsync(async () =>
            {
                var createResult = await _client.Indices.CreateAsync(index, c =>
                c.Map<T>(m => m.AutoMap()));
            });

            var existingIndex = await _client.Indices.ExistsAsync(index);
            if (existingIndex.Exists)
                return true;
            return false;
        }
        return true;
    }

    public async Task<bool> DeleteIndexAsync(string index)
    {
        var indexExists = await _client.Indices.ExistsAsync(index);
        if (indexExists.Exists)
        {
            await Polly.Policy.Handle<Exception>().RetryAsync(3, (exception, retryCount) =>
            {
                _logger.LogError(exception, $"Failed to create index '{index}', retry count: {retryCount}");
            })
            .ExecuteAsync(async () =>
            {
                await _client.Indices.DeleteAsync(index);
            });

            var existingIndex = await _client.Indices.ExistsAsync(index);
            if (!existingIndex.Exists)
                return true;

            return false;
        }

        return true;
    }

    public async Task<List<T>> GetAllAsync<T>(string index) where T : class
    {
        var result = await _client.SearchAsync<T>(s => s.Index(index).
                                                            Query(q => q.
                                                                MatchAll()));

        return result.Documents.ToList() ?? new List<T>();
    }

    public async Task<List<T>> QueryAsync<T>(string index, QueryContainer predicate) where T : class
    {
        var result = await _client.SearchAsync<T>(s => s.Index(index).
                                                            Query(q => predicate));
        return result.Documents.ToList() ?? new List<T>();
    }

    public async Task<bool> CreateOrUpdateAsync<T>(string index, T document) where T : class
    {
        var indexResponse = await _client.IndexAsync(document, idx => idx.Index(index).OpType(OpType.Index));
        return indexResponse.IsValid;
    }

    public async Task<bool> DeleteAsync<T>(string index, string key) where T : class
    {
        var response = await _client.DeleteAsync<T>(key, d => d.Index(index));
        return response.IsValid;
    }
}
