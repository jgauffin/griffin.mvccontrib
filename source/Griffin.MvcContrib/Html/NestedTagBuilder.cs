using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Html
{
    /// <summary>
    /// Tag builder which can contain child tag builders
    /// </summary>
    public class NestedTagBuilder : TagBuilder
    {
        private LinkedList<NestedTagBuilder> _children = new LinkedList<NestedTagBuilder>();

        public NestedTagBuilder(string tagName) : base(tagName)
        {
        }

        public void AddChild(NestedTagBuilder builder)
        {
            _children.AddLast(builder);
        }

        public IEnumerable<NestedTagBuilder> Children
        {
            get { return _children; }
        }

        public override string ToString()
        {
            return ToString(TagRenderMode.Normal);
        }

        /// <summary>
        /// TagBuilder do not have a virtual method so we need to "new" it to be able to add support for child tags.
        /// </summary>
        /// <param name="renderMode"></param>
        /// <returns></returns>
        public new string ToString(TagRenderMode renderMode)
        {
            var sb = new StringBuilder();
            switch (renderMode)
            {
                case TagRenderMode.StartTag:
                    if (_children.Count == 0)
                        return base.ToString(renderMode);
                    
                    sb.AppendLine(base.ToString(renderMode));
                    sb.Append(ChildrenToString());
                    return sb.ToString();

                case TagRenderMode.EndTag:

                    if (_children.Count == 0)
                        return base.ToString(renderMode);

                    //Let's move all children before the end tag.
                    sb.AppendLine(ChildrenToString());
                    sb.Append(base.ToString(renderMode));
                    return sb.ToString();

                case TagRenderMode.SelfClosing:
                    if (_children.Count != 0)
                        throw new InvalidOperationException("Tag has one or more child tags and cannot be self closed. HTML: " + ToString());
                    return base.ToString(renderMode);

                default:
                    if (_children.Count != 0)
                        InnerHtml += ChildrenToString();
                    return base.ToString(renderMode);
            }
        }
        
        /// <summary>
        /// Convert children HTML to a string.
        /// </summary>
        /// <returns></returns>
        public virtual string ChildrenToString()
        {
            var sb = new StringBuilder();
            foreach (var child in _children)
                sb.Append(child.ToString());
            return sb.ToString();
        }

        /// <summary>
        /// Add a collection of children to our list.
        /// </summary>
        /// <param name="children"></param>
        public void AddChildren(IEnumerable<NestedTagBuilder> children)
        {
            foreach (var tagBuilder in children)
            {
                _children.AddLast(tagBuilder);
            }
        }

        /// <summary>
        /// Remove all children from the list.
        /// </summary>
        public void RemoveChildren()
        {
            _children.Clear();
        }
    }
}