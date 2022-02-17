using AuthServer.Core.DataAccess.EntityFramework;
using AuthServer.Core.Service;
using AuthServer.Core.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class ServiceRepository<TEntity, TDto> : IServiceRepository<TEntity, TDto> where TEntity : class, new() where TDto : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEfEntityRepositoryBase<TEntity> _efEntityRepositoryBase;

        public ServiceRepository(IUnitOfWork unitOfWork, IEfEntityRepositoryBase<TEntity> efEntityRepositoryBase)
        {
            _unitOfWork = unitOfWork;
            _efEntityRepositoryBase = efEntityRepositoryBase;
        }

        public async Task<Response<TDto>> AddAsync(TDto entity)
        {
            var newEntity = ObjectMapper.Mapper.Map<TEntity>(entity);
            await _efEntityRepositoryBase.AddAsync(newEntity);
            await _unitOfWork.CommitAsync();
            var newDto = ObjectMapper.Mapper.Map<TDto>(newEntity);
            return new Response<TDto>(newDto, 200);
        }

        public async Task<Response<IEnumerable<TDto>>> GetAllAsync()
        {
            var products = ObjectMapper.Mapper.Map<List<TDto>>(await _efEntityRepositoryBase.GetAllAsync());
            return new Response<IEnumerable<TDto>>(products, 200);
        }

        public async Task<Response<TDto>> GetByIdAsync(int id)
        {
            var product = await _efEntityRepositoryBase.GetByIdAsync(id);
            if (product == null)
                return new Response<TDto>("Product not found.", 404, true);
            return new Response<TDto>(ObjectMapper.Mapper.Map<TDto>(product), 200);
        }

        public async Task<Response<NoDataDto>> Remove(int id)
        {
            var isExistEntity = await _efEntityRepositoryBase.GetByIdAsync(id);
            if (isExistEntity == null)
                return new Response<NoDataDto>("Product not found", 404, true);

            _efEntityRepositoryBase.Remove(isExistEntity);
            await _unitOfWork.CommitAsync();
            return new Response<NoDataDto>(204);
        }

        public async Task<Response<NoDataDto>> Update(TDto entity, int id)
        {
            var isExistEntity = await _efEntityRepositoryBase.GetByIdAsync(id);
            if (isExistEntity == null)
                return new Response<NoDataDto>("Product not found", 404, true);

            var updateEntiy = ObjectMapper.Mapper.Map<TEntity>(entity);
            _efEntityRepositoryBase.Update(updateEntiy);
            await _unitOfWork.CommitAsync();

            return new Response<NoDataDto>(204);
        }

        public async Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate)
        {
            var list = _efEntityRepositoryBase.Where(predicate);
            list.Skip(4).Take(5);

            //committed to database with ToListAsync();
            return new Response<IEnumerable<TDto>>(ObjectMapper.Mapper.Map<IEnumerable<TDto>>(await list.ToListAsync()), 200);
        }
    }
}
