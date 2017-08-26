using Realms;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Concurrency;

namespace IOBootstrap.NET.Core.Database
{
    public interface IIODatabase
    {

        Realm GetRealmForThread();
        Realm GetRealmForMainThread();
        IObservable<Object> DeleteAll();
        IObservable<Object> DeleteEntity<TEntity>(TEntity entity) where TEntity : RealmObject;
        IObservable<IList<TEntity>> InsertEntities<TEntity>(IList<TEntity> entities) where TEntity : RealmObject;
        IObservable<TEntity> InsertEntity<TEntity>(TEntity entity) where TEntity : RealmObject;
        IObservable<IList<TEntity>> UpdateEntities<TEntity>(IList<TEntity> entities) where TEntity : RealmObject;
        IObservable<TEntity> UpdateEntity<TEntity>(TEntity entity) where TEntity : RealmObject;
		
	}
}
