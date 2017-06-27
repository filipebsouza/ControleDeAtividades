using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Data.Linq;
using System.Web;
using System.Reflection;
using Domain.Model;

namespace Repository.Logic
{
    public class GenericRepository<T> : IDisposable
        where T : class
    {
        private DbContext _entities;
        public DbContext Context
        {
            get { return _entities; }
            set { _entities = value; }
        }

        public GenericRepository()
        {
            AtualizarContexto();
        }

        private void AtualizarContexto()
        {
            ControleAtividadesEntities contexto = new ControleAtividadesEntities();

            _entities = contexto;
        }

        public IQueryable<T> Listar(bool atualizarContexto = false)
        {
            IQueryable<T> query = _entities.Set<T>();

            if (atualizarContexto)
                AtualizarContexto();

            return query;
        }

        public IQueryable<T> Listar(System.Linq.Expressions.Expression<Func<T, bool>> predicate, bool atualizarContexto = false)
        {
            IQueryable<T> query;

            if (predicate == null)
                query = _entities.Set<T>();
            else
                query = _entities.Set<T>().Where(predicate);

            if (atualizarContexto)
                AtualizarContexto();

            return query;
        }

        public T Obter(System.Linq.Expressions.Expression<Func<T, bool>> predicate, bool atualizarContexto = false)
        {
            T query;

            if (predicate == null)
                query = _entities.Set<T>().SingleOrDefault();
            else
                query = _entities.Set<T>().Where(predicate).SingleOrDefault();

            if (atualizarContexto)
                AtualizarContexto();

            return query;
        }

        public void Incluir(T entity)
        {
            _entities.Set<T>().Add(entity);
        }

        public void Deletar(T entity)
        {
            _entities.Set<T>().Remove(entity);
        }

        public void Alterar(T entity)
        {
            _entities.Entry(entity).State = EntityState.Modified;
        }

        public void Salvar(bool atualizarContexto = false)
        {
            _entities.SaveChanges();

            if (atualizarContexto)
                AtualizarContexto();
        }

        public void Dispose()
        {
            _entities.Dispose();
        }
    }

    public static class GenericRepositoryExtension
    {
        [Obsolete("Use o método OrderByIQueryableOrdered")]
        public static IQueryable<T> OrderByIQueryable<T>(this IQueryable<T> query, string propriedade)
        {
            string ordenacao = TipoOrdenacao(propriedade);

            var param = Expression.Parameter(typeof(T), "p");
            var prop = Expression.Property(param, propriedade);
            var exp = Expression.Lambda(prop, param);

            Type[] tipos = new Type[] { query.ElementType, exp.Body.Type };

            var queryFinal = Expression.Call(typeof(Queryable), ordenacao, tipos, query.Expression, exp);

            return query.Provider.CreateQuery<T>(queryFinal);
        }

        [Obsolete("Use o método OrderByListOrdered")]
        public static List<T> OrderByList<T>(this List<T> query, string propriedade)
        {
            string ordenacao = TipoOrdenacao(propriedade);

            var param = Expression.Parameter(typeof(T), "p");
            var prop = Expression.Property(param, propriedade);
            var exp = Expression.Lambda(prop, param);

            var queryIqueryable = query.AsQueryable();

            Type[] tipos = new Type[] { queryIqueryable.ElementType, exp.Body.Type };

            var queryFinal = Expression.Call(typeof(Queryable), ordenacao, tipos, queryIqueryable.Expression, exp);

            return queryIqueryable.Provider.CreateQuery<T>(queryFinal).ToList();
        }

        [Obsolete("Use o método OrderByIEnumerableOrdered")]
        public static IEnumerable<T> OrderByIEnumerable<T>(this IEnumerable<T> query, string propriedade)
        {
            string ordenacao = TipoOrdenacao(propriedade);

            var param = Expression.Parameter(typeof(T), "p");
            var prop = Expression.Property(param, propriedade);
            var exp = Expression.Lambda(prop, param);

            var queryIqueryable = query.AsQueryable();

            Type[] tipos = new Type[] { queryIqueryable.ElementType, exp.Body.Type };

            var queryFinal = Expression.Call(typeof(Queryable), ordenacao, tipos, queryIqueryable.Expression, exp);

            return queryIqueryable.Provider.CreateQuery<T>(queryFinal);
        }

