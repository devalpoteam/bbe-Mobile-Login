using System.Linq.Expressions;

namespace AutService.JwAuthLogin.Application.Contracts
{
    public interface IDatabaseRepository<T> where T : class
    {
        /// <summary>
        /// Busca un registro por una expresión y de manera síncrona
        /// </summary>
        /// <param name="predicate">Expresión</param>
        /// <returns>Listado de Entidades</returns>
        public IQueryable<T> Find(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Agrega un registro de manera síncrona
        /// </summary>
        /// <param name="entity">Entidad</param>
        /// <returns>Entidad</returns>
        public T Add(T entity);

        /// <summary>
        /// Agrega múltiples registros de manera asíncrona
        /// </summary>
        /// <param name="entities">Lista de entidades</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de Entidades</returns>
        public Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Actualiza un registro de manera asíncrona
        /// </summary>
        /// <param name="entity">Entidad</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Entidad</returns>
        public Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Elimina un registro de manera asíncrona
        /// </summary>
        /// <param name="entity">Entidad</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Entidad</returns>
        public Task<T> DeleteAsync(T entity, CancellationToken cancellationToken = default);

    }
}
