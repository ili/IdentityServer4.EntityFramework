using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using IdentityServer4.Models;
using LinqToDB.Extensions;
using LinqToDB.Mapping;

namespace IdentityServer4.Contrib.LinqToDB.Entities
{
	/// <summary>
	///     This class contains <see cref="MappingSchema" /> extensions for <see cref="IdentityServer4.Contrib.LinqToDB.Entities" />
	///     namespace
	/// </summary>
	public static class MappingExtensions
	{
		private static readonly Dictionary<Type, ConcurrentDictionary<Type, Func<object, object>>> _mappers =
			new Dictionary<Type, ConcurrentDictionary<Type, Func<object, object>>>();

		/// <summary>
		///     Applies default mappings for Entities for provided <see cref="MappingSchema" />
		/// </summary>
		/// <param name="schema"><see cref="MappingSchema" /> to apply mappings</param>
		/// <returns>
		///     <paramref name="schema" />
		/// </returns>
		public static MappingSchema ApplyDefaultEntitiesMappings(this MappingSchema schema)
		{
			var builder = schema.GetFluentMappingBuilder();

			builder
				.Entity<PersistedGrant>()
				.Property(_ => _.Key)
				.IsPrimaryKey()
				.Property(_ => _.ClientId)
				.HasSkipOnUpdate()
				.Property(_ => _.CreationTime)
				.Property(_ => _.Data)
				.Property(_ => _.Expiration)
				.Property(_ => _.SubjectId)
				.Property(_ => _.Type);

			lock (_mappers)
			{
				_mappers.Clear();
			}

			return schema;
		}

		/// <summary>
		///     Applies default mappings for Entities for provided <see cref="MappingSchema.Default" />
		/// </summary>
		/// <returns>
		///     <see cref="MappingSchema.Default" />
		/// </returns>
		public static MappingSchema ApplyDefaultEntitiesMappings()
		{
			return MappingSchema.Default.ApplyDefaultEntitiesMappings();
		}

		internal static TRes GetSimpleMap<TSource, TRes>(TSource source)
			where TRes : new()
		{
			ConcurrentDictionary<Type, Func<object, object>> mappers;

			lock (_mappers)
			{
				if (!_mappers.TryGetValue(typeof(TSource), out mappers))
				{
					mappers = new ConcurrentDictionary<Type, Func<object, object>>();
					_mappers.Add(typeof(TSource), mappers);
				}
			}

			Func<object, object> mapper;

			if (!mappers.TryGetValue(typeof(TRes), out mapper))
			{
				var osrc = Expression.Variable(typeof(object), "osrc");
				var src = Expression.Variable(typeof(TSource), "src");
				var res = Expression.Variable(typeof(TRes), "res");
				var srcDescriptor = MappingSchema.Default.GetEntityDescriptor(typeof(TSource));
				var destDescriptor = MappingSchema.Default.GetEntityDescriptor(typeof(TRes));


				var assignments = new List<Expression>();

				assignments.Add(Expression.Assign(src, Expression.Convert(osrc, typeof(TSource))));
				assignments.Add(Expression.Assign(res, Expression.New(typeof(TRes).GetDefaultConstructorEx())));

				foreach (var column in srcDescriptor.Columns)
				{
					if (!column.MemberType.IsScalar(false) || !column.MemberAccessor.HasGetter || column.IsIdentity)
						continue;

					var dest = destDescriptor.Columns.FirstOrDefault(_ => _.MemberName == column.MemberName);

					if (dest == null || !dest.MemberAccessor.HasSetter)
						continue;

					assignments.Add(Expression.Assign(Expression.PropertyOrField(res, dest.MemberName),
						Expression.PropertyOrField(src, dest.MemberName)));
				}

				var label = Expression.Label(typeof(TRes));
				assignments.Add(Expression.Return(label, res, typeof(TRes)));
				assignments.Add(Expression.Label(label, res));

				var body = Expression.Block(new[] {src, res}, assignments);
				var mapperEx = Expression.Lambda<Func<object, object>>(body, osrc);

				mapper = mapperEx.Compile();

				mappers.TryAdd(typeof(TRes), mapper);
			}


			return (TRes) mapper(source);
		}
	}
}