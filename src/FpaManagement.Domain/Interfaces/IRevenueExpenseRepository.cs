using FpaManagement.Domain.Entities;

namespace FpaManagement.Domain.Interfaces;

public interface IRevenueExpenseRepository : IGenericRepository<Revenue>, IGenericRepository<Expense>
{
    // TODO:
}
