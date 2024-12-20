using AutService.JwAuthLogin.Application.Contracts;
using AutService.JwAuthLogin.Infrastructure.Context;

namespace AutService.JwAuthLogin.Infrastructure.Repositories
{
    internal sealed class DatabaseRepository<T>(

        DatabaseContext context) : IDatabaseRepository<T> where T : class
    {
        private readonly DatabaseContext _context = context;

        /// <summary>
        /// Busca un registro por una expresión y de manera síncrona
        /// </summary>
        /// <param name="predicate">Expresión</param>
        /// <returns>Listado de Entidades</returns>
        public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
        {

            try
            {
                return _context.Set<T>().Where(predicate);
            }
            catch (Exception exception)
            {
                return Enumerable.Empty<T>().AsQueryable();
            }
        }

        /// <summary>
        /// Agrega un registro de manera síncrona
        /// </summary>
        /// <param name="entity">Entidad</param>
        /// <returns>Entidad</returns>
        public T Add(T entity)
        {

            try
            {
                _context.Set<T>().Add(entity);
                _context.SaveChanges();
            }
            catch (Exception exception)
            {

            }
            return entity;
        }
        /// <summary>
        /// Agrega múltiples registros de manera asíncrona
        /// </summary>
        /// <param name="entities">Lista de entidades</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de Entidades</returns>
        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.Set<T>().AddRangeAsync(entities, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exception)
            {

            }
            return entities;
        }

        /// <summary>
        /// Actualiza un registro de manera asíncrona
        /// </summary>
        /// <param name="entity">Entidad</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Entidad</returns>
        public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            try
            {
                _context.Set<T>().Update(entity);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exception)
            {

            }
            return entity;
        }

        /// <summary>
        /// Elimina un registro de manera asíncrona
        /// </summary>
        /// <param name="entity">Entidad</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Entidad</returns>
        public async Task<T> DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            var watch = Stopwatch.StartNew();

            try
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exception)
            {

            }
            return entity;
        }
    }
}
