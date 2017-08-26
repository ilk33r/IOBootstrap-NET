using Realms;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Concurrency;

namespace IOBootstrap.NET.Core.Database
{
    public class IODatabase
    {

        #region Properties

        private Realm realm;
        private RealmConfiguration realmConfiguration;

        #endregion

        #region Initialization Methods

        public IODatabase(string databasePath)
        {
            // Initialize properties
            this.realmConfiguration = new RealmConfiguration(databasePath);
            this.realmConfiguration.ShouldDeleteIfMigrationNeeded = true;
            this.realm = Realm.GetInstance(this.realmConfiguration);
        }

        public Realm GetRealmForThread()
        {
            // Retrieve realm
            return Realm.GetInstance(this.realmConfiguration);
        }

        public Realm GetRealmForMainThread()
        {
            // Retrieve default realm
            return this.realm;
        }

        #endregion

        #region Database Operations

        public IObservable<Object> DeleteAll()
        {
            // Handle delete all data
            Func<IObserver<Object>, IDisposable> handleDeleteAllData = (subscriber) =>
            {
                // Obtain realm instance
                Realm realmInstance = this.GetRealmForThread();

                // Begin write transaction
                Transaction realmTransaction = realmInstance.BeginWrite();

                // Delete all objects
                realmInstance.RemoveAll();

                // Write transaction
                realmTransaction.Commit();

                // Send completed event to listeners
                subscriber.OnCompleted();

                // Return disposable
                return null;
            };

            // Create a signal
            return Observable.Create(handleDeleteAllData)
                             .SubscribeOn(NewThreadScheduler.Default);
        }

        public IObservable<Object> DeleteEntity<TEntity>(TEntity entity) where TEntity: RealmObject
        {
            // Handle delete entity
            Func<IObserver<Object>, IDisposable> handleDeleteAllData = (subscriber) =>
            {
				// Obtain a Realm instance
                Realm realmInstance = this.GetRealmForThread();

				// Begin write transaction
				Transaction realmTransaction = realmInstance.BeginWrite();

				// Delete all entity
                realmInstance.Remove(entity);

				// Write transaction
				realmTransaction.Commit();

				// Send completed event to listeners
				subscriber.OnCompleted();

                // Return disposable
                return null;
            };

			// Create a signal
			return Observable.Create(handleDeleteAllData)
						.SubscribeOn(NewThreadScheduler.Default);
        }

        public IObservable<IList<TEntity>> InsertEntities<TEntity>(IList<TEntity> entities) where TEntity: RealmObject
		{
			// Handle insert entities
            Func<IObserver<IList<TEntity>>, IDisposable> handleInsertEntities = (subscriber) => {
				// Obtain a Realm instance
				Realm realmInstance = this.GetRealmForThread();

				// Begin write transaction
				Transaction realmTransaction = realmInstance.BeginWrite();

                // Loop throught entities
                foreach(TEntity entity in entities) {
					// Add objects to database
					realmInstance.Add(entity);               
                }

				// Write transaction
				realmTransaction.Commit();

                // Send entities to listeners
                subscriber.OnNext(entities);

				// Send completed event to listeners
				subscriber.OnCompleted();

				// Return disposable
				return null;
            };

			// Create a signal
			return Observable.Create(handleInsertEntities)
					.SubscribeOn(NewThreadScheduler.Default);
		}

		public IObservable<TEntity> InsertEntity<TEntity>(TEntity entity) where TEntity: RealmObject
		{
			// Handle insert entity
            Func<IObserver<TEntity>, IDisposable> handleInsertEntity = (subscriber) => {
				// Obtain a Realm instance
				Realm realmInstance = this.GetRealmForThread();

				// Begin write transaction
				Transaction realmTransaction = realmInstance.BeginWrite();

				// Add objects to database
				realmInstance.Add(entity);

				// Send entity to listeners
				subscriber.OnNext(entity);

				// Send completed event to listeners
				subscriber.OnCompleted();

				// Return disposable
				return null;
			};

			// Create a signal
			return Observable.Create(handleInsertEntity)
					.SubscribeOn(NewThreadScheduler.Default);
		}

		public IObservable<IList<TEntity>> UpdateEntities<TEntity>(IList<TEntity> entities) where TEntity: RealmObject
		{
            // Handle update entities
            Func<IObserver<IList<TEntity>>, IDisposable> handleUpdateEntities = (subscriber) =>
            {
				// Obtain a Realm instance
				Realm realmInstance = this.GetRealmForThread();

				// Begin write transaction
				Transaction realmTransaction = realmInstance.BeginWrite();

				// Loop throught entities
				foreach (TEntity entity in entities)
				{
					// Add objects to database
                    realmInstance.Add(entity, true);
				}
				
                // Send entity to listeners
				subscriber.OnNext(entities);

				// Send completed event to listeners
				subscriber.OnCompleted();

				// Return disposable
				return null;
            };

			// Create a signal
			return Observable.Create(handleUpdateEntities)
					.SubscribeOn(NewThreadScheduler.Default);
		}

		public IObservable<TEntity> UpdateEntity<TEntity>(TEntity entity) where TEntity: RealmObject
		{
			// Handle update entities
			Func<IObserver<TEntity>, IDisposable> handleUpdateEntity = (subscriber) =>
			{
				// Obtain a Realm instance
				Realm realmInstance = this.GetRealmForThread();

				// Begin write transaction
				Transaction realmTransaction = realmInstance.BeginWrite();

                // Add object to database
                realmInstance.Add(entity, true);

				// Send entity to listeners
				subscriber.OnNext(entity);

				// Send completed event to listeners
				subscriber.OnCompleted();

				// Return disposable
				return null;
			};

			// Create a signal
			return Observable.Create(handleUpdateEntity)
					.SubscribeOn(NewThreadScheduler.Default);
		}   

        #endregion
    }
}