﻿// ------------------------------------------------------------
//  Copyright (c) saxu@microsoft.com.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;

namespace Annotation.EdmUtil
{
    public class UriPath// : IEquatable<UriPath>
    {
        private string _pathLiteral;

        /// <summary>
        /// The segments that make up this path.
        /// </summary>
        private readonly IList<PathSegment> segments;

        public UriPath(IEnumerable<PathSegment> segments)
        {
            this.segments = segments.ToList();

            Kind = CalculatePathKind(this.segments);
        }

        public IList<PathSegment> Segments => segments;

        /// <summary>
        /// Gets the first segment in the path.
        /// </summary>
        public PathSegment FirstSegment => this.segments.FirstOrDefault();

        /// <summary>
        /// Get the last segment in the path. Returns null if the path is empty.
        /// </summary>
        public PathSegment LastSegment => this.segments.LastOrDefault();

        /// <summary>
        /// Get the number of segments in this path.
        /// </summary>
        public int Count => this.segments.Count;

        public PathKind Kind { get; }

        private string _targetString;

        public override string ToString()
        {
            if (_pathLiteral == null)
            {
                _pathLiteral = "~/" + string.Join("/", segments.Select(s => s.UriLiteral));
            }

            return _pathLiteral;
        }

        public string TargetString
        {
            get
            {
                if (_targetString == null)
                {
                    _targetString = this.GetTargetString();
                }

                return _targetString;
            }
        }

        private static PathKind CalculatePathKind(IList<PathSegment> segments)
        {
            PathSegment lastSegment = segments.Last();
            if (lastSegment is NavigationSegment)
            {
                if (lastSegment.IsSingle)
                {
                    return PathKind.SingleNavigation;
                }

                return PathKind.CollectionNavigation;
            }
            else if (lastSegment is PropertySegment)
            {
                return PathKind.Property;
            }
            else if (lastSegment is OperationSegment)
            {
                return PathKind.Operation;
            }
            else if (lastSegment.Kind == SegmentKind.Type)
            {
                return PathKind.TypeCast;
            }
            else
            {
                int count = segments.Count;
                if (count == 1)
                {
                    if (lastSegment is EntitySetSegment)
                    {
                        return PathKind.EntitySet;
                    }
                    else if (lastSegment is SingletonSegment)
                    {
                        return PathKind.Singleton;
                    }
                    else if (lastSegment is OperationImportSegment)
                    {
                        return PathKind.OperationImport;
                    }

                    throw new System.Exception($"Unknown path kind!");
                }
                else if (count == 2 && lastSegment is KeySegment &&
                    segments[0] is EntitySetSegment)
                {
                    return PathKind.Entity;
                }
                else
                {
                    PathSegment pre = segments[segments.Count - 2];
                    if (pre is NavigationSegment && lastSegment is KeySegment)
                    {
                        return PathKind.SingleNavigation;
                    }

                    throw new System.Exception($"Unknown path kind!");
                }
            }
        }
/*
        public bool Equals(UriPath other)
        {
            if (other == null || this.Count != other.Count)
            {
                return false;
            }

            for (int i = 0; i < Count; i++)
            {
                PathSegment originalSegment = segments[i];
                PathSegment otherSegment = other.Segments[i];

                if (!originalSegment.Match(otherSegment))
                {
                    return false;
                }
            }

            return true;
        }*/
    }
}
