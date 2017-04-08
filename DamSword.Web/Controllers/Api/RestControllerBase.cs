using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using DamSword.Common;
using DamSword.Data;
using DamSword.Data.Entities;
using DamSword.Data.Repositories;
using DamSword.Services;
using DamSword.Services.Exceptions;
using DamSword.Web.Attributes;
using DamSword.Web.Exceptions;
using DamSword.Web.DTO;
using DamSword.Web.DTO.Objects;
using Microsoft.AspNetCore.Mvc;

namespace DamSword.Web.Controllers.Api
{
    [Authorize]
    public abstract class RestControllerBase<TEntity, TObject> : ApiControllerBase
        where TEntity : class, IEntity, new()
        where TObject : class, IObject
    {
        protected readonly IEntityRepository<TEntity> Repository;
        protected readonly IEntityService<TEntity> Service;
        protected readonly IUnitOfWork UnitOfWork;

        protected virtual UserPermissions? CreatePermissions { get; } = null;
        protected virtual UserPermissions? ReadPermissions { get; } = null;
        protected virtual UserPermissions? UpdatePermissions { get; } = null;
        protected virtual UserPermissions? DeletePermissions { get; } = null;

        protected virtual Expression<Func<TEntity, bool>> Predicate => e => true;
        protected abstract Expression<Func<TEntity, TObject>> ReadMap { get; }
        protected abstract Expression<Func<TObject, TEntity>> WriteMap { get; }

        protected RestControllerBase(IEntityRepository<TEntity> repository, IEntityService<TEntity> service, IUnitOfWork unitOfWork)
        {
            Repository = repository;
            Service = service;
            UnitOfWork = unitOfWork;
        }

        [HttpGet]
        public virtual IActionResult List([FromQuery] Request<GenericListFetch> request)
        {
            if (ReadPermissions.HasValue)
                this.Require(ReadPermissions.Value);

            var items = Repository
                .QueryApiListRequest(request.Data)
                .Where(Predicate)
                .Select(ReadMap)
                .ToArray();

            return this.ApiListResult(request, items);
        }

        [HttpGet]
        public virtual IActionResult Details([FromQuery] Request<long> request)
        {
            if (ReadPermissions.HasValue)
                this.Require(ReadPermissions.Value);
            
            var item = Repository
                .Select(Predicate)
                .Where(e => e.Id == request.Data)
                .Select(ReadMap)
                .SingleOrDefault();

            if (item == null)
                throw new RequestException(HttpStatusCode.NotFound);

            return this.ApiResult(request, item);
        }

        [HttpPost]
        public virtual IActionResult Create([FromBody] Request<TObject> request)
        {
            if (CreatePermissions.HasValue)
                this.Require(CreatePermissions.Value);

            request.Data.Id = 0;
            var id = Store(request.Data, WriteMap);

            return this.ApiResult(request, id);
        }

        [HttpPut]
        public virtual IActionResult Update([FromBody] Request<TObject> request)
        {
            if (UpdatePermissions.HasValue)
                this.Require(UpdatePermissions.Value);

            Store(request.Data, WriteMap);
            return this.ApiSuccessResult(request);
        }

        [HttpDelete]
        public virtual IActionResult Delete([FromQuery] Request<long> request)
        {
            if (DeletePermissions.HasValue)
                this.Require(DeletePermissions.Value);

            Service.Delete(request.Data);
            return this.ApiSuccessResult(request);
        }

        protected virtual long Store(TObject @object, Expression<Func<TObject, TEntity>> mapper)
        {
            if (@object == null)
                throw new ArgumentNullException(nameof(@object));

            var memberInitExpression = mapper.Body as MemberInitExpression;
            if (memberInitExpression?.Type != typeof(TEntity))
                throw new ArgumentException($"Must contain \"{typeof(TEntity).Name}\" Entity {nameof(MemberInitExpression).SplitUpperCaseBySpace()}.", nameof(mapper));

            var bindings = memberInitExpression.Bindings.Select(b => b as MemberAssignment).ToArray();
            if (bindings.Any(b => b == null))
                throw new ArgumentException($"Must contain only {nameof(MemberAssignment).SplitUpperCaseBySpace()} Expressions.", nameof(mapper));

            var currentEntity = @object.Id != 0 ? Repository.GetById(@object.Id) : new TEntity();
            var mappedEntity = mapper.Compile().Invoke(@object);
            var entityType = typeof(TEntity);

            foreach (var binding in bindings)
            {
                var property = entityType.GetProperty(binding.Member.Name);
                var value = property.GetValue(mappedEntity);
                property.SetValue(currentEntity, value);
            }

            try
            {
                Service.Save(currentEntity);
            }
            catch (EntityValidationException<TEntity> validationException)
            {
                var binding = bindings.SingleOrDefault(b => b.Member.Name == validationException.FieldName);
                if (binding == null)
                    throw new InvalidOperationException($"Non-mapped property \"{validationException.FieldName}\" has invalid value for Entity \"{typeof(TEntity).Name}\".");

                var parameter = mapper.Parameters.Single();
                var invalidMemberNames = binding.Expression.Query(e => e.IsMemberOfParameter(parameter))
                    .Select(e => e.AsMemberExpressionSkipConvert()?.Member.Name)
                    .ToArray();

                throw new RestObjectValidationException(invalidMemberNames, validationException.Message);
            }
            
            UnitOfWork.Commit();
            return currentEntity.Id;
        }
    }
}