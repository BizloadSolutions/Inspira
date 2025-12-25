using System.Collections.Generic;
using System.Threading.Tasks;
using Inspira.Domain.Entities;
using Inspira.Domain.Repositories;
using MongoDB.Driver;

namespace Inspira.Infrastructure.Repositories;

public class FormRepository : IFormRepository
{
    private readonly IMongoCollection<Form> _collection;

    public FormRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Form>("forms");
    }

    public async Task CreateAsync(Form form) => await _collection.InsertOneAsync(form);

    public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(f => f.FormId == id);

    public async Task<IEnumerable<Form>> GetAllAsync()
    {
        var cursor = await _collection.FindAsync(FilterDefinition<Form>.Empty);
        return await cursor.ToListAsync();
    }

    public async Task<Form?> GetByIdAsync(int id)
    {
        var cursor = await _collection.FindAsync(f => f.FormId == id);
        return await cursor.FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(Form form) => await _collection.ReplaceOneAsync(f => f.FormId == form.FormId, form);
}
