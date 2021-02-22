using System;
using System.Collections.Generic;
using GraphQL.Resolvers;

namespace GraphQL.Types
{
    public interface IInterfaceGraphType : IAbstractGraphType, IComplexGraphType
    {
    }

    public class InterfaceGraphType<TSource> : ComplexGraphType<TSource>, IInterfaceGraphType
    {
        private readonly List<IObjectGraphType> _possibleTypes = new List<IObjectGraphType>();

        public IEnumerable<IObjectGraphType> PossibleTypes => _possibleTypes;

        public Func<object, IObjectGraphType> ResolveType { get; set; }

        public void AddPossibleType(IObjectGraphType type)
        {
            _possibleTypes.Fill(type);
        }
    }

    public class InterfaceGraphType : InterfaceGraphType<object>
    {
        public new FieldType Field<TGraphType>(
            string name,
            string description = null,
            QueryArguments arguments = null,
            Func<ResolveFieldContext<object>, object> resolve = null,
            string deprecationReason = null)
            where TGraphType : IGraphType
        {
            return AddField(new FieldType
            {
                Name = name,
                Description = description,
                DeprecationReason = deprecationReason,
                Type = typeof(TGraphType),
                Arguments = arguments,
                Resolver = resolve != null
                    ? new FuncFieldResolver<object, object>(resolve)
                    : null,
            });
        }
    }
}
