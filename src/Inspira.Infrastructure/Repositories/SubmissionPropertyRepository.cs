using System.Collections.Generic;
using System.Threading.Tasks;
using Inspira.Domain.Entities;
using Inspira.Domain.Repositories;
using MongoDB.Driver;

namespace Inspira.Infrastructure.Repositories;

public class SubmissionPropertyRepository : ISubmissionPropertyRepository
{
    private readonly IMongoCollection<SubmissionProperty> _collection;

    public SubmissionPropertyRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<SubmissionProperty>("submissionProperties");
    }

    public async Task CreateAsync(SubmissionProperty submissionProperty) => await _collection.InsertOneAsync(submissionProperty);

    public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(p => p.SubmissionPropertyId == id);

    public async Task<IEnumerable<SubmissionProperty>> GetAllAsync()
    {
        var cursor = await _collection.FindAsync(FilterDefinition<SubmissionProperty>.Empty);
        return await cursor.ToListAsync();
    }

    public async Task<SubmissionProperty?> GetByIdAsync(int id)
    {
        var cursor = await _collection.FindAsync(p => p.SubmissionPropertyId == id);
        return await cursor.FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(SubmissionProperty submissionProperty) => await _collection.ReplaceOneAsync(p => p.SubmissionPropertyId == submissionProperty.SubmissionPropertyId, submissionProperty);
}
