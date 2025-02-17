﻿
using System;
using System.Collections.Generic;
using System.Linq;
using Annotation.EdmUtil.Commons;
using Microsoft.OData.Edm;

namespace Annotation.EdmUtil
{
    /// <summary>
    /// A key segment, for example ~/users({id})
    /// or ~/users/{id}
    /// </summary>
    public class KeySegment : PathSegment
    {
        /// <summary>
        /// Initializes a new instance of <see cref="KeySegment"/> class.
        /// </summary>
        /// <param name="keys">The key/value pairs for the key segment.</param>
        /// <param name="entityType">The declaring entity type.</param>
        /// <param name="navigationSource">The related navigation source.</param>
        public KeySegment(IDictionary<string, string> keys,
            IEdmEntityType entityType,
            IEdmNavigationSource navigationSource)
            : base(keys.ToKeyValueString())
        {
            Values = keys;
            DeclaringType = entityType;
            EdmType = entityType;
            NavigationSource = navigationSource;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="KeySegment"/> class.
        /// </summary>
        /// <param name="keys">The key/value pairs for the key segment.</param>
        /// <param name="edmEntityType">The declaring entity type.</param>
        /// <param name="navigationSource">The related navigation source.</param>
        public KeySegment(string keys, IEdmEntityType entityType, IEdmNavigationSource navigationSource)
            : base(keys)
        {
            keys.ExtractKeyValuePairs(out IDictionary<string, string> values, out _);
            Values = values;
            DeclaringType = entityType;
            EdmType = entityType;
            NavigationSource = navigationSource;
        }

        /// <inheritdoc/>
        public override SegmentKind Kind => SegmentKind.Key;

        public IDictionary<string, string> Values { get; }

        /// <summary>
        /// KeySegment is always single value. So IsSingle is always is true.
        /// </summary>
        public override bool IsSingle => true;

        public IEdmEntityType DeclaringType { get; }

        public override IEdmType EdmType { get; }

        public override IEdmNavigationSource NavigationSource { get;}

        public override string Target => throw new NotImplementedException();

        /// <summary>
        /// Gets the Uri literal for the key segment.
        /// It should be the name of the key.
        /// </summary>
        public override string UriLiteral
        {
            get
            {
                var keys = DeclaringType.Key();
                if (!keys.Any())
                {
                    throw new Exception($"The entity type '{DeclaringType.FullName()}' in a KeySegment '{Identifier}' has not keys?!");
                }

                if (keys.Count() == 1)
                {
                    var key = keys.Single();

                    // {key}
                    return "{" + key.Name + "}";
                }

                // {k1={k1},k2={k2}}
                return "{" + String.Join(",", keys.Select(k => $"{k.Name}={{{k.Name}}}")) + "}";
            }
        }

        /// <inheritdoc/>
        public override bool Match(PathSegment other)
        {
            KeySegment otherKeySegment = other as KeySegment;
            if (otherKeySegment == null)
            {
                return false;
            }

            // Compare the key segment using It's declaring type.
            return ReferenceEquals(EdmType, otherKeySegment.EdmType);
        }
    }
}
