﻿// ------------------------------------------------------------
//  Copyright (c) saxu@microsoft.com.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OData.Edm;

namespace Annotation.EdmUtil
{
    /// <summary>
    /// A bound operation segment, for example: ~/users/Ns.ResetAll(....)
    /// </summary>
    public class OperationSegment : PathSegment
    {
        /// <summary>
        /// Initializes a new instance of <see cref="OperationSegment"/> class.
        /// </summary>
        /// <param name="operation">The wrapped Edm operation (function or action).</param>
        /// <param name="entitySet">The wrapped entity set.</param>
        public OperationSegment(IEdmOperation operation, IEdmEntitySetBase entitySet)
            : base(operation?.Name)
        {
            Operation = operation ?? throw new ArgumentNullException(nameof(operation));

            EdmType = operation.ReturnType?.Definition;

            NavigationSource = entitySet;

            Target = operation.TargetName();

            if (EdmType != null)
            {
                IsSingle = EdmType.TypeKind != EdmTypeKind.Collection;
            }
        }

        /// <inheritdoc/>
        public override SegmentKind Kind => SegmentKind.Operation;

        public IEdmOperation Operation { get; }

        public override bool IsSingle { get; }

        public override IEdmType EdmType { get; }

        public override IEdmNavigationSource NavigationSource { get; }

        public override string Target { get; }

        /// <summary>
        /// Gets the Uri literal for the operation segment.
        /// It should be the name of the operation.
        /// </summary>
        public override string UriLiteral => Operation.FullName();

        /// <inheritdoc/>
        public override bool Match(PathSegment other)
        {
            OperationSegment otherOperationSegment = other as OperationSegment;
            if (otherOperationSegment == null)
            {
                return false;
            }

            return ReferenceEquals(Operation, otherOperationSegment.Operation);
        }
    }
}
