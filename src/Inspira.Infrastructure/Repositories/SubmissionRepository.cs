using System.Collections.Generic;
using System.Threading.Tasks;
using Inspira.Domain.Entities;
using Inspira.Domain.Repositories;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Inspira.Infrastructure.Repositories;

public class SubmissionRepository : ISubmissionRepository
{
    private readonly IMongoCollection<Submission> _collection;

    public SubmissionRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Submission>("submissions");
    }

    public async Task CreateAsync(Submission submission) => await _collection.InsertOneAsync(submission);

    public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(s => s.SubmissionId == id);

    public async Task<IEnumerable<Submission>> GetAllAsync()
    {
        var cursor = await _collection.FindAsync(FilterDefinition<Submission>.Empty);
        return await cursor.ToListAsync();
    }

    public async Task<Submission?> GetByIdAsync(int id)
    {
        var cursor = await _collection.FindAsync(s => s.SubmissionId == id);
        return await cursor.FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(Submission submission) => await _collection.ReplaceOneAsync(s => s.SubmissionId == submission.SubmissionId, submission);
}