        public static IOrderedQueryable<T> OrderByIQueryableOrdered<T>(this IQueryable<T> query, string propertyName, string ordenacao = null)
        {
            var type = typeof(T);
            var property = type.GetProperty(propertyName);
            var parameter = Expression.Parameter(type, "p");

            if (property == null)
            {
                throw new Exception("Essa coluna não existe.");
            }
            else
            {
                var propertyReference = Expression.Property(parameter, property);

                var sortExpression = Expression.Call(
                  typeof(Queryable),
                  ((ordenacao == null || ordenacao == "ASC") ? "OrderBy" : "OrderByDescending"),
                  new Type[] { type, property.PropertyType },
                  query.Expression, Expression.Quote(Expression.Lambda(Expression.MakeMemberAccess(parameter, property), parameter)));

                return query.Provider.CreateQuery<T>(sortExpression) as IOrderedQueryable<T>;
            }
        }

        public static IOrderedQueryable<T> OrderByListOrdered<T>(this List<T> query, string propertyName, string ordenacao = null)
        {
            var type = typeof(T);
            var property = type.GetProperty(propertyName);
            var parameter = Expression.Parameter(type, "p");
            var propertyReference = Expression.Property(parameter, property); //p.ProductName

            var queryT = query.AsQueryable();

            var sortExpression = Expression.Call(
              typeof(Queryable),
                ((ordenacao == null || ordenacao == "ASC") ? "OrderBy" : "OrderByDescending"),
              new Type[] { type, property.PropertyType },
              queryT.Expression, Expression.Quote(Expression.Lambda(Expression.MakeMemberAccess(parameter, property), parameter)));

            return queryT.Provider.CreateQuery<T>(sortExpression) as IOrderedQueryable<T>;
        }

        public static IOrderedQueryable<T> OrderByIEnumerableOrdered<T>(this IEnumerable<T> query, string propertyName, string ordenacao = null)
        {
            var type = typeof(T);
            var property = type.GetProperty(propertyName);
            var parameter = Expression.Parameter(type, "p");
            var propertyReference = Expression.Property(parameter, property); //p.ProductName

            var queryT = query.AsQueryable();

            var sortExpression = Expression.Call(
              typeof(Queryable),
              ((ordenacao == null || ordenacao == "ASC") ? "OrderBy" : "OrderByDescending"),
              new Type[] { type, property.PropertyType },
              queryT.Expression, Expression.Quote(Expression.Lambda(Expression.MakeMemberAccess(parameter, property), parameter)));

            return queryT.Provider.CreateQuery<T>(sortExpression) as IOrderedQueryable<T>;
        }

        private static string TipoOrdenacao(string propriedade)
        {
            string ordenacao;

            if (HttpContext.Current.Session["GRTipoOrdenacao"] != null)
            {
                if (propriedade == HttpContext.Current.Session["GRTipoOrdenacao"].ToString())
                {
                    ordenacao = "OrderByDescending";

                    HttpContext.Current.Session["GRTipoOrdenacao"] = null;
                }
                else
                {
                    ordenacao = "OrderBy";

                    HttpContext.Current.Session["GRTipoOrdenacao"] = propriedade;
                }
            }
            else
            {
                ordenacao = "OrderBy";

                HttpContext.Current.Session["GRTipoOrdenacao"] = propriedade;
            }

            return ordenacao;
        }

    }

    public class GenericDistinct<T> : IEqualityComparer<T>
    {
        private PropertyInfo propriedade;

        /// <summary>
        /// Realiza o distinct em cima de um atributo.
        /// </summary>
        /// <param name="p">Passe o nome do atributo.</param>
        public GenericDistinct(string p)
        {
            var tipo = typeof(T);
            propriedade = tipo.GetProperty(p);
        }

        public bool Equals(T p1, T p2)
        {
            return propriedade.GetValue(p1, null).Equals(propriedade.GetValue(p2, null));
        }

        public int GetHashCode(T obj)
        {
            return obj == null ? 0 : propriedade.GetValue(obj, null).GetHashCode();
        }
    }

}