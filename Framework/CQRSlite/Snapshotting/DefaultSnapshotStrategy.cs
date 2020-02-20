﻿using System;
using System.Reflection;
using CQRSlite.Domain;

namespace CQRSlite.Snapshotting
{
    /// <inheritdoc />
    /// <summary>
    /// Default implementation of snapshot strategy interface/
    /// Snapshots aggregates of type SnapshotAggregateRoot every 100th event.
    /// </summary>
    public class DefaultSnapshotStrategy : ISnapshotStrategy
    {
        private int snapshotInterval = 100;

        public DefaultSnapshotStrategy() { }
        public DefaultSnapshotStrategy(int interval)
        {
            snapshotInterval = interval;
        }

        public bool IsSnapshotable(Type aggregateType)
        {
            if (aggregateType.GetTypeInfo().BaseType == null)
                return false;
            if (aggregateType.GetTypeInfo().BaseType.GetTypeInfo().IsGenericType &&
                aggregateType.GetTypeInfo().BaseType.GetGenericTypeDefinition() == typeof(SnapshotAggregateRoot<>))
                return true;
            return IsSnapshotable(aggregateType.GetTypeInfo().BaseType);
        }

        public bool ShouldMakeSnapShot(AggregateRoot aggregate)
        {
            if (!IsSnapshotable(aggregate.GetType()))
                return false;

            var i = aggregate.Version;
            for (var j = 0; j < aggregate.GetUncommittedChanges().Length; j++)
                if (++i % snapshotInterval == 0 && i != 0)
                    return true;
            return false;
        }
    }
}