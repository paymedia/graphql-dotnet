using System.Collections.Generic;
using System.Linq;
using GraphQL.Language.AST;

namespace GraphQL.Validation
{
    public class BasicVisitor
    {
        private readonly IEnumerable<INodeVisitor> _visitors;

        public BasicVisitor(params INodeVisitor[] visitors)
        {
            _visitors = visitors;
        }

        public void Visit(INode node)
        {
            if (node == null)
            {
                return;
            }

            var visitorsList = _visitors.ToList();

            foreach(var visitor in visitorsList)
                visitor.Enter(node);

            if (node.Children != null)
            {
                foreach (var child in node.Children)
                    Visit(child);
            }

            for (var i = visitorsList.Count - 1; i >= 0; i--)
            {
                visitorsList[i].Leave(node);
            }
        }
    }
}
