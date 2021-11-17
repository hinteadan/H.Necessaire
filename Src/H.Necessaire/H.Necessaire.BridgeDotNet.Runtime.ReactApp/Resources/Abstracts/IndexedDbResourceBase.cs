using System;
using System.Linq;
using System.Threading.Tasks;
using static Retyped.dexie;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public abstract class IndexedDbResourceBase<TIndexedDbStorage, TEntity, TId> : ImADependency where TIndexedDbStorage : ImAnIndexedDBStorage
    {
        #region Construct
        protected TIndexedDbStorage storage;

        public virtual void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            storage = dependencyProvider.Get<TIndexedDbStorage>();
        }
        #endregion

        Func<object, TEntity, TEntity> jsonParsingDecorator;
        public void Use(Func<object, TEntity, TEntity> jsonParsingDecorator)
        {
            this.jsonParsingDecorator = jsonParsingDecorator;
        }

        protected abstract TId GetIdFor(TEntity item);

        protected virtual string GetIdForStore(TId id) => id.ToString();

        protected virtual Task<TEntity> Load(Dexie.Table<object, object> table, TId id)
        {
            TaskCompletionSource<TEntity> taskCompletionSource = new TaskCompletionSource<TEntity>();

            table
                .get(GetIdForStore(id))
                .then(
                    json =>
                    {
                        TEntity result = json.ToType<TEntity>();
                        result = jsonParsingDecorator == null ? result : jsonParsingDecorator.Invoke(json, result);
                        taskCompletionSource.SetResult(result);
                        return json;
                    },
                    x =>
                    {
                        taskCompletionSource.SetException(new InvalidOperationException($"Error Loading model from {table.name}"));
                        return null;
                    }
                );

            return taskCompletionSource.Task;
        }

        protected virtual Task<long> DeleteMany(object filter)
        {
            Dexie.Collection<object, object> filteredCollection = ApplyFilter(filter);

            TaskCompletionSource<long> taskCompletionSource = new TaskCompletionSource<long>();

            filteredCollection
                .delete()
                .then(
                    count =>
                    {
                        taskCompletionSource.SetResult((long)count);
                        return count;
                    },
                    x =>
                    {
                        taskCompletionSource.SetException(new InvalidOperationException($"Error deleting {nameof(filter)} collection"));
                        return null;
                    }
                )
                ;

            return taskCompletionSource.Task;
        }

        protected virtual Task Save(Dexie.Table<object, object> table, TEntity model)
        {
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

            object modelAsJson = model.ToJson();

            table
                .put(modelAsJson)
                .then(
                    x =>
                    {
                        taskCompletionSource.SetResult(true);
                        return x;
                    },
                    x =>
                    {
                        taskCompletionSource.SetException(new InvalidOperationException($"Error saving model to {table.name}"));
                        return x;
                    }
                );

            return taskCompletionSource.Task;
        }

        protected virtual Task Delete(Dexie.Table<object, object> table, TId id)
        {
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

            table
                .delete(GetIdForStore(id))
                .then(
                    json =>
                    {
                        taskCompletionSource.SetResult(true);
                        return json;
                    },
                    x =>
                    {
                        taskCompletionSource.SetException(new InvalidOperationException($"Error deleting model {id} from {table.name}"));
                        return null;
                    }
                );

            return taskCompletionSource.Task;
        }

        protected virtual Task<TEntity[]> Search(object filter)
        {
            Dexie.Collection<object, object> filteredCollection = ApplyFilter(filter);

            TaskCompletionSource<TEntity[]> taskCompletionSource = new TaskCompletionSource<TEntity[]>();

            filteredCollection
                .toArray()
                .then(
                    x =>
                    {
                        var result = x.Select(o =>
                        {
                            TEntity res = o.ToType<TEntity>();
                            res = jsonParsingDecorator == null ? res : jsonParsingDecorator.Invoke(o, res);
                            return res;
                        }).ToArray();
                        taskCompletionSource.SetResult(result);
                        return x;
                    },
                    x =>
                    {
                        taskCompletionSource.SetException(new InvalidOperationException($"Error filtering {nameof(TEntity)} collection"));
                        return null;
                    }
                );

            return taskCompletionSource.Task;
        }

        protected virtual Task<long> Count(object filter)
        {
            Dexie.Collection<object, object> filteredCollection = ApplyFilter(filter);

            TaskCompletionSource<long> taskCompletionSource = new TaskCompletionSource<long>();

            filteredCollection
                .count()
                .then(
                    count =>
                    {
                        taskCompletionSource.SetResult((long)count);
                        return count;
                    },
                    x =>
                    {
                        taskCompletionSource.SetException(new InvalidOperationException($"Error counting {nameof(filter)} collection"));
                        return null;
                    }
                );

            return taskCompletionSource.Task;
        }

        protected abstract Dexie.Collection<object, object> ApplyFilter(object filter);
    }
}
