using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AuthServer.Core.Service
{
    public interface IServiceRepository<TEntity, TDto> where TEntity : class where TDto : class
    {
        Task<Response<TDto>> GetByIdAsync(int id);

        Task<Response<IEnumerable<TDto>>> GetAllAsync();

        Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate);

        Task<Response<TDto>> AddAsync(TDto entity);

        Task<Response<NoDataDto>> Remove(int id);

        Task<Response<TDto>> Update(TDto entity, int id);
    }
}
